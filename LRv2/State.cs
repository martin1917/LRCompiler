namespace LRv2;

public class State
{
    private static int count = 0;

    public int NumberState { get; }

    public List<Situation> Situations { get; set; }

    public State(List<Situation> situations)
    {
        NumberState = count++;
        Situations = situations;
    }

    public void AddSituation(Situation situation)
    {
        if (!Situations.Contains(situation))
        {
            Situations.Add(situation);
        }
    }

    public IEnumerable<string> GetLexemsAfterDot()
        => Situations
            .Where(s => !s.DotPlaceAtEnd())
            .Select(s => s.GetLexemAfterDot()).Distinct();

    public override bool Equals(object? obj)
    {
        if (obj is not State state) return false;

        return state.Situations.Count == Situations.Count 
            && Situations.All(x => state.Situations.Contains(x));
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Situations.GetHashCode();
            return hash;
        }
    }

    public override string? ToString()
    {
        return $"Num: {NumberState}\n" +
            $"{string.Join('\n', Situations)}";
    }
}
