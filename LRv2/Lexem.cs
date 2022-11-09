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

    public bool IsVariableOrConst()
        => TypeTerminal.Name == TypeTerminal.Variable.Name
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
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Value.GetHashCode();
            hash = hash * 23 + TypeTerminal.GetHashCode();
            return hash;
        }
    }

    public override string? ToString()
    {
        return $"(type: {TypeTerminal}\tvalue: {Value})";
    }
}
