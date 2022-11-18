using LRv2.AST;
using LRv2.LexicalAnalyzer;
using LRv2.SyntaxAnalyzer;

namespace LRv2;

public class Program
{
    public static void Main(string[] args)
    {
        var code = Utils.GetAllTextFromFile("program.txt");
        Test(code);
    }

    private static void Test(string code)
    {
        // лексер
        var lexer = new Lexer(code);

        // лексемы, полученные с помощью лексера
        List<Lexem> lexems;
        try
        {
            lexems = lexer.GetLexems();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        // таблица переходов для LR(1) анализатора
        LRTable table;
        try
        {
            table = LRTableGenerator.Generate();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        Utils.SaveTableInExcel(table, "LRTable");
        
        // парсер
        var parser = new Parser(table);

        // Преобразование коллекцим лексем в древовидную структуру,
        // согласно правилам грамматики и таблице переходов
        TreeNode cst;
        try
        {
            cst = parser.Parse(lexems);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        Utils.SaveInJson(cst, nameof(cst));

        // Преобразователь конкретного дерева в абстрактное
        var builder = new TreeBuilder(cst);

        // абстрактно синтаксическое дерево (AST)
        var ast = builder.BuildAST();

        Utils.SaveInJson(ast, nameof(ast));

        // Обработчик AST
        var worker = new ASTWorker(ast);
        try
        {
            worker.Proccess();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}