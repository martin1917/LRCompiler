namespace LRv2.SyntaxAnalyzer.Nodes;

public class AssignmentNode : BaseNode
{
    public VariableNode Variable { get; set; }

    public BaseNode Expression { get; set; }
}
