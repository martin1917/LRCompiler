namespace LRv2.SyntaxAnalyzer.Nodes;

public class IfElseNode : BaseNode
{
    public BaseNode Predicate { get; set; }

    public StatementNode TrueBranch { get; set; }

    public StatementNode FalseBranch { get; set; }
}
