namespace LRv2;

public class TypeTerminal
{
    public static TypeTerminal[] AllTypes =
    {
        new TypeTerminal("var", "VAR"),
        new TypeTerminal("ident", "[a-z]{1,11}"),
        new TypeTerminal("const", "[01]"),
        new TypeTerminal("comma", ","),
        new TypeTerminal("colon", ":"),
        new TypeTerminal("logical", "LOGICAL"),
        new TypeTerminal("semicolon", ";"),
        new TypeTerminal("begin", "BEGIN"),
        new TypeTerminal("end", "END"),
        new TypeTerminal("space", "[ \\n\\t\\r]"),
        new TypeTerminal("assign", "="),
        new TypeTerminal("lparam", "\\("),
        new TypeTerminal("rparam", "\\)"),
        new TypeTerminal("if", "IF"),
        new TypeTerminal("then", "THEN"),
        new TypeTerminal("else", "ELSE"),
        new TypeTerminal("read", "READ"),
        new TypeTerminal("write", "WRITE"),
        new TypeTerminal("not", "NOT"),
        new TypeTerminal("and", "AND"),
        new TypeTerminal("or", "OR"),
        new TypeTerminal("equ", "EQU"),
        new TypeTerminal("eof", "\\$")
    };

    public static readonly TypeTerminal Var = AllTypes[0];
    public static readonly TypeTerminal Ident = AllTypes[1];
    public static readonly TypeTerminal Const = AllTypes[2];
    public static readonly TypeTerminal Comma = AllTypes[3];
    public static readonly TypeTerminal Colon = AllTypes[4];
    public static readonly TypeTerminal Logical = AllTypes[5];
    public static readonly TypeTerminal Semicolon = AllTypes[6];
    public static readonly TypeTerminal Begin = AllTypes[7];
    public static readonly TypeTerminal End = AllTypes[8];
    public static readonly TypeTerminal Space = AllTypes[9];
    public static readonly TypeTerminal Assign = AllTypes[10];
    public static readonly TypeTerminal Lparam = AllTypes[11];
    public static readonly TypeTerminal Rparam = AllTypes[12];
    public static readonly TypeTerminal If = AllTypes[13];
    public static readonly TypeTerminal Then = AllTypes[14];
    public static readonly TypeTerminal Else = AllTypes[15];
    public static readonly TypeTerminal Read = AllTypes[16];
    public static readonly TypeTerminal Write = AllTypes[17];
    public static readonly TypeTerminal Not = AllTypes[18];
    public static readonly TypeTerminal And = AllTypes[19];
    public static readonly TypeTerminal Or = AllTypes[20];
    public static readonly TypeTerminal Equ = AllTypes[21];
    public static readonly TypeTerminal Eof = AllTypes[22];

    public string Name { get; }

    public string Regex { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TypeTerminal typeTerminal) return false;
        return Name.Equals(typeTerminal.Name);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }

    public override string? ToString()
    {
        return Name;
    }

    private TypeTerminal(string nameTerminal, string regex)
    {
        Name = nameTerminal;
        Regex = regex;
    }
}
