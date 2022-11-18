namespace LRv2.SyntaxAnalyzer.Nodes;

public class VarNode : BaseNode
{
    public List<VariableNode> Variables { get; }

	public VarNode(List<VariableNode> variables)
	{
		Variables = variables;
	}
}
