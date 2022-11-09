using LRv2.Extentions;

namespace LRv2.Parser;

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

        var rules = Rule.AllRules.Where(r => r.Left.StartsWith(lexem));

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
        var rules = Rule.AllRules.Where(r => r.Right.Contains(lexem));

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

    public static void Closure(State state)
    {
        for (int i = 0; i < state.Situations.Count; i++)
        {
            var situation = state.Situations[i];

            if (situation.DotPlaceAtEnd())
                continue;

            var lexem = situation.GetLexemAfterDot();

            if (!lexem.IsNotTerminal())
                continue;

            var rules = Rule.AllRules.Where(r => r.Left.StartsWith(lexem));
            foreach (var rule in rules)
            {
                if (situation.AfterDotOnlyOneLexem())
                {
                    state.AddSituation(new Situation(rule, 0, situation.Lookahead));
                }
                else
                {
                    var firstLexems = FirstLexemsFor(situation.Rule.Right[situation.Pos + 1]);
                    foreach (var first in firstLexems)
                    {
                        state.AddSituation(new Situation(rule, 0, first));
                    }
                }
            }
        }
    }

    public static State Goto(State state, string lexem)
    {
        List<Situation> newSituations = new();
        foreach (var situation in state.Situations)
        {
            if (situation.DotPlaceAtEnd())
                continue;

            var lexemAfterDot = situation.GetLexemAfterDot();
            if (lexemAfterDot.Equals(lexem))
            {
                newSituations.Add(new Situation(situation.Rule, situation.Pos + 1, situation.Lookahead));
            }
        }

        newSituations = newSituations.Distinct().ToList();
        var newState = new State(newSituations);
        Closure(newState);
        return newState;
    }

    public static LRTable BuildLRTable()
    {
        LRTable table = new();

        var initSituation = new Situation(Rule.InitRule, 0, "eof");
        var initState = new State(new List<Situation>() { initSituation });
        Closure(initState);

        var states = new List<State>() { initState };
        for (int i = 0; i < states.Count; i++)
        {
            var state = states[i];
            AddReducing(state, table);

            var LexemsAfterDot = state.GetLexemsAfterDot();
            foreach (var lexem in LexemsAfterDot)
            {
                State newState = Goto(state, lexem);
                int pos = states.IndexOf(newState);
                int nextStateNumber;

                if (pos == -1)
                {
                    states.Add(newState);
                    nextStateNumber = newState.NumberState;
                }
                else
                {
                    nextStateNumber = states[pos].NumberState;
                }

                var successAdding = table.Add(state.NumberState, lexem, ParserOperation.NewShift(nextStateNumber));
                if (!successAdding)
                {
                    var item = table.Items.First(item => item.StateNumber == state.NumberState && item.InputingLexem == lexem);
                    var message = $"[ERROR] - CONFLICT\n" +
                        $"БЫЛО: {item}\n" +
                        $"Пытаемся вставить 'SHIFT {nextStateNumber}'";

                    throw new Exception(message);
                }
            }
        }

        return table;
    }

    private static void AddReducing(State state, LRTable table)
    {
        foreach (var situation in state.Situations)
        {
            if (!situation.DotPlaceAtEnd())
                continue;

            if (situation.Rule.Left.StartsWith("<program>"))
            {
                table.Add(state.NumberState, "eof", ParserOperation.NewAccept());
            }
            else
            {
                var successAdding = table.Add(state.NumberState, situation.Lookahead, ParserOperation.NewReduce(situation.Rule.NumberRule));
                if (!successAdding)
                {
                    var item = table.Items.First(item => item.StateNumber == state.NumberState && item.InputingLexem == situation.Lookahead);
                    var message = $"ERROR - CONFLICT\n" +
                        $"БЫЛО: {item}\n" +
                        $"Пытаемся вставить 'REDUCE {situation.Rule.NumberRule}'";
                    throw new Exception(message);
                }
            }
        }
    }
}
