using System.Reflection;
using System.Xml.Linq;

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

    public static readonly TypeTerminal Var       = new(name: "var",     regex: @"VAR");
    public static readonly TypeTerminal Ident     = new(name: "ident",   regex: @"\b[a-z]{1,11}\b");
    public static readonly TypeTerminal Const     = new(name: "const",   regex: @"[01]");
    public static readonly TypeTerminal Comma     = new(name: ",",       regex: @",");
    public static readonly TypeTerminal Colon     = new(name: ":",       regex: @":");
    public static readonly TypeTerminal Logical   = new(name: "logical", regex: @"LOGICAL");
    public static readonly TypeTerminal Semicolon = new(name: ";",       regex: @";");
    public static readonly TypeTerminal Begin     = new(name: "begin",   regex: @"BEGIN");
    public static readonly TypeTerminal End       = new(name: "end",     regex: @"\bEND\b");
    public static readonly TypeTerminal Space     = new(name: "space",   regex: @"[ \n\t\r]");
    public static readonly TypeTerminal Assign    = new(name: "=",       regex: @"=");
    public static readonly TypeTerminal Lparam    = new(name: "(",       regex: @"\(");
    public static readonly TypeTerminal Rparam    = new(name: ")",       regex: @"\)");
    public static readonly TypeTerminal If        = new(name: "if",      regex: @"IF");
    public static readonly TypeTerminal EndIf     = new(name: "end_if",  regex: @"END_IF");
    public static readonly TypeTerminal Then      = new(name: "then",    regex: @"THEN");
    public static readonly TypeTerminal Else      = new(name: "else",    regex: @"ELSE");
    public static readonly TypeTerminal Read      = new(name: "read",    regex: @"READ");
    public static readonly TypeTerminal Write     = new(name: "write",   regex: @"WRITE");
    public static readonly TypeTerminal Not       = new(name: "not",     regex: @"NOT");
    public static readonly TypeTerminal And       = new(name: "and",     regex: @"AND");
    public static readonly TypeTerminal Or        = new(name: "or",      regex: @"OR");
    public static readonly TypeTerminal Equ       = new(name: "equ",     regex: @"EQU");
    public static readonly TypeTerminal Eof       = new(name: "eof",     regex: @"\$");

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

    private TypeTerminal(string name, string regex)
    {
        Name = name;
        Regex = regex;
    }
}
