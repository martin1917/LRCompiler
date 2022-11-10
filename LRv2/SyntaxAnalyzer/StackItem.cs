namespace LRv2.SyntaxAnalyzer;

public class StackItem
{
    public string Value { get; }

    public string? Payload { get; }

    public int StateNumber { get; }

    public TreeNode? TreeNode { get; }

    public StackItem(int stateNumber, string value) 
        : this(stateNumber, value, null, null) { }
    
    public StackItem(int stateNumber, string value, string payload) 
        : this(stateNumber, value, payload, null) { }

    public StackItem(int stateNumber, string value, TreeNode treeNode) 
        : this(stateNumber, value, null, treeNode) { }

    public StackItem(int stateNumber, string value, string? payload, TreeNode? treeNode)
    {
        StateNumber = stateNumber;
        Value = value;
        Payload = payload;
        TreeNode = treeNode;
    }
}
