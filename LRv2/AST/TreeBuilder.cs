using LRv2.SyntaxAnalyzer.Nodes;
using LRv2.SyntaxAnalyzer;

namespace LRv2.AST;

public class TreeBuilder
{
    private readonly TreeNode cst;

    public TreeBuilder(TreeNode cst)
    {
        this.cst = cst;
    }

    // <program> ::= <variable_declaration> <description_calculations>
    public ProgramNode BuildAST()
    {
        var res = new ProgramNode()
        {
            Vars = GetVarNode(cst.Childs!.First().Childs![1]),
            Body = GetStatement(cst.Childs!.Last())
        };

        return res;
    }

    // <list_variables> ::= <variable> | <variable> , <list_variables>
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

    private List<StatemantNode> GetAssignmentsAndOperators(TreeNode node)
    {
        List<StatemantNode> res = new();

        void GetAssignmentsAndOperatorsInternal(TreeNode node, List<StatemantNode> res)
        {
            if (node.Value == "<assignment>")
            {
                var expr = ExpressionToTreePriority.Convert(GetValueInLeaf(node.Childs![2]));

                var nodeIdent = node.Childs!.First();
                var ident = nodeIdent.Childs!.First().Value;

                res.Add(new AssignmentNode()
                {
                    Variable = new VariableNode()
                    {
                        Ident = ident
                    },
                    Expression = expr
                });
            }

            else if (node.Value == "<operator>")
            {
                var nameNodes = node.Childs!.Select(c => c.Value);

                if (nameNodes.Contains("read"))
                {
                    res.Add(new ReadNode()
                    {
                        Vars = GetVarNode(node.Childs![2])
                    });
                }
                else if (nameNodes.Contains("write"))
                {
                    res.Add(new WriteNode()
                    {
                        Vars = GetVarNode(node.Childs![2])
                    });
                }
                else if (nameNodes.Contains("if") && nameNodes.Contains("else"))
                {
                    var predicate = ExpressionToTreePriority.Convert(GetValueInLeaf(node.Childs![2]));
                    var trueBranch = GetStatement(node.Childs![5]);
                    var falseBranch = GetStatement(node.Childs![7]);

                    res.Add(new IfElseNode()
                    {
                        Predicate = predicate,
                        FalseBranch = falseBranch,
                        TrueBranch = trueBranch,
                    });
                }
            }

            else if (node.Value != "<description_calculations>")
            {
                node.Childs?.ForEach(child => GetAssignmentsAndOperatorsInternal(child, res));
            }
        }

        GetAssignmentsAndOperatorsInternal(node, res);
        return res;
    }

    // <description_calculations> ::= <list_actions>
    // <list_actions> ::= <list_assignments> | <list_assignments> <list_actions>
    // <list_actions> ::= <list_operators> | <list_operators> <list_actions>
    private ScopeNode GetStatement(TreeNode node)
    {
        var statements = GetAssignmentsAndOperators(node.Childs![1]);

        return new ScopeNode() { Statements = statements };
    }

    private List<string> GetValueInLeaf(TreeNode node)
    {
        List<string> result = new();

        void GetValueInLeafInternal(TreeNode node, List<string> values)
        {
            if (node.Childs == null)
            {
                values.Add(node.Value);
            }
            else
            {
                foreach (var child in node.Childs)
                {
                    GetValueInLeafInternal(child, values);
                }
            }
        }

        GetValueInLeafInternal(node, result);
        return result;
    }
}
