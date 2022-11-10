namespace LRv2;

public class Rule
{
    public static Rule[] AllRules =
    {
        new Rule("<S>", new string[]{ "<program>" }),

        new Rule("<program>", new string[]{ "<variable_declaration>", "<description_calculations>" }),

        new Rule("<description_calculations>", new string[]{ "begin", "<list_actions>", "end" }),

        new Rule("<list_actions>", new string[]{ "<list_assignments>" }),
        new Rule("<list_actions>", new string[]{ "<list_assignments>", "<list_actions>" }),
        new Rule("<list_actions>", new string[]{ "<list_operators>" }),
        new Rule("<list_actions>", new string[]{ "<list_operators>", "<list_actions>" }),

        new Rule("<variable_declaration>", new string[]{ "var", "<list_variables>", "colon",  "logical", "semicolon" }),

        new Rule("<list_variables>", new string[]{ "ident" }),
        new Rule("<list_variables>", new string[]{ "ident", "comma", "<list_variables>" }),

        new Rule("<list_operators>", new string[]{ "<operator>" }),
        new Rule("<list_operators>", new string[]{ "<operator>", "<list_operators>" }),

        new Rule("<operator>", new string[]{ "read", "lparam", "<list_variables>", "rparam", "semicolon" }),
        new Rule("<operator>", new string[]{ "write", "lparam", "<list_variables>", "rparam", "semicolon" }),
        new Rule("<operator>", new string[]{ "if", "lparam", "<expr>", "rparam", "then", "<description_calculations>", "else", "<description_calculations>" }),

        new Rule("<list_assignments>", new string[]{ "<assignment>" }),
        new Rule("<list_assignments>", new string[]{ "<assignment>", "<list_assignments>" }),

        new Rule("<assignment>", new string[]{ "ident", "assign", "<expr>", "semicolon" }),

        new Rule("<expr>", new string[]{ "<unary_op>", "<sub_expr>" }),
        new Rule("<expr>", new string[]{ "<sub_expr>" }),

        new Rule("<sub_expr>", new string[]{ "lparam", "<expr>", "rparam" }),
        new Rule("<sub_expr>", new string[]{ "<operand>" }),
        new Rule("<sub_expr>", new string[]{ "<sub_expr>", "<bin_op>", "<sub_expr>" }),

        new Rule("<unary_op>", new string[]{ "not" }),

        new Rule("<bin_op>", new string[]{ "and" }),
        new Rule("<bin_op>", new string[]{ "or" }),
        new Rule("<bin_op>", new string[]{ "equ" }),

        new Rule("<operand>", new string[]{ "ident" }),
        new Rule("<operand>", new string[]{ "const" }),
    };

    public static readonly Rule InitRule = AllRules[0];

    private static int count = 0;

    public int NumberRule { get; }

    public string Left { get; } = null!;

    public string[] Right { get; } = null!;

    private Rule(string left, string[] right)
    {
        NumberRule = count++;
        Left = left;
        Right = right;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Rule rule) return false;

        if (Right.Length != rule.Right.Length) return false;
        for(int i = 0; i < Right.Length; i++)
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
