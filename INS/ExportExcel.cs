namespace INS
{
    using Excel = Microsoft.Office.Interop.Excel;
    internal static class ExportExcel
    {
        public static Excel.Worksheet GetExcel(Excel.Workbook workbook, string workSheetName, string[] sheetHeader, double[,] data, int row, int col)
        {
            Excel.Worksheet worksheet = workbook.Worksheets.Add();
            worksheet.Name = workSheetName;
            worksheet.Cells.Columns.AutoFit();
            var startCell = worksheet.Cells[2, 1];
            var endCell = worksheet.Cells[row + 1, col];
            worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, col]].Value2 = sheetHeader;
            worksheet.Range[startCell, endCell].Value2 = data;
            return worksheet;
        }

        public static void AddExcel(Excel.Worksheet worksheet, Excel.Range startCell, string[] sheetHeader, double[,] data, int row, int col)
        {
            worksheet.Range[worksheet.Cells[1, startCell.Column], worksheet.Cells[1, startCell.Column + col - 1]].Value2 = sheetHeader;
            var endCell = worksheet.Cells[startCell.Row + row - 1, startCell.Column + col - 1];
            worksheet.Range[startCell, endCell].Value2 = data;
        }
    }
}
