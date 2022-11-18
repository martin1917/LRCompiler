namespace LRv2.SyntaxAnalyzer.Nodes;

public class IfElseNode : StatemantNode
{
    public BaseNode Predicate { get; }

    public ScopeNode TrueBranch { get; }

    public ScopeNode FalseBranch { get; }

    public IfElseNode(BaseNode predicate, ScopeNode trueBranch, ScopeNode falseBranch)
    {
        Predicate = predicate;
        TrueBranch = trueBranch;
        FalseBranch = falseBranch;
    }
}
