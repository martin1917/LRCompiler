namespace LRv2;

public class Lexem
{
    public TypeTerminal TypeTerminal { get; } = null!;
    
    public string Value { get; } = null!;

    public int Pos { get; }

    public Lexem(string value, TypeTerminal typeTerminal, int pos)
    {
        Value = value;
        TypeTerminal = typeTerminal;
        Pos = pos;
    }

    public string Type => TypeTerminal.Name;

    public bool IsIdentOrConst()
        => TypeTerminal.Name == TypeTerminal.Ident.Name
        || TypeTerminal.Name == TypeTerminal.Const.Name;

    public static Lexem CreateEndLexem()
        => new("$", TypeTerminal.Eof, -1);

    public override bool Equals(object? obj)
    {
        if (obj is not Lexem lexem) return false;
        return Value.Equals(lexem.Value) && TypeTerminal.Equals(lexem.TypeTerminal);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value, TypeTerminal);
    }

    public override string? ToString()
    {
        return $"(type: {TypeTerminal}\tvalue: {Value})";
    }
}
