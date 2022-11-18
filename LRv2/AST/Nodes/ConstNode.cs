namespace LRv2.SyntaxAnalyzer.Nodes;

public class ConstNode : BaseNode
{
    public bool Value { get; }

	public ConstNode(bool value)
	{
		Value = value;
	}
}
