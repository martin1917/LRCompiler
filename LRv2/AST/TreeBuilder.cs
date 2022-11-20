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
        var variableDeclaration = GetVarNode(cst.Childs!.First().Childs![1]);
        var descriptionCalculations = GetStatement(cst.Childs!.Last());
        var program = new ProgramNode(variableDeclaration, descriptionCalculations);
        return program;
    }

    // <list_variables> ::= <variable> | <variable> , <list_variables>
    private VarNode GetVarNode(TreeNode node)
    {
        var res = new List<VariableNode>();

        void GetAllVariablesInternal(TreeNode node, List<VariableNode> variables)
        {
            var nodeIdent = node.Childs!.First();
            var ident = nodeIdent.Childs!.First().Value;
            variables.Add(new VariableNode(ident));

            if (node.Childs!.Count == 3)
            {
                GetAllVariablesInternal(node.Childs!.Last(), variables);
            }
        }

        GetAllVariablesInternal(node, res);

        var listVariables = new VarNode(res);
        return listVariables;
    }

    // <list_assignments> ::= <assignment> | <assignment><list_assignments>
    // <list_operators> ::= <operator> | <operator><list_operators>
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
                var variable = new VariableNode(ident);

                res.Add(new AssignmentNode(variable, expr));
            }

            else if (node.Value == "<operator>")
            {
                var nameNodes = node.Childs!.Select(c => c.Value);

                if (nameNodes.Contains("read"))
                {
                    var variablesInBracket = GetVarNode(node.Childs![2]);
                    res.Add(new ReadNode(variablesInBracket));
                }
                else if (nameNodes.Contains("write"))
                {
                    var variablesInBracket = GetVarNode(node.Childs![2]);
                    res.Add(new WriteNode(variablesInBracket));
                }
                else if (nameNodes.Contains("if") && nameNodes.Contains("else"))
                {
                    var predicate = ExpressionToTreePriority.Convert(GetValueInLeaf(node.Childs![2]));
                    var trueBranch = GetStatement(node.Childs![5]);
                    var falseBranch = GetStatement(node.Childs![7]);

                    res.Add(new IfElseNode(predicate, trueBranch, falseBranch));
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

    // <list_actions> ::= <list_assignments> | <list_assignments> <list_actions>
    // <list_actions> ::= <list_operators> | <list_operators> <list_actions>
    private ScopeNode GetStatement(TreeNode node)
    {
        var statements = GetAssignmentsAndOperators(node.Childs![1]);
        return new ScopeNode(statements);
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
