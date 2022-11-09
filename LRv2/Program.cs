using LRv2.AST;
using LRv2.Extentions;
using LRv2.Parser;
using OfficeOpenXml;
using System.Data;

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
                x: LOGICAL;

            BEGIN
                x = 1;
            END
            ";

        var lexer = new Lexer(code);
        var lexems = lexer.GetLexems();
        var fsm = ParserHelpers.BuildLRTable();

        Run(fsm, lexems);
    }

    private static void Run(LRTable fsm, List<Lexem> lexems)
    {
        bool accept = false;
        int i = 0;

        var stack = new Stack<StackItem>();
        stack.Push(new StackItem("", 0));

        while (!accept)
        {
            var stateOnTopStack = stack.Peek().StateNumber;
            var parserOperation = fsm.Get(stateOnTopStack, lexems[i].TypeTerminal.Name);

            if (parserOperation.KindOperation is KindOperation.ERROR)
            {
                Console.WriteLine("Ошибка во время парсинга!!!\n");
                break;
            }

            switch (parserOperation.KindOperation)
            {
                case KindOperation.ACCEPT:
                    { 
                        accept = true;                            
                    }
                    break;

                case KindOperation.SHIFT:
                    {
                        var nextStateNumber = parserOperation.Number;
                        stack.Push(new StackItem(lexems[i].TypeTerminal.Name, nextStateNumber));
                        i++;
                    }
                    break;

                case KindOperation.REDUCE:
                    {
                        var rule = Rule.AllRules.First(r => r.NumberRule == parserOperation.Number);

                        var childs = new List<TreeNode?>();
                        for(int k = 0; k < rule.Right.Length; k++)
                        {
                            var item = stack.Pop();
                            childs.Insert(0, item.TreeNode == null ? new TreeNode(item.Value) : item.TreeNode);
                        }

                        var stateAferReducing = stack.Peek().StateNumber;
                        var operation = fsm.Get(stateAferReducing, rule.Left);
                        var nextStateNumber = operation.Number;
                        stack.Push(new StackItem(rule.Left, nextStateNumber, new TreeNode(rule.Left, childs)));
                    }
                    break;
            }
        }


        Console.WriteLine(accept ? "ХОРОШО\n" : "ПЛОХО\n");
    }

    private static void SaveInExcel(LRTable fsm)
    {
        var uniqueNumberStates = fsm.GetUniqueNumberStates();
        var uniqueLexems = fsm.GetUniqueLexems();

        DataTable dataTable = new();
        dataTable.Columns.Add("State", typeof(int));
        foreach (var colName in uniqueLexems)
        {
            dataTable.Columns.Add($"{colName}", typeof(string));
        }

        foreach (var numberState in uniqueNumberStates)
        {
            var row = dataTable.NewRow();
            row["State"] = numberState;

            foreach (var lexem in uniqueLexems)
            {
                var pareserOperation = fsm.Get(numberState, lexem);
                row[lexem] = pareserOperation.ToString();
            }
            dataTable.Rows.Add(row);
        }

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("FSM");

        var initRow = 2;
        var initCol = 5;

        for (int i = 0; i < dataTable.Columns.Count; i++)
        {
            var col = dataTable.Columns[i];
            sheet.Cells[initRow, initCol + i].Value = col.ColumnName;
        }

        for (int i = 0; i < dataTable.Rows.Count; i++)
        {
            var row = dataTable.Rows[i];
            int column = initCol;

            for (int j = 0; j < row.ItemArray.Length; j++)
            {
                var value = row.ItemArray[j];
                sheet.Cells[initRow + i + 1, column + j].Value = value;
            }
        }

        var endRow = initRow + dataTable.Rows.Count;
        var endCol = initCol + dataTable.Columns.Count;
        sheet.Cells[initRow, initCol, endRow, endCol].AutoFitColumns();

        File.WriteAllBytes("C:\\Users\\marti\\OneDrive\\Desktop\\fsm.xlsx", package.GetAsByteArray());
    }
}

public class StackItem
{
    public string Value { get; }

    public TreeNode? TreeNode { get; } = null;

    public int StateNumber { get; }

    public StackItem(string value, int stateNumber, TreeNode? treeNode = null)
    {
        Value = value;
        StateNumber = stateNumber;
        TreeNode = treeNode;
    }
}