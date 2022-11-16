using System.Text.RegularExpressions;

namespace Some;

public static class StringExtention
{
    public static bool IsIdent(this string str)
        => Regex.IsMatch(str, @"[a-z]{1,11}");

    public static bool IsUnaryOp(this string str)
        => str == "NOT";
    
    public static bool IsBinOp(this string str)
        => str == "OR"
        || str == "AND"
        || str == "EQU";
}

public class Tree
{
    public string Value { get; set; }

    public List<Tree>? Childs { get; set; }
}

public class Program
{
    public static readonly Dictionary<string, int> priority = new()
    {
        {"EQU", 1},

        {"OR", 2},

        {"AND", 3},

        {"NOT", 4},
    };

    public static void Main(string[] args)
    {
        var expr = "NOT x OR NOT (q EQU y OR z)";
        var rpn = GetRPN(expr);
        var tree = ExprToTree(rpn);

        List<string> list = new();
        GetLeafs(tree, list);
        Console.WriteLine();
    }

    public static string GetRPN(string expr)
    {
        expr = Regex.Replace(expr, @"\(", " ( ");
        expr = Regex.Replace(expr, @"\)", " ) ");
        expr = Regex.Replace(expr, @"\s+", " ");

        var symbols = expr
            .Split(" ")
            .Where(s => !string.IsNullOrEmpty(s));

        return GetRPN(symbols);
    }

    public static string GetRPN(IEnumerable<string> symbols)
    {
        var result = string.Empty;
        var stack = new Stack<string>();

        foreach (var symbol in symbols)
        {
            if (symbol.IsIdent())
            {
                result += $"{symbol} ";
                continue;
            }

            if (symbol == "(")
            {
                stack.Push($"{symbol}");
                continue;
            }

            if (symbol == ")")
            {
                while (stack.Peek() != "(")
                {
                    var popElem = stack.Pop();
                    result += $"{popElem} ";
                }
                stack.Pop();
                continue;
            }

            int priority = Program.priority[$"{symbol}"];
            if (stack.Count == 0 || stack.Peek() == "(" || Program.priority[stack.Peek()] < priority)
            {
                stack.Push($"{symbol}");
            }
            else
            {
                while (stack.Any() && Program.priority[stack.Peek()] >= priority && stack.Peek() != "(")
                {
                    var popElem = stack.Pop();
                    result += $"{popElem} ";
                }
                stack.Push($"{symbol}");
            }
        }

        while (stack.Any())
        {
            var popElem = stack.Pop();
            result += $"{popElem} ";
        }

        return result.Trim();
    }

    public static Tree ExprToTree(string expr)
    {
        var parts = expr.Split(" ");
        var stack = new Stack<Tree>();

        foreach(var part in parts)
        {
            if (part.IsIdent())
            {
                stack.Push(new Tree()
                {
                    Value = part,
                    Childs = null
                });

                continue;
            }

            if (part.IsUnaryOp())
            {
                var elem = stack.Pop();

                stack.Push(new Tree()
                {
                    Value = part,
                    Childs = new() { elem }
                });

                continue;
            }

            if (part.IsBinOp())
            {
                var elem2 = stack.Pop();
                var elem1 = stack.Pop();

                stack.Push(new Tree()
                {
                    Value = part,
                    Childs = new() { elem1, elem2 }
                });
            }
        }

        return stack.Pop();
    }

    public static void GetLeafs(Tree tree, List<string> list)
    {
        if (tree.Childs == null)
        {
            list.Add(tree.Value);
        }
        else
        {
            foreach (var child in tree.Childs)
            {
                GetLeafs(child, list);
            }
        }
    }
}