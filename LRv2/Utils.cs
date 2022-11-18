using LRv2.SyntaxAnalyzer;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Data;

namespace LRv2;

public static class Utils
{
    private const string PATH = "../../../../Etc";
    private const string JSON_EXT = "json";
    private const string EXCEL_EXT = "xlsx";

    public static string GetAllTextFromFile(string fileName)
    {
        using var reader = new StreamReader($"{PATH}/{fileName}");
        var code = reader.ReadToEnd();
        return code;
    }

    public static void SaveInJson<T>(T obj, string fileName) where T : class
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };

        string json = JsonConvert.SerializeObject(obj, settings);
        using var writer = new StreamWriter($"{PATH}/{fileName}.{JSON_EXT}", false);
        writer.Write(json);
    }

    public static void SaveTableInExcel(LRTable table, string fileName)
    {
        var uniqueNumberStates = table.GetUniqueNumberStates();
        var uniqueLexems = table.GetUniqueLexems();

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
                var pareserOperation = table.Get(numberState, lexem);
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

        File.WriteAllBytes($"{PATH}/{fileName}.{EXCEL_EXT}", package.GetAsByteArray());
    }
}
