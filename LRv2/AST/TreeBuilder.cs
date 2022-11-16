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

    // <assignment> ::= <id> = <expr> ;
    private List<StatemantNode> GetAssigments(TreeNode node)
    {
        List<StatemantNode> res = new();

        void GetAssigmentsInternal(TreeNode node, List<StatemantNode> assignmentNodes)
        {
            if (node.Value == "<assignment>")
            {
                var expr = ExpressionToTreePriority.Convert(GetValueInLeaf(node.Childs![2]));

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

    // <operator> ::= read ( <list_variables> )
    // <operator> ::= write ( <list_variables> )
    // <operator> ::= if ( <expr> ) then <description_calculations> else <description_calculations>
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
                    var predicate = ExpressionToTreePriority.Convert(GetValueInLeaf(node.Childs![2]));
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

    // <description_calculations> ::= <list_actions>
    // <list_actions> ::= <list_assignments> | <list_assignments> <list_actions>
    // <list_actions> ::= <list_operators> | <list_operators> <list_actions>
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
