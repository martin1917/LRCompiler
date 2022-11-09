namespace LRv2.Extentions;

public static class StringExtention
{
    public static bool IsNotTerminal(this string str)
    {
        return str.StartsWith('<') && str.EndsWith('>');
    }
}
