using System.Reflection;

namespace LRv2.LexicalAnalyzer;

public class TypeTerminal
{
    private static List<TypeTerminal> allTypes;

    public static List<TypeTerminal> AllTypes => allTypes
        ??= LoadAllTypes();

    private static List<TypeTerminal> LoadAllTypes()
    {
        var fields = typeof(TypeTerminal)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => f.GetValue(null) as TypeTerminal);

        return new List<TypeTerminal>(fields!);
    }

    public static readonly TypeTerminal Var       =  new("var", "VAR");
    public static readonly TypeTerminal Ident     =  new("ident", "[a-z]{1,11}");
    public static readonly TypeTerminal Const     =  new("const", "[01]");
    public static readonly TypeTerminal Comma     =  new(",", ",");
    public static readonly TypeTerminal Colon     =  new(":", ":");
    public static readonly TypeTerminal Logical   =  new("logical", "LOGICAL");
    public static readonly TypeTerminal Semicolon =  new(";", ";");
    public static readonly TypeTerminal Begin     =  new("begin", "BEGIN");
    public static readonly TypeTerminal End       =  new("end", "END");
    public static readonly TypeTerminal Space     =  new("space", "[ \\n\\t\\r]");
    public static readonly TypeTerminal Assign    =  new("=", "=");
    public static readonly TypeTerminal Lparam    =  new("(", "\\(");
    public static readonly TypeTerminal Rparam    =  new(")", "\\)");
    public static readonly TypeTerminal If        =  new("if", "IF");
    public static readonly TypeTerminal Then      =  new("then", "THEN");
    public static readonly TypeTerminal Else      =  new("else", "ELSE");
    public static readonly TypeTerminal Read      =  new("read", "READ");
    public static readonly TypeTerminal Write     =  new("write", "WRITE");
    public static readonly TypeTerminal Not       =  new("not", "NOT");
    public static readonly TypeTerminal And       =  new("and", "AND");
    public static readonly TypeTerminal Or        =  new("or", "OR");
    public static readonly TypeTerminal Equ       =  new("equ", "EQU");
    public static readonly TypeTerminal Eof       =  new("eof", "\\$");

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
