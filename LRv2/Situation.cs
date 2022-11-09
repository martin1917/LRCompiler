namespace LRv2;

public class Situation
{
    public Rule Rule { get; }

    public int Pos { get; }

    public string Lookahead { get; }

    public Situation(Rule rule, int pos, string lookahead)
    {
        Rule = rule;
        Pos = pos;
        Lookahead = lookahead;
    }

    public string GetLexemAfterDot() => Rule.Right[Pos];

    public bool DotPlaceAtEnd() => Pos == Rule.Right.Length;

    public bool AfterDotOnlyOneLexem() => Pos == Rule.Right.Length - 1;

    public override bool Equals(object? obj)
    {
        if (obj is not Situation situation) return false;

        return Rule.Equals(situation.Rule) 
            && Pos == situation.Pos 
            && Lookahead == situation.Lookahead;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Rule.GetHashCode();
            hash = hash * 23 + Lookahead.GetHashCode();
            hash = hash * 23 + Pos.GetHashCode();
            return hash;
        }
    }

    public override string? ToString()
    {
        int offset = 0;
        int border = Math.Min(Pos, Rule.Right.Length);
        for (int i = 0; i < border; i++)
        {
            offset += Rule.Right[i].Length + 1;
        }

        string rightPart = string.Join(' ', Rule.Right);
        offset = Math.Min(offset, rightPart.Length);
        rightPart = rightPart.Insert(offset, "•");
        return $"[{Rule.Left} ::= {rightPart}\t|{Lookahead}]";
    }
}
