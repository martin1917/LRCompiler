namespace LRv2.AST;

public class StackItem
{
    public string Value { get; }

    public string? Payload { get; }

    public int StateNumber { get; }

    public TreeNode? TreeNode { get; }

    public StackItem(int stateNumber, string value, TreeNode? treeNode = null) : this(stateNumber, value, null, treeNode)
    {
    }

    public StackItem(int stateNumber, string value, string? payload, TreeNode? treeNode = null)
    {
        StateNumber = stateNumber;
        Value = value;
        Payload = payload;
        TreeNode = treeNode;
    }
}
