namespace LRv2.SyntaxAnalyzer.Nodes;

public class IfElseNode : StatemantNode
{
    public BaseNode Predicate { get; set; }

    public ScopeNode TrueBranch { get; set; }

    public ScopeNode FalseBranch { get; set; }
}
