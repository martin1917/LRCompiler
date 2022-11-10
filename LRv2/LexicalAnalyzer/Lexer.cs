using System.Text.RegularExpressions;

namespace LRv2.LexicalAnalyzer;

public class Lexer
{
    private string text;
    private int pos;
    private List<Lexem> lexems = new();

    public Lexer(string text)
    {
        this.text = text;
    }

    public List<Lexem> GetLexems()
    {
        while (Next()) { }
        lexems.Add(Lexem.CreateEndLexem());
        return lexems;
    }

    public bool Next()
    {
        if (pos >= text.Length)
        {
            return false;
        }

        foreach (var type in TypeTerminal.AllTypes)
        {
            var regex = new Regex($"^{type.Regex}");
            var matches = regex.Matches(text[pos..]);
            if (matches.Count > 0)
            {
                var enumerator = matches.GetEnumerator();
                enumerator.MoveNext();
                var item = enumerator.Current.ToString();
                if (type != TypeTerminal.Space)
                {
                    lexems.Add(new Lexem(item, type, pos));
                }
                pos += item.Length;
                return true;
            }
        }

        throw new Exception($"Error on {pos}");
    }
}