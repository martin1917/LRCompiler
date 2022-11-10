using LRv2.LexicalAnalyzer;
using LRv2.SyntaxAnalyzer;

namespace LRv2;

public class Program
{
    public static void Main(string[] args)
    {
        Test();
    }

    private static void Test()
    {
        var code = @"
            VAR 
                x, y: LOGICAL;
            BEGIN
                IF (x EQU 1) THEN
                BEGIN
                    y = 0;
                    x = NOT (y AND (NOT (x OR y)));
                END
                ELSE
                BEGIN
                    READ(a, b, c);
                    IF ((b EQU c) AND a) THEN
                    BEGIN
                        a = 1;
                        x = NOT (c);
                    END
                    ELSE
                    BEGIN
                        c = 1;
                    END
                    WRITE(x);
                END
            END
            ";

        var lexer = new Lexer(code);
        var lexems = lexer.GetLexems();

        var table = LRTableGenerator.Generate();
        var parser = new Parser(table);

        var ast = parser.Parse(lexems);
        Console.WriteLine();
    }
}