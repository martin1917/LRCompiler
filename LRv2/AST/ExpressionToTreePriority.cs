using LRv2.LexicalAnalyzer;
using LRv2.SyntaxAnalyzer.Nodes;
using System.Text.RegularExpressions;

namespace LRv2.AST;

public class ExpressionToTreePriority
{
    private static readonly Dictionary<string, int> priorities = new()
    {
        {"equ", 1},

        {"or", 2},

        {"and", 3},

        {"not", 4},
    };

    private static string GetRPN(IEnumerable<string> symbols)
    {
        var result = string.Empty;
        var stack = new Stack<string>();

        foreach (var symbol in symbols)
        {
            if (Regex.IsMatch(symbol, TypeTerminal.Ident.Regex)
                && !Consts.IsKeyWord(symbol))
            {
                result += $"{symbol} ";
                continue;
            }

            if (symbol == TypeTerminal.Lparam.Name)
            {
                stack.Push($"{symbol}");
                continue;
            }

            if (symbol == TypeTerminal.Rparam.Name)
            {
                while (stack.Peek() != TypeTerminal.Lparam.Name)
                {
                    var popElem = stack.Pop();
                    result += $"{popElem} ";
                }
                stack.Pop();
                continue;
            }

            int priority = priorities[$"{symbol}"];
            if (stack.Count == 0 || stack.Peek() == TypeTerminal.Lparam.Name || priorities[stack.Peek()] < priority)
            {
                stack.Push($"{symbol}");
            }
            else
            {
                while (stack.Any() && priorities[stack.Peek()] >= priority)
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

    public static BaseNode Convert(IEnumerable<string> items)
    {
        var parts = GetRPN(items).Split(" ");
        var stack = new Stack<BaseNode>();

        foreach (var part in parts)
        {
            if (Regex.IsMatch(part, TypeTerminal.Ident.Regex) && !Consts.IsKeyWord(part))
            {
                stack.Push(new VariableNode()
                {
                    Ident = part
                });

                continue;
            }

            if (Regex.IsMatch(part, TypeTerminal.Const.Regex))
            {
                stack.Push(new ConstNode()
                {
                    Value = part == "1"
                });

                continue;
            }

            if (Consts.IsUnOp(part))
            {
                var elem = stack.Pop();

                stack.Push(new UnaryOpNode()
                {
                    Operation = part,
                    Node = elem
                });

                continue;
            }

            if (Consts.IsBinOp(part))
            {
                var elem2 = stack.Pop();
                var elem1 = stack.Pop();

                stack.Push(new BinOpNode()
                {
                    Operation = part,
                    Left = elem1,
                    Right = elem2
                });
            }
        }

        return stack.Pop();
    }
}
