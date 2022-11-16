namespace LRv2.SyntaxAnalyzer.Nodes;

public abstract class BaseNode
{
    public string Type => GetType().Name;
}

public abstract class StatemantNode : BaseNode { }