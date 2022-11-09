namespace LRv2.Parser;

public class LRTableItems
{
    public int StateNumber { get; }

    public string InputingLexem { get; }

    public ParserOperation ParserOperation { get; }

    public LRTableItems(int stateNumber, string inputingLexem, ParserOperation parserOperation)
    {
        StateNumber = stateNumber;
        InputingLexem = inputingLexem;
        ParserOperation = parserOperation;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not LRTableItems item) return false;

        return StateNumber == item.StateNumber
            && InputingLexem == item.InputingLexem
            && ParserOperation == item.ParserOperation;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + StateNumber.GetHashCode();
            hash = hash * 23 + InputingLexem.GetHashCode();
            hash = hash * 23 + ParserOperation.GetHashCode();
            return hash;
        }
    }

    public override string? ToString()
    {
        return ParserOperation.KindOperation switch
        {
            KindOperation.SHIFT => $"[{StateNumber}, {InputingLexem}] = SHIFT {ParserOperation.Number}",
            KindOperation.REDUCE => $"[{StateNumber}, {InputingLexem}] = REDUCE {ParserOperation.Number}",
            KindOperation.ACCEPT => $"[{StateNumber}, {InputingLexem}] = ACCEPT",
            _ => string.Empty
        };
    }
}
