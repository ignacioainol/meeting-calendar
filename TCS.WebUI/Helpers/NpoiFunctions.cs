using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCS.WebUI.Helpers
{
    public static class NpoiFunctions
    {
        public static void NewCell(IRow row, ICellStyle cellStyle, int columna, string valor)
        {
            ICell cell = row.CreateCell(columna);
            cell.SetCellValue(valor);
            cell.CellStyle = cellStyle;
        }

        public static void NewCell(IRow row, ICellStyle cellStyle, int columna, int valor)
        {
            ICell cell = row.CreateCell(columna);
            cell.SetCellValue(valor);
            cell.CellStyle = cellStyle;
        }

        public static void NewCell(IRow row, ICellStyle cellStyle, int columna, double valor)
        {
            ICell cell = row.CreateCell(columna);
            cell.SetCellValue(valor);
            cell.CellStyle = cellStyle;
        }

        public static void SetColumn(ISheet sheet, int col, int ancho)
        {
            sheet.SetColumnWidth(col, ancho * 256);
        }

        public static void MergeCells(ISheet sheet, HSSFWorkbook excelWorkbook, int firtsRow, int lastRow, int firstCol, int lastCol)
        {
            NPOI.SS.Util.CellRangeAddress cra = new NPOI.SS.Util.CellRangeAddress(firtsRow, lastRow, firstCol, lastCol);
            sheet.AddMergedRegion(cra);
            NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);
        }

        public static ICellStyle EstiloColumna(HSSFWorkbook excelWorkbook)
        {
            IFont font = excelWorkbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = excelWorkbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.SetFont(font);

            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderBottom = BorderStyle.Thin;

            return cellStyle;
        }

        public static ICellStyle EstiloHeader(HSSFWorkbook excelWorkbook)
        {
            IFont font = excelWorkbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = excelWorkbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.SetFont(font);

            cellStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            cellStyle.WrapText = true;
            cellStyle.FillPattern = FillPattern.SolidForeground;

            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderBottom = BorderStyle.Thin;

            return cellStyle;
        }

        public static ICellStyle EstiloFila(HSSFWorkbook excelWorkbook)
        {
            IFont font = excelWorkbook.CreateFont();

            ICellStyle cellStyle = excelWorkbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.SetFont(font);

            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderBottom = BorderStyle.Thin;

            return cellStyle;
        }

        public static ICellStyle EstiloFilaNumeric(HSSFWorkbook excelWorkbook)
        {
            IFont font = excelWorkbook.CreateFont();

            ICellStyle cellStyle = excelWorkbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Right;
            cellStyle.SetFont(font);

            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderBottom = BorderStyle.Thin;

            return cellStyle;
        }

        public static ICellStyle EstiloHora(HSSFWorkbook excelWorkbook)
        {
            IFont font = excelWorkbook.CreateFont();

            ICellStyle cellStyle = excelWorkbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.SetFont(font);

            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderBottom = BorderStyle.Thin;

            cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("[h]:mm:ss");

            return cellStyle;
        }

        public static ICellStyle EstiloAzul(HSSFWorkbook excelWorkbook)
        {
            IFont font = excelWorkbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = excelWorkbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.SetFont(font);

            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderBottom = BorderStyle.Thin;

            cellStyle.FillForegroundColor = HSSFColor.Blue.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;

            return cellStyle;
        }

        public static ICellStyle EstiloRojo(HSSFWorkbook excelWorkbook)
        {
            IFont font = excelWorkbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;
            font.Color = IndexedColors.White.Index;

            ICellStyle cellStyle = excelWorkbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.SetFont(font);

            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderBottom = BorderStyle.Thin;

            cellStyle.FillForegroundColor = HSSFColor.Red.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;

            return cellStyle;
        }

        public static ICellStyle EstiloVerde(HSSFWorkbook excelWorkbook)
        {
            IFont font = excelWorkbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;
            font.Color = IndexedColors.White.Index;

            ICellStyle cellStyle = excelWorkbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.SetFont(font);

            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderBottom = BorderStyle.Thin;

            cellStyle.FillForegroundColor = HSSFColor.LightGreen.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;

            return cellStyle;
        }

        public static ICellStyle EstiloAmarillo(HSSFWorkbook excelWorkbook)
        {
            IFont font = excelWorkbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold;
            font.Color = IndexedColors.White.Index;

            ICellStyle cellStyle = excelWorkbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.SetFont(font);

            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderBottom = BorderStyle.Thin;

            cellStyle.FillForegroundColor = HSSFColor.Yellow.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;

            return cellStyle;
        }

        public static void CreateE<T>(HSSFWorkbook excelWorkbook, string title, string sheetTitle, List<T> any)
        {
            ICellStyle estiloHeader = EstiloHeader(excelWorkbook);
            ICellStyle estiloFila = EstiloFila(excelWorkbook);
            ICellStyle estiloFilaNumeric = EstiloFilaNumeric(excelWorkbook);

            ISheet sheet = excelWorkbook.CreateSheet(sheetTitle);
            int num = 0;
            IRow row = sheet.CreateRow(num);
            ICell cell = row.CreateCell(0);

            Type type = typeof(T);
            int NumberOfProperties = type.GetProperties().Length;

            cell.SetCellValue(title.ToUpper());
            cell.CellStyle = estiloHeader;
            cell.Row.Height = 800;
            MergeCells(sheet, excelWorkbook, num, num, 0, NumberOfProperties-1);
            num++;

            row = sheet.CreateRow(num);
            row.Height = 400;

            for (int i = 0; i < NumberOfProperties; i++)
            {
                var property = type.GetProperties()[i];
                string propertyname = property.Name;
                NewCell(row, estiloHeader, i, propertyname.ToUpper());
            }
            num++;

            foreach (var item in any)
            {
                row = sheet.CreateRow(num);
                for (int i = 0; i < NumberOfProperties; i++)
                {
                    var property = type.GetProperties()[i];
                    string propertyname = property.Name;
                    object value = (property.GetValue(item) != null) ? property.GetValue(item) : "";

                    if (value.GetType() == typeof(string) && (value.ToString().Count() > 0 ? value.ToString().Last() != '%' : true))
                    {
                        NewCell(row, estiloFila, i, value.ToString());
                    }
                    else
                    {
                        NewCell(row, estiloFilaNumeric, i, value.ToString());
                    }

                    int colWidth = 0;
                    if (NumberOfProperties > colWidth)
                    {
                        colWidth = NumberOfProperties;
                    }

                    if (value.ToString().Length > colWidth)
                    {
                        colWidth = value.ToString().Length;
                    }

                    SetColumn(sheet, i, (int)Math.Log(colWidth * 0.5, 1.065));
                }
                row.Height = 300;
                num++;
            }

            if (!any.Any())
            {
                for (int i = 0; i < NumberOfProperties; i++)
                {
                    int colWidth = NumberOfProperties;
                    SetColumn(sheet, i, (int)Math.Log(colWidth * 0.5, 1.065));
                }
            }
            //var test = type.GetProperties()[7];
            //object list = test.GetValue(any[0]);
            //var test2 = any.FirstOrDefault();
        }

        public static void CreateE<T>(HSSFWorkbook excelWorkbook, string title, string sheetTitle, List<T> any, string[] customColumnNames)
        {
            ICellStyle estiloHeader = EstiloHeader(excelWorkbook);
            ICellStyle estiloFila = EstiloFila(excelWorkbook);
            ICellStyle estiloFilaNumeric = EstiloFilaNumeric(excelWorkbook);

            ISheet sheet = excelWorkbook.CreateSheet(sheetTitle);
            int num = 0;
            IRow row = sheet.CreateRow(num);
            ICell cell = row.CreateCell(0);

            Type type = typeof(T);
            int NumberOfProperties = type.GetProperties().Length;

            cell.SetCellValue(title.ToUpper());
            cell.CellStyle = estiloHeader;
            cell.Row.Height = 800;
            MergeCells(sheet, excelWorkbook, num, num, 0, NumberOfProperties - 1);
            num++;

            row = sheet.CreateRow(num);
            row.Height = 400;

            for (int i = 0; i < customColumnNames.Length; i++)
            {
                string propertyname = customColumnNames[i];
                NewCell(row, estiloHeader, i, propertyname.ToUpper());
            }
            num++;

            foreach (var item in any)
            {
                row = sheet.CreateRow(num);
                for (int i = 0; i < NumberOfProperties; i++)
                {
                    var property = type.GetProperties()[i];
                    string propertyname = property.Name;
                    object value = (property.GetValue(item) != null)? property.GetValue(item) : "";

                    if ((value.ToString().Count() > 0 ? value.ToString().Last() == '%' : false))
                    {
                        NewCell(row, estiloFilaNumeric, i, value.ToString());
                    }
                    else if (value.GetType() == typeof(string))
                    {
                        NewCell(row, estiloFila, i, value.ToString());
                    }
                    else
                    {
                        NewCell(row, estiloFilaNumeric, i, Convert.ToDouble(value));
                    }

                    int colWidth = 0;
                    if (customColumnNames[i].Length > colWidth)
                    {
                        colWidth = customColumnNames[i].Length;
                    }

                    if(value.ToString().Length > colWidth)
                    {
                        colWidth = value.ToString().Length;
                    }

                    SetColumn(sheet, i, (int)Math.Log(colWidth*0.5,1.065));
                }
                row.Height = 300;
                num++;
            }

            if(!any.Any())
            {
                for (int i = 0; i < customColumnNames.Length; i++)
                {
                    int colWidth = customColumnNames[i].Length;
                    SetColumn(sheet, i, (int)Math.Log(colWidth * 0.5, 1.065));
                }
            }
        }
    }
}
