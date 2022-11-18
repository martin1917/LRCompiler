namespace LRv2.SyntaxAnalyzer.Nodes;

public class UnaryOpNode : StatemantNode
{
    public string Operation { get; }

    public BaseNode Node { get; }

    public UnaryOpNode(string operation, BaseNode node)
    {
        Operation = operation;
        Node = node;
    }
}
