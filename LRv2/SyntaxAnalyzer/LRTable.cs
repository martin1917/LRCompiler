namespace LRv2.SyntaxAnalyzer;

public class LRTable
{
    private HashSet<LRTableCell> cells { get; } = new();

    public IEnumerable<LRTableCell> Cells => cells;

    public ParserOperation Get(int stateNumber, string inputingLexem)
    {
        var row = cells.FirstOrDefault(r => r.StateNumber == stateNumber && r.InputingLexem.Equals(inputingLexem));
        return row != null 
            ? row.ParserOperation 
            : ParserOperation.NewError();
    }

    public bool Add(int stateNumber, string inputingLexem, ParserOperation parserOperation)
        => cells.Add(new LRTableCell(stateNumber, inputingLexem, parserOperation));

    public IEnumerable<int> GetUniqueNumberStates()
        => cells.Select(r => r.StateNumber).Distinct();

    public IEnumerable<string> GetUniqueLexems()
        => cells.Select(r => r.InputingLexem).Distinct();
}