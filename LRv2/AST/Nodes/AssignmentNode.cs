namespace LRv2.SyntaxAnalyzer.Nodes;

public class AssignmentNode : StatemantNode
{
    public VariableNode Variable { get; set; }

    public BaseNode Expression { get; set; }
}
