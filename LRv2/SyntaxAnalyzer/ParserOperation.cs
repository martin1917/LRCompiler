namespace LRv2.SyntaxAnalyzer;

public struct ParserOperation : IEquatable<ParserOperation>
{
    public ParserTypeOperation TypeOperation { get; }

    public int Number { get; }

    private ParserOperation(ParserTypeOperation typeOperation, int number = -1)
    {
        TypeOperation = typeOperation;
        Number = number;
    }

    public static ParserOperation NewShift(int nextState)
        => new(ParserTypeOperation.SHIFT, nextState);

    public static ParserOperation NewReduce(int numberRule)
        => new(ParserTypeOperation.REDUCE, numberRule);

    public static ParserOperation NewAccept()
        => new(ParserTypeOperation.ACCEPT);

    public static ParserOperation NewError()
        => new(ParserTypeOperation.ERROR);

    public static bool operator ==(ParserOperation op1, ParserOperation op2)
    {
        return op1.Equals(op2);
    }

    public static bool operator !=(ParserOperation op1, ParserOperation op2)
    {
        return !op1.Equals(op2);
    }

    public bool Equals(ParserOperation other)
    {
        return TypeOperation == other.TypeOperation
            && Number == other.Number;
    }

    public override bool Equals(object? obj)
    {
        var IsEqual = false;

        if (obj is ParserOperation)
        {
            IsEqual = Equals((ParserOperation)obj);
        }

        return IsEqual;
    }

    public override int GetHashCode()
    {
        return TypeOperation.GetHashCode() ^ Number.GetHashCode();
    }

    public override string? ToString()
    {
        return TypeOperation switch
        {
            ParserTypeOperation.SHIFT => $"SHIFT {Number}",
            ParserTypeOperation.REDUCE => $"REDUCE {Number}",
            ParserTypeOperation.ACCEPT => $"ACCEPT",
            _ => string.Empty
        };
    }
}