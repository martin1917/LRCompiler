namespace LRv2.SyntaxAnalyzer;

public static class Rules
{
    private static readonly Rule[] all =
    {
        new Rule("<S>", new string[]{ "<program>" }),

        new Rule("<program>", new string[]{ "<variable_declaration>", "<description_calculations>" }),

        new Rule("<description_calculations>", new string[]{ "begin", "<list_actions>", "end" }),

        new Rule("<list_actions>", new string[]{ "<list_assignments>" }),
        new Rule("<list_actions>", new string[]{ "<list_assignments>", "<list_actions>" }),
        new Rule("<list_actions>", new string[]{ "<list_operators>" }),
        new Rule("<list_actions>", new string[]{ "<list_operators>", "<list_actions>" }),

        new Rule("<variable_declaration>", new string[]{ "var", "<list_variables>", ":",  "logical", ";" }),

        new Rule("<list_variables>", new string[]{ "ident" }),
        new Rule("<list_variables>", new string[]{ "ident", ",", "<list_variables>" }),

        new Rule("<list_operators>", new string[]{ "<operator>" }),
        new Rule("<list_operators>", new string[]{ "<operator>", "<list_operators>" }),

        new Rule("<operator>", new string[]{ "read", "(", "<list_variables>", ")", ";" }),
        new Rule("<operator>", new string[]{ "write", "(", "<list_variables>", ")", ";" }),
        new Rule("<operator>", new string[]{ "if", "(", "<expr>", ")", "then", "<description_calculations>", "else", "<description_calculations>" }),

        new Rule("<list_assignments>", new string[]{ "<assignment>" }),
        new Rule("<list_assignments>", new string[]{ "<assignment>", "<list_assignments>" }),

        new Rule("<assignment>", new string[]{ "ident", "=", "<expr>", ";" }),

        new Rule("<expr>", new string[]{ "<unary_op>", "<sub_expr>" }),
        new Rule("<expr>", new string[]{ "<sub_expr>" }),

        new Rule("<sub_expr>", new string[]{ "(", "<expr>", ")" }),
        new Rule("<sub_expr>", new string[]{ "<operand>" }),
        new Rule("<sub_expr>", new string[]{ "<sub_expr>", "<bin_op>", "<sub_expr>" }),

        new Rule("<unary_op>", new string[]{ "not" }),

        new Rule("<bin_op>", new string[]{ "and" }),
        new Rule("<bin_op>", new string[]{ "or" }),
        new Rule("<bin_op>", new string[]{ "equ" }),

        new Rule("<operand>", new string[]{ "ident" }),
        new Rule("<operand>", new string[]{ "const" }),
    };

    public static readonly Rule Init = all.First();

    public static IEnumerable<Rule> GetStartWith(string start)
        => all.Where(rule => rule.Left.Equals(start));

    public static IEnumerable<Rule> GetRightPartContains(string lexem)
        => all.Where(rule => rule.Right.Contains(lexem));

    public static Rule GetByNumber(int number)
        => all.First(rule => rule.NumberRule == number);
}