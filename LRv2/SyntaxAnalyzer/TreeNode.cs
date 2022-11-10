namespace LRv2.SyntaxAnalyzer;

public class TreeNode
{
    public string Value { get; }

    public List<TreeNode>? Childs { get; } = null;

    public TreeNode(string value) : this(value, null) { }

    public TreeNode(string value, List<TreeNode>? childs)
    {
        Value = value;
        Childs = childs;
    }

    public override string? ToString()
    {
        return Value;
    }
}