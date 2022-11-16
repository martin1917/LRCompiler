namespace LRv2.SyntaxAnalyzer.Nodes;

public class StatementNode : BaseNode
{
    public List<BaseNode> Code { get; set; } = new();
}
