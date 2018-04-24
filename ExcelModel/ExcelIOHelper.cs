using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Reflection;

namespace ExcelModel
{
    /// <summary>
    /// 对Excel的IO接口
    /// </summary>
    internal static class ExcelIOHelper
    {        
        /// <summary>
        /// 创建excel
        /// </summary>
        /// <returns></returns>
        public static _Workbook CreateExcel(out Application app, out Workbooks wbks)
        {
            app = new Application();
            wbks = app.Workbooks;
            _Workbook _wbk = wbks.Add(true);
            return _wbk;
        }

        /// <summary>
        /// 打开excel
        /// </summary>
        /// <param name="excelPath"></param>
        /// <returns></returns>
        public static _Workbook OpenExcel(out Application app, string excelPath, out Workbooks wbks)
        {
            app = new Application();
            wbks = app.Workbooks;
            _Workbook _wbk = wbks.Add(excelPath);
            return _wbk;
        }
        /// <summary>
        /// 根据索引id获取Sheets
        /// </summary>
        /// <param name="_wbk"></param>
        /// <param name="index">索引起始从1开始</param>
        /// <returns></returns>
        public static _Worksheet GetSheetByIndex(_Workbook _wbk, int index, out Sheets shs)
        {
            shs = _wbk.Sheets;
            _Worksheet _wsh = (_Worksheet)shs.get_Item(index);
            return _wsh;
        }

        /// <summary>
        /// 删除Sheet
        /// </summary>
        /// <param name="app"></param>
        /// <param name="_wsh"></param>
        public static void DeleteSheet(Application app, Sheets _wsh)
        {
            app.DisplayAlerts = false;
            _wsh.Delete();
            app.DisplayAlerts = true;
        }

        /// <summary>
        /// 在_wsh之前插入表格
        /// </summary>
        /// <param name="app"></param>
        public static void AddSheet(Application app, _Worksheet _wsh, int count = 1,  XlSheetType type = XlSheetType.xlWorksheet)
        {
            //插入Sheet//a(before)，b(after)：确定添加位置；c：数目；d：类型
            app.Worksheets.Add(_wsh, Missing.Value, count, type);
        }
        /// <summary>
        /// 重命名Sheet
        /// </summary>
        /// <param name="_wsh"></param>
        /// <param name="sheetName"></param>
        public static void RenameSheet(_Worksheet _wsh, string sheetName)
        {
            _wsh.Name = sheetName;
        }
        /// <summary>
        /// 插入行，向该行上边插入
        /// </summary>
        /// <param name="_wsh"></param>
        /// <param name="rowIndex">该行上部插入列</param>
        public static void AddRow(_Worksheet _wsh, int rowIndex)
        {
            ((Range)_wsh.Rows[rowIndex, Missing.Value]).Insert(Missing.Value, XlInsertFormatOrigin.xlFormatFromLeftOrAbove);
        }

        /// <summary>
        /// 隐藏行
        /// </summary>
        /// <param name="_wsh"></param>
        /// <param name="rowIndex"></param>
        /// <param name="isHide"></param>
        public static void HideRow(_Worksheet _wsh, int rowIndex, bool isHide)
        {
            ((Range)_wsh.Rows[rowIndex, Missing.Value]).EntireRow.Hidden = isHide;
        }

        /// <summary>
        /// 添加列，向列所在右边插入
        /// </summary>
        /// <param name="_wsh"></param>
        /// <param name="columnIndex">在该列右边插入</param>
        public static void AddColumn(_Worksheet _wsh, int columnIndex)
        {
            ((Range)_wsh.Rows[columnIndex, Missing.Value]).Insert(Missing.Value, XlInsertShiftDirection.xlShiftToRight);
        }

        /// <summary>
        /// 隐藏列
        /// </summary>
        /// <param name="_wsh"></param>
        /// <param name="columnIndex"></param>
        /// <param name="isHide"></param>
        public static void HideColumn(_Worksheet _wsh, int columnIndex, bool isHide)
        {
            ((Range)_wsh.Columns[Missing.Value, columnIndex]).EntireColumn.Hidden = isHide;
        }

        /// <summary>
        /// 删除行，然后下一行上移
        /// </summary>
        /// <param name="_wsh"></param>
        /// <param name="rowIndex"></param>
        public static void DeleteRow(_Worksheet _wsh, int rowIndex)
        {
            ((Range)_wsh.Rows[rowIndex, Missing.Value]).Delete(XlDeleteShiftDirection.xlShiftUp);
        }

        /// <summary>
        /// 删除列，然后右边列向左移动
        /// </summary>
        /// <param name="_wsh"></param>
        /// <param name="columnIndex"></param>
        public static void DeleteColumn(_Worksheet _wsh, int columnIndex)
        {
            ((Range)_wsh.Rows[Missing.Value, columnIndex]).Delete(XlDeleteShiftDirection.xlShiftToLeft);            
        }        

        /// <summary>
        /// 选中表格中区域
        /// </summary>
        /// <param name="_wsh"></param>
        /// <param name="cell1"></param>
        /// <param name="cell2"></param>
        /// <returns></returns>
        public static Range GetSheetRange(_Worksheet _wsh, CustomCell cell1, CustomCell cell2)
        {
            return _wsh.get_Range(_wsh.Cells[cell1.Row, cell1.Column],  _wsh.Cells[cell2.Row, cell2.Column]);
        }

        /// <summary>
        /// 获取单元格
        /// </summary>
        /// <param name="_wsh"></param>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static Range GetCell(_Worksheet _wsh, CustomCell cell)
        {
            return _wsh.Cells[cell.Row, cell.Column];
        }
        /// <summary>
        /// 设置单元格公式
        /// </summary>
        /// <param name="_wsh"></param>
        /// <param name="cell"></param>
        /// <param name="functionContent">公式 例如："=Sum(A1/B1)"</param>
        public static void SetCellFunction(_Worksheet _wsh, CustomCell cell, string functionContent)
        {
            _wsh.Cells[cell.Row, cell.Column] = functionContent;//"=Sum(A1/B1)";
        }

        /// <summary>
        /// 设置单元格公式
        /// </summary>
        /// <param name="_wsh"></param>
        /// <param name="cell"></param>
        /// <param name="cellFormat">公式 例如："@"代表文本格式,"，"G/通用格式"代表通用格式</param>
        public static void SetCellFormat(_Worksheet _wsh, CustomCell cell, string cellFormat)
        {
            _wsh.Cells[cell.Row, cell.Column].NumberFormatLocal = cellFormat;
        }
        /// <summary>
        /// 设置行高
        /// </summary>
        /// <param name="_wsh"></param>
        /// <param name="rowIndex"></param>
        /// <param name="height"></param>
        public static void SetRowHight(_Worksheet _wsh, int rowIndex, int height)
        {
            ((Range)_wsh.Rows[rowIndex, Missing.Value]).RowHeight = height;
        }
        /// <summary>
        /// 设置列宽
        /// </summary>
        /// <param name="_wsh"></param>
        /// <param name="columnIndex"></param>
        /// <param name="widtht"></param>
        public static void SetColumnWidtht(_Worksheet _wsh, int columnIndex, int widtht)
        {
            ((Range)_wsh.Columns[Missing.Value, columnIndex]).ColumnWidth = widtht;
        }

        /// <summary>
        /// 设定选定区域颜色，包括单独的单元格颜色
        /// </summary>
        /// <param name="range"></param>
        /// <param name="colorIndex"></param>
        public static void SetRangeColor(Range range, int colorIndex)
        {
            range.Interior.ColorIndex = colorIndex;
        }

        /// <summary>
        /// 获取选定区域单元格字体（用于集体设置）
        /// </summary>
        /// <param name="range"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Font GetRangeFont(Range range, int size)
        {
            return range.Font;
        }

        /// <summary>
        /// 设置选定区域对其方式，包括单独单元格
        /// </summary>
        /// <param name="range"></param>
        /// <param name="type">对齐方式</param>
        public static void SetRangeHorizontalAlignment(Range range, XlVAlign type = XlVAlign.xlVAlignCenter)
        {
            range.HorizontalAlignment = type;
        }

        /// <summary>
        /// 设置区域边框
        /// </summary>
        /// <param name="range"></param>
        /// <param name="lineStycle"></param>
        public static void SetRangeBordersLineStyle(Range range, int lineStycle = 3)
        {
            range.Borders.LineStyle = lineStycle;
        }

        /// <summary>
        /// 设置选定区域边框的上、下、左、右线条
        /// </summary>
        /// <param name="range"></param>
        /// <param name="position"></param>
        /// <param name="type"></param>
        public static void SetRangeBorderWeight(Range range, XlBordersIndex position, XlBorderWeight type)
        {
            range.Borders[position].Weight = type;
        }

        /// <summary>
        /// 选中区域复制到指定起始单元格
        /// </summary>
        /// <param name="range">选中区域</param>
        /// <param name="positionCell">指定起始单元格</param>
        /// <param name="app">如果是官方的复制方法，则为空</param>
        public static void CopyRangeToPosition(Range range, Range positionCell, Application app = null)
        {
            if (app == null)//官方的复制方法
            {
                range.Select();
                range.Copy(positionCell);
            }
            else//非官方复制方法
            {
                range.Select();
                range.Copy(positionCell);
                positionCell.Select();
                //屏蔽掉Alert，默认确定粘贴
                app.DisplayAlerts = false;
                positionCell.Parse(Missing.Value, Missing.Value);
                app.DisplayAlerts = true;
            }

        }

        /// <summary>
        /// 保存Excel
        /// 这个地方只能采用该方法保存，不然在指定路径下保存文件外，在我的文档中也会生成一个对应的副本
        /// </summary>
        /// <param name="app"></param>
        /// <param name="workBook"></param>
        /// <param name="filePath"></param>
        public static void SaveExcel(Application app, _Workbook workBook, string filePath)
        {
            //屏蔽掉系统跳出的Alert
            app.AlertBeforeOverwriting = false;

            //保存到指定目录
            workBook.SaveAs(filePath, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
        }

        /// <summary>
        /// 退出当前的Excel
        /// </summary>
        /// <param name="app"></param>
        public static void QuitExcel(Application app, Workbooks wbks, _Workbook _wbk)
        {
            _wbk.Close(null, null, null);
            wbks.Close();
            app.Quit();

            //释放掉多余的excel进程
            System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
            app = null;
        }

        /// <summary>
        /// 修改下拉单元格的值
        /// 这里的“1，2，3”设置的就是下拉框的值
        /// </summary>
        /// <param name="range"></param>
        /// <param name="Formula1"></param>
        /// <param name="dvType"></param>
        /// <param name="dvAlertStyle"></param>
        public static void ModifyRangeValidation(Range range, string Formula1, XlDVType dvType = XlDVType.xlValidateList, XlDVAlertStyle dvAlertStyle = XlDVAlertStyle.xlValidAlertStop)
        {
            range.Validation.Modify(dvType, dvAlertStyle, Type.Missing, Formula1, Type.Missing);
        }
        /// <summary>
        /// 添加下拉单元格的值
        /// 这里的“1，2，3”设置的就是下拉框的值
        /// </summary>
        /// <param name="range"></param>
        /// <param name="Formula1"></param>
        /// <param name="dvType"></param>
        /// <param name="dvAlertStyle"></param>
        public static void AddRangeValidation(Range range, string Formula1, XlDVType dvType = XlDVType.xlValidateList, XlDVAlertStyle dvAlertStyle = XlDVAlertStyle.xlValidAlertStop)
        {
            range.Validation.Add(dvType, dvAlertStyle, Type.Missing, Formula1, Type.Missing);
        }
        /// <summary>
        /// 获取下拉单元格的值
        /// </summary>
        /// <param name="Cell"></param>
        /// <returns></returns>
        public static string GetRangeValidationValue(Range Cell)
        {
            string strValue = Cell.Validation.Formula1;
            return strValue;
        }        
    }
    
}
