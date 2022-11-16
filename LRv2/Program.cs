using LRv2.LexicalAnalyzer;
using LRv2.SyntaxAnalyzer;
using LRv2.SyntaxAnalyzer.Nodes;
using System.Text.RegularExpressions;

namespace LRv2;

public class Program
{
    static string[] examples =
    {
        @"
            VAR 
                x, y: LOGICAL;
            BEGIN
                IF (x EQU 1) THEN
                BEGIN
                    y = 0;
                    x = NOT (y AND (NOT (x OR y)));
                END
                ELSE
                BEGIN
                    READ(a, b, c);
                    IF ((b EQU c) AND a) THEN
                    BEGIN
                        a = 1;
                        x = NOT (c);
                    END
                    ELSE
                    BEGIN
                        c = 1;
                    END
                    WRITE(x);
                END
            END
        ",

        @"
            VAR
                x, y, z, d, w: LOGICAL;

            BEGIN
                READ(x, y, z, d, w);

                resone = x OR y AND z;
                restwo = (x OR y) AND z;

                resthree = x OR y AND z OR w;
                resfour = (x OR y) AND (z OR w);

                resfive = NOT x OR y EQU z OR w;
            END
        ",

        @"
            VAR
	            x, y, z, w: LOGICAL;
	
            BEGIN
	            READ(x, y, z, w);
	
	            IF (NOT (x OR y) AND z) THEN
	            BEGIN
		            WRITE(z, w);
		            z = NOT ((x EQU z) AND w);
	            END
	            ELSE
	            BEGIN
		            x = w OR z AND x;
		            w = x EQU x AND (NOT x);
		            READ(z, x);
		
		            IF (z EQU x) THEN
		            BEGIN
			            READ(w);
			            WRITE(w);
		            END
		            ELSE
		            BEGIN
			            w = z OR x AND z AND x;
			            WRITE(z,w);
			            READ(w);
		            END
	            END
            END
        "
    };

    public static void Main(string[] args)
    {
        Test(2);
    }

    private static void Test(int index)
    {
        var code = examples[index];

        var lexer = new Lexer(code);
        var lexems = lexer.GetLexems();

        var table = LRTableGenerator.Generate();
        var parser = new Parser(table);

        var ast = parser.Parse(lexems);
    }
}

public static class Consts
{
    private static readonly List<string> keyWords = new()
    {
        "not",
        "or",
        "and",
        "equ",
        "begin",
        "end",
        "var",
        "if",
        "else",
        "logical",
    };
    
    private static readonly List<string> binOps = new()
    {
        "or",
        "and",
        "equ",
    };
    
    private static readonly List<string> unOps = new()
    {
        "not",
    };

    public static bool IsKeyWord(string str) => keyWords.Contains(str);

    public static bool IsBinOp(string str) => binOps.Contains(str);

    public static bool IsUnOp(string str) => unOps.Contains(str);
}

public class ExpressionToTree
{
    private static readonly Dictionary<string, int> priorities = new()
    {
        {"equ", 1},

        {"or", 2},

        {"and", 3},

        {"not", 4},
    };

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

    public static BaseNode Convert(string expr)
    {
        var parts = expr.Split(" ");
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
                    Value = bool.Parse(part)
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