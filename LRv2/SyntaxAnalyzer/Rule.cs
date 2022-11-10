namespace LRv2.SyntaxAnalyzer;

public class Rule
{
    private static int count = 0;

    public int NumberRule { get; }

    public string Left { get; } = null!;

    public string[] Right { get; } = null!;

    public Rule(string left, string[] right)
    {
        NumberRule = count++;
        Left = left;
        Right = right;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Rule rule) return false;

        if (Right.Length != rule.Right.Length) return false;
        for (int i = 0; i < Right.Length; i++)
        {
            if (Right[i] != rule.Right[i]) return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Left, Right);
    }

    public override string? ToString()
    {
        return $"{Left} ::= {string.Join(' ', Right)}";
    }
}