namespace LRv2.SyntaxAnalyzer.Nodes;

public class ReadNode : StatemantNode
{
    public VarNode Vars { get; }

	public ReadNode(VarNode vars)
	{
		Vars = vars;
	}
}
