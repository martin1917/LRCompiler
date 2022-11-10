using LRv2.Extentions;

namespace LRv2.SyntaxAnalyzer;

public static class LRTableGenerator
{
    public static LRTable Generate()
    {
        LRTable table = new();

        var initSituation = new Situation(Rules.Init, 0, "eof");
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
                    var item = table.Cells.First(item => item.StateNumber == state.NumberState && item.InputingLexem == lexem);
                    var message = $"[ERROR] - CONFLICT\n" +
                        $"БЫЛО: {item}\n" +
                        $"Пытаемся вставить 'SHIFT {nextStateNumber}'";

                    throw new Exception(message);
                }
            }
        }

        return table;
    }

    private static State Goto(State state, string lexem)
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

    private static void Closure(State state)
    {
        for (int i = 0; i < state.Situations.Count; i++)
        {
            var situation = state.Situations[i];

            if (situation.DotPlaceAtEnd())
                continue;

            var lexem = situation.GetLexemAfterDot();

            if (!lexem.IsNotTerminal())
                continue;

            var rules = Rules.GetStartWith(lexem);
            foreach (var rule in rules)
            {
                if (situation.AfterDotOnlyOneLexem())
                {
                    state.AddSituation(new Situation(rule, 0, situation.Lookahead));
                }
                else
                {
                    var firstLexems = ParserHelpers.FirstLexemsFor(situation.Rule.Right[situation.Pos + 1]);
                    foreach (var first in firstLexems)
                    {
                        state.AddSituation(new Situation(rule, 0, first));
                    }
                }
            }
        }
    }

    private static void AddReducing(State state, LRTable table)
    {
        foreach (var situation in state.Situations)
        {
            if (!situation.DotPlaceAtEnd())
                continue;

            if (situation.Rule.Left.StartsWith("<S>"))
            {
                table.Add(state.NumberState, "eof", ParserOperation.NewAccept());
            }
            else
            {
                var successAdding = table.Add(state.NumberState, situation.Lookahead, ParserOperation.NewReduce(situation.Rule.NumberRule));
                if (!successAdding)
                {
                    var item = table.Cells.First(item => item.StateNumber == state.NumberState && item.InputingLexem == situation.Lookahead);
                    var message = $"ERROR - CONFLICT\n" +
                        $"БЫЛО: {item}\n" +
                        $"Пытаемся вставить 'REDUCE {situation.Rule.NumberRule}'";
                    throw new Exception(message);
                }
            }
        }
    }
}
