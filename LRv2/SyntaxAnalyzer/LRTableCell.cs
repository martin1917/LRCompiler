namespace LRv2.SyntaxAnalyzer;

public class LRTableCell
{
    public int StateNumber { get; }

    public string InputingLexem { get; }

    public ParserOperation ParserOperation { get; }

    public LRTableCell(int stateNumber, string inputingLexem, ParserOperation parserOperation)
    {
        StateNumber = stateNumber;
        InputingLexem = inputingLexem;
        ParserOperation = parserOperation;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not LRTableCell item) return false;

        return StateNumber == item.StateNumber
            && InputingLexem == item.InputingLexem;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StateNumber, InputingLexem, ParserOperation);
    }

    public override string? ToString()
    {
        return ParserOperation.TypeOperation switch
        {
            ParserTypeOperation.SHIFT => $"[{StateNumber}, {InputingLexem}] = SHIFT {ParserOperation.Number}",
            ParserTypeOperation.REDUCE => $"[{StateNumber}, {InputingLexem}] = REDUCE {ParserOperation.Number}",
            ParserTypeOperation.ACCEPT => $"[{StateNumber}, {InputingLexem}] = ACCEPT",
            _ => string.Empty
        };
    }
}
