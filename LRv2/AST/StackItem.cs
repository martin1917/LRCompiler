namespace LRv2.AST;

public class StackItem
{
    public string Symbol { get; }

    public string? Value { get; }

    public int StateNumber { get; }

    public TreeNode? TreeNode { get; }

    public StackItem(int stateNumber, string symbol, TreeNode? treeNode = null) : this(stateNumber, symbol, null, treeNode)
    {
    }

    public StackItem(int stateNumber, string symbol, string? value, TreeNode? treeNode = null)
    {
        StateNumber = stateNumber;
        Symbol = symbol;
        Value = value;
        TreeNode = treeNode;
    }
}
