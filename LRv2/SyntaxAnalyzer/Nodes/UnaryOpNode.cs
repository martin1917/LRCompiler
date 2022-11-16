namespace LRv2.SyntaxAnalyzer.Nodes;

public class UnaryOpNode : StatemantNode
{
    public string Operation { get; set; }

    public BaseNode Node { get; set; }
}
