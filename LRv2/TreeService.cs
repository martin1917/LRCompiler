using LRv2.LexicalAnalyzer;
using LRv2.SyntaxAnalyzer;
using LRv2.SyntaxAnalyzer.Nodes;
using System.Text.RegularExpressions;

namespace LRv2;

public class TreeService
{
	private readonly TreeNode cst;

	public TreeService(TreeNode cst)
	{
		this.cst = cst;
	}

    public ProgramNode GetTree()
    {
        var res = new ProgramNode()
        {
            Vars = GetVarNode(cst.Childs!.First().Childs![1]),
            Body = GetStatement(cst.Childs!.Last())
        };

        return res;
    }

    private VarNode GetVarNode(TreeNode node)
    {
        var res = new List<string>();

        void GetAllVariablesInternal(TreeNode node, List<string> idents)
        {
            idents.Add(node.Childs!.First().Childs!.First().Value);
            if (node.Childs!.Count == 3)
            {
                GetAllVariablesInternal(node.Childs!.Last(), idents);
            }
        }

        GetAllVariablesInternal(node, res);

        return new VarNode()
        {
            Variables = res
                .Select(s => new VariableNode() { Ident = s })
                .ToList()
        };
    }

    private List<StatemantNode> GetAssigments(TreeNode node)
    {
        List<StatemantNode> res = new();

        void GetAssigmentsInternal(TreeNode node, List<StatemantNode> assignmentNodes)
        {
            if (node.Value == "<assignment>")
            {
                var expr = ExpressionToTree.Convert(GetValueInLeaf(node.Childs![2]));

                var nodeIdent = node.Childs!.First();
                var ident = nodeIdent.Childs!.First().Value;

                assignmentNodes.Add(new AssignmentNode()
                {
                    Variable = new VariableNode()
                    {
                        Ident = ident
                    },
                    Expression = expr
                });
            }
            else if (node.Value != "<description_calculations>")
            {
                if (node.Value == "<list_operators>") return;
                node.Childs?.ForEach(c => GetAssigmentsInternal(c, assignmentNodes));
            }
        }

        GetAssigmentsInternal(node, res);
        return res;
    }

    private List<StatemantNode> GetOperators(TreeNode node)
    {
        List<StatemantNode> res = new();

        void GetOperatorsInternal(TreeNode node, List<StatemantNode> operatorNodes)
        {
            if (node.Value == "<operator>")
            {
                var nameNodes = node.Childs!.Select(c => c.Value);

                if (nameNodes.Contains("read"))
                {
                    operatorNodes.Add(new ReadNode()
                    {
                        Vars = GetVarNode(node.Childs![2])
                    });
                }
                else if (nameNodes.Contains("write"))
                {
                    operatorNodes.Add(new WriteNode()
                    {
                        Vars = GetVarNode(node.Childs![2])
                    });
                }
                else if (nameNodes.Contains("if") && nameNodes.Contains("else"))
                {
                    var predicate = ExpressionToTree.Convert(GetValueInLeaf(node.Childs![2]));
                    var left = GetStatement(node.Childs![5]);
                    var right = GetStatement(node.Childs![7]);

                    operatorNodes.Add(new IfElseNode()
                    {
                        Predicate = predicate,
                        FalseBranch = left,
                        TrueBranch = right,
                    });
                }
            }
            else if (node.Value != "<description_calculations>")
            {
                if (node.Value == "<list_assignments>") return;
                node.Childs?.ForEach(child => GetOperatorsInternal(child, operatorNodes));
            }
        }

        GetOperatorsInternal(node, res);
        return res;
    }

    private ScopeNode GetStatement(TreeNode node)
    {
        var statements = new List<StatemantNode>();

        var assignments = GetAssigments(node.Childs![1]);
        var operators = GetOperators(node.Childs![1]);

        statements.AddRange(assignments);
        statements.AddRange(operators);

        return new ScopeNode() { Statements = statements };
    }

    private List<string> GetValueInLeaf(TreeNode node)
    {
        List<string> result = new();

        void GetValueInLeafInternal(TreeNode node, List< string> values)
        {
            if(node.Childs == null)
            {
                values.Add(node.Value);
            }
            else
            {
                foreach(var child in node.Childs)
                {
                    GetValueInLeafInternal(child, values);
                }
            }
        }

        GetValueInLeafInternal(node, result);
        return result;
    }
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