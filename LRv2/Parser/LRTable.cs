namespace LRv2.Parser;

public class LRTable
{
    public HashSet<LRTableItems> Items { get; } = new();

    public ParserOperation Get(int stateNumber, string inputingLexem)
    {
        var row = Items.FirstOrDefault(r => r.StateNumber == stateNumber && r.InputingLexem.Equals(inputingLexem));
        return row != null 
            ? row.ParserOperation 
            : ParserOperation.NewError();
    }

    public bool Add(int stateNumber, string inputingLexem, ParserOperation parserOperation)
        => Items.Add(new LRTableItems(stateNumber, inputingLexem, parserOperation));

    public IEnumerable<int> GetUniqueNumberStates()
        => Items.Select(r => r.StateNumber).Distinct();

    public IEnumerable<string> GetUniqueLexems()
        => Items.Select(r => r.InputingLexem).Distinct();
}