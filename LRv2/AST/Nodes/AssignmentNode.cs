namespace LRv2.SyntaxAnalyzer.Nodes;

public class AssignmentNode : StatemantNode
{
    public VariableNode Variable { get; }

    public BaseNode Expression { get; }

    public AssignmentNode(VariableNode variable, BaseNode expression)
    {
        Variable = variable;
        Expression = expression;
    }
}
