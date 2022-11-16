using LRv2.AST;
using LRv2.LexicalAnalyzer;
using LRv2.SyntaxAnalyzer;

namespace LRv2;

public class Program
{
    static string[] examples =
    {
        @"
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
        ",

        @"
            VAR
                x, y, z, d, w: LOGICAL;

            BEGIN
                READ(x, y, z, d, w);

                resone = x OR y AND z;
                restwo = (x OR y) AND z;

                resthree = x OR y AND z OR w;
                resfour = (x OR y) AND (z OR w);

                resfive = NOT x OR y EQU z OR w;
            END
        ",

        @"
            VAR
	            x, y, z, w: LOGICAL;
	
            BEGIN
	            READ(x, y, z, w);
	
	            IF (NOT (x OR y) AND z) THEN
	            BEGIN
		            WRITE(z, w);
		            z = NOT ((x EQU z) AND w);
	            END
	            ELSE
	            BEGIN
		            x = w OR z AND x;
		            w = x EQU x AND (NOT x);
		            READ(z, x);
		
		            IF (z EQU x) THEN
		            BEGIN
			            READ(w);
			            WRITE(w);
		            END
		            ELSE
		            BEGIN
			            w = z OR x AND z AND x;
			            WRITE(z,w);
			            READ(w);
		            END
	            END
            END
        "
    };

    public static void Main(string[] args)
    {
        Test(1);
    }

    private static void Test(int index)
    {
        var code = examples[index];

        var lexer = new Lexer(code);
        var lexems = lexer.GetLexems();

        var table = LRTableGenerator.Generate();
        var parser = new Parser(table);

        var cst = parser.Parse(lexems);
        var builder = new TreeBuilder(cst);
        var ast = builder.BuildAST(); 
        Utils.SaveInJson(ast, "C:\\Users\\marti\\OneDrive\\Desktop\\tree.json");
    }
}