using LRv2.Extentions;
using LRv2.Parser;
using OfficeOpenXml;
using System.Data;

namespace LRv2
{
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

            Run(fsm, lexems);
        }

        private static void Run(LRTable fsm, List<Lexem> lexems)
        {
            bool accept = false;
            int i = 0;

            var stack = new Stack<int>();
            stack.Push(0);

            while (!accept)
            {
                var stateOnTopStack = stack.Peek();
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
                            stack.Push(nextStateNumber);
                            i++;
                        }
                        break;

                    case KindOperation.REDUCE:
                        {
                            var rule = Rule.AllRules.First(r => r.NumberRule == parserOperation.Number);
                            stack.PopCount(rule.Right.Length);
                            var stateAferReducing = stack.Peek();
                            var operation = fsm.Get(stateAferReducing, rule.Left);
                            var nextStateNumber = operation.Number;
                            stack.Push(nextStateNumber);
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
}