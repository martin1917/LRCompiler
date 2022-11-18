namespace LRv2.SyntaxAnalyzer.Nodes;

public class WriteNode : StatemantNode
{
    public VarNode Vars { get; }

	public WriteNode(VarNode vars)
	{
		Vars = vars;
	}
}