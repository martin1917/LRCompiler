namespace LRv2.SyntaxAnalyzer.Nodes;

public class ScopeNode : BaseNode
{
    public List<StatemantNode> Statements { get; }

	public ScopeNode(List<StatemantNode> statements)
	{
		Statements = statements;
	}
}
