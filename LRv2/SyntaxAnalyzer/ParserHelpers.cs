using LRv2.Extentions;

namespace LRv2.SyntaxAnalyzer;

public static class ParserHelpers
{
    public static HashSet<string> FirstLexemsFor(string lexem)
    {
        HashSet<string> result = new();

        if (!lexem.IsNotTerminal())
        {
            result.Add(lexem);
            return result;
        }

        var rules = Rules.GetStartWith(lexem);

        foreach (var rule in rules)
        {
            var rightFirst = rule.Right[0];

            if (rightFirst.Equals(lexem)) continue;

            result.AddRange(FirstLexemsFor(rightFirst));
        }

        return result;
    }

    public static HashSet<string> FollowLexemsFor(string lexem)
    {
        var rules = Rules.GetRightPartContains(lexem);

        HashSet<string> result = new();

        foreach (var rule in rules)
        {
            var indexes = rule.Right.IndexesOf(lexem);
            foreach (var index in indexes)
            {
                if (index == rule.Right.Length - 1)
                {
                    if (rule.Left.StartsWith(rule.Right[index])) continue;
                    result.AddRange(FollowLexemsFor(rule.Left));
                }
                else
                {
                    result.AddRange(FirstLexemsFor(rule.Right[index + 1]));
                }
            }
        }

        return result;
    }
}
