namespace LRv2.AST;

public class TreeNode
{
    public string Value { get; }

    public List<TreeNode> Childs { get; }

    public TreeNode(string value) : this(value, new List<TreeNode>()) { }

    public TreeNode(string value, List<TreeNode> childs)
    {
        Value = value;
        Childs = childs;
    }

    public void Add(TreeNode node)
    {
        Childs.Add(node);
    }

    public override string? ToString()
    {
        return Value;
    }
}