namespace LRv2.SyntaxAnalyzer.Nodes;

public class ScopeNode : BaseNode
{
    public List<StatemantNode> Statements { get; set; } = new();
}
