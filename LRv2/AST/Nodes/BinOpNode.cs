namespace LRv2.SyntaxAnalyzer.Nodes;

public class BinOpNode : StatemantNode
{
    public string Operation { get; }

    public BaseNode Left { get; }

    public BaseNode Right { get; }

    public BinOpNode(string operation, BaseNode left, BaseNode right)
    {
        Operation = operation;
        Left = left;
        Right = right;
    }
}
