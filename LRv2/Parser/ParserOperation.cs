using System.Drawing;

namespace LRv2.Parser;

public struct ParserOperation : IEquatable<ParserOperation>
{
    public KindOperation KindOperation { get; }

    public int Number { get; }

    private ParserOperation(KindOperation kindOperation, int number = -1)
    {
        KindOperation = kindOperation;
        Number = number;
    }

    public static ParserOperation NewShift(int nextState)
        => new(KindOperation.SHIFT, nextState);

    public static ParserOperation NewReduce(int numberRule)
        => new(KindOperation.REDUCE, numberRule);

    public static ParserOperation NewAccept()
        => new(KindOperation.ACCEPT);

    public static ParserOperation NewError()
        => new(KindOperation.ERROR);

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
        return KindOperation == other.KindOperation
            && Number == other.Number;
    }

    public override bool Equals(object? obj)
    {
        var IsEqual = false;

        if (obj is Point)
        {
            IsEqual = Equals((Point)obj);
        }

        return IsEqual;
    }

    public override int GetHashCode()
    {
        return KindOperation.GetHashCode() ^ Number.GetHashCode();
    }

    public override string? ToString()
    {
        return KindOperation switch
        {
            KindOperation.SHIFT => $"SHIFT {Number}",
            KindOperation.REDUCE => $"REDUCE {Number}",
            KindOperation.ACCEPT => $"ACCEPT",
            _ => string.Empty
        };
    }
}