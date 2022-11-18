namespace LRv2.SyntaxAnalyzer.Nodes;

public class ProgramNode : BaseNode
{
    public VarNode Vars { get; }

    public ScopeNode Body { get; }

    public ProgramNode(VarNode vars, ScopeNode body)
    {
        Vars = vars;
        Body = body;
    }
}
