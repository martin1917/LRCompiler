namespace LRv2.SyntaxAnalyzer.Nodes;

public class UnaryOpNode : BaseNode
{
    public string Operation { get; set; }

    public BaseNode Node { get; set; }
}
