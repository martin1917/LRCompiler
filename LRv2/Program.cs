using LRv2.AST;
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
        var fsm = ParserHelpers.BuildLRTable();

        var ast = GenerateAST(fsm, lexems);
        Console.WriteLine();
    }

    private static TreeNode GenerateAST(LRTable fsm, List<Lexem> lexems)
    {
        bool accept = false;
        int i = 0;

        var stack = new Stack<StackItem>();
        stack.Push(new StackItem(0, ""));

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

                        StackItem stackItem = lexems[i].IsVariableOrConst()
                            ? new StackItem(nextStateNumber, lexems[i].TypeTerminal.Name, lexems[i].Value)
                            : new StackItem(nextStateNumber, lexems[i].TypeTerminal.Name);

                        stack.Push(stackItem);
                        i++;
                    }
                    break;

                case KindOperation.REDUCE:
                    {
                        var rule = Rule.AllRules.First(r => r.NumberRule == parserOperation.Number);

                        var childs = new List<TreeNode?>();
                        for(int k = 0; k < rule.Right.Length; k++)
                        {
                            StackItem item = stack.Pop();

                            TreeNode child = item.TreeNode 
                                ?? new TreeNode(
                                    value: item.Symbol, 
                                    childs: item.Value != null 
                                        ? new() { new TreeNode(item.Value) } 
                                        : new());

                            childs.Insert(0, child);
                        }

                        var stateAferReducing = stack.Peek().StateNumber;
                        var operation = fsm.Get(stateAferReducing, rule.Left);
                        var nextStateNumber = operation.Number;
                        stack.Push(new StackItem(nextStateNumber, rule.Left, new TreeNode(rule.Left, childs)));
                    }
                    break;
            }
        }

        if (!accept)
        {
            throw new Exception("Ошибка парсинга");
        }

        return stack.Pop().TreeNode!;
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
    public string Symbol { get; }

    public string? Value { get; }

    public int StateNumber { get; }

    public TreeNode? TreeNode { get; }

    public StackItem(int stateNumber, string symbol, TreeNode? treeNode = null) : this(stateNumber, symbol, null, treeNode)
    {
    }

    public StackItem(int stateNumber, string symbol, string? value, TreeNode? treeNode = null)
    {
        StateNumber = stateNumber;
        Symbol = symbol;
        Value = value;
        TreeNode = treeNode;
    }
}