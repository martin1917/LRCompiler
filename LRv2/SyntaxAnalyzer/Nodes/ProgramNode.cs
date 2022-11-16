namespace LRv2.SyntaxAnalyzer.Nodes;

public class ProgramNode : BaseNode
{
    public VarNode Vars { get; set; }

    public StatementNode Body { get; set; }
}
