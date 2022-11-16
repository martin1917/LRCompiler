namespace LRv2.AST;

public static class Consts
{
    private static readonly List<string> keyWords = new()
    {
        "not",
        "or",
        "and",
        "equ",
        "begin",
        "end",
        "var",
        "if",
        "else",
        "logical",
    };

    private static readonly List<string> binOps = new()
    {
        "or",
        "and",
        "equ",
    };

    private static readonly List<string> unOps = new()
    {
        "not",
    };

    public static bool IsKeyWord(string str) => keyWords.Contains(str);

    public static bool IsBinOp(string str) => binOps.Contains(str);

    public static bool IsUnOp(string str) => unOps.Contains(str);
}