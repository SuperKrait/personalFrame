using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ExcelModel
{
    /// <summary>
    /// 实际Excel操作类
    /// 所有的索引从1开始
    /// </summary>
    public class ExcelSimpleMgr : IDisposable
    {
        private Application app;
        private Workbooks wbks;
        private _Workbook _wbk;
        private _Worksheet workSheet;
        /// <summary>
        /// 当前正在处于处理中的Sheet
        /// </summary>
        public _Worksheet CurWorkSheet
        {
            get
            {
                return workSheet;
            }
        }
        /// <summary>
        /// 初始化Excel
        /// </summary>
        /// <param name="type">0为创建，1为打开</param>
        /// <param name="path">路径</param>
        public ExcelSimpleMgr(int type, string path = "")
        {
            if ((type == 0 || type == 1) && string.IsNullOrEmpty(path))
            {
                _wbk = ExcelIOHelper.CreateExcel(out app, out wbks);
                //默认位置为我的文档
                excelPath = GetFileName();
                //AddSheet();
            }
            else if (type == 1 && !string.IsNullOrEmpty(path))
            {
                _wbk = ExcelIOHelper.OpenExcel(out app, path, out wbks);
                excelPath = path;
            }
            else
            {
                _wbk = ExcelIOHelper.CreateExcel(out app, out wbks);
                //默认位置为我的文档
                excelPath = path;
            }
            SetWorkSheet(1);

        }

        private bool isSave = true;
        private bool isQuit = false;
        private string excelPath;
        private int curSheetIndex = 1;
        /// <summary>
        /// 获取新建文件路径，以及文件名
        /// </summary>
        /// <returns>新建文件路径</returns>
        private string GetFileName()
        {
            //获取我的文档路径
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //新建文件名
            string fileName = "新建Excel文件.xlsx";
            string  tmpPath = GetFileNameByName(path + "\\" + fileName, 0);
            return tmpPath;
        }
        /// <summary>
        /// 检查本地中是否有相同的路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetFileNameByName(string filePath, int index)
        {
            string extensionName = Path.GetExtension(filePath);
            string pathName = filePath.Substring(0, filePath.LastIndexOf('.'));//无后缀的路径
            string fullName = pathName + index + extensionName;
            if (File.Exists(fullName))
            {
                index++;
                return GetFileNameByName(filePath, index);
            }
            else
            {
                return fullName;
            }
        }



        /// <summary>
        /// 设置当前工作表
        /// </summary>
        /// <param name="index">数据索引从1开始</param>
        /// <returns></returns>
        private bool SetWorkSheet(int index)
        {
            Sheets shs;
            workSheet = ExcelIOHelper.GetSheetByIndex(_wbk, index, out shs);
            if (workSheet == null)
                return false;
            else
            {
                curSheetIndex = index;
                return true;
            }
        }

        /// <summary>
        /// 删除Sheet
        /// </summary>
        /// <param name="index">数据索引从1开始</param>
        public void DeleteSheet(int index)
        {
            Sheets shs;
            _Worksheet sheet = ExcelIOHelper.GetSheetByIndex(_wbk, index, out shs);
            ExcelIOHelper.DeleteSheet(app, shs);
        }

        /// <summary>
        /// 在某个索引位置插入表格
        /// </summary>
        /// <param name="count">插入数量，默认为1</param>
        /// <param name="insertIndex">插入表格位置,如果为-1的时候，则为当前sheet位置</param>
        public void AddSheet(int count = 1, int insertIndex = -1)
        {
            if (insertIndex == -1)
                insertIndex = curSheetIndex;
            Sheets shs;
            _Worksheet sheet = ExcelIOHelper.GetSheetByIndex(_wbk, insertIndex, out shs);
            ExcelIOHelper.AddSheet(app, sheet, count);
        }
        /// <summary>
        /// 在当前表格中添加行
        /// </summary>
        /// <param name="rowIndex">添加行的索引</param>
        public void AddRow(int rowIndex)
        {
            ExcelIOHelper.AddRow(workSheet, rowIndex);
        }
        /// <summary>
        /// 隐藏/显示行
        /// </summary>
        /// <param name="rowIndex">行id</param>
        /// <param name="activeState">传true为隐藏行,传False为显示行</param>
        public void SetRowActive(int rowIndex, bool activeState)
        {
            ExcelIOHelper.HideRow(workSheet, rowIndex, activeState);
        }
        /// <summary>
        /// 删除行
        /// </summary>
        /// <param name="rowIndex">行索引id</param>
        public void DeleteRow(int rowIndex)
        {
            ExcelIOHelper.DeleteRow(workSheet, rowIndex);
        }
        /// <summary>
        /// 删除列
        /// </summary>
        /// <param name="columnIndex"></param>
        public void AddColumn(int columnIndex)
        {
            ExcelIOHelper.AddColumn(workSheet, columnIndex);
        }
        /// <summary>
        /// 隐藏/显示列
        /// </summary>
        /// <param name="columnIndex">列Id</param>
        /// <param name="activeState">传true为隐藏列,传False为显示列</param>
        public void SetColumnActive(int columnIndex, bool activeState)
        {
            ExcelIOHelper.HideColumn(workSheet, columnIndex, activeState);
        }
        /// <summary>
        /// 删除列
        /// </summary>
        /// <param name="rowIndex">列索引id</param>
        public void DeleteColumn(int columnIndex)
        {
            ExcelIOHelper.DeleteColumn(workSheet, columnIndex);
        }
        /// <summary>
        /// 获取选中区域
        /// </summary>
        /// <param name="cell1"></param>
        /// <param name="cell2"></param>
        /// <returns></returns>
        public Range GetSheetRange(CustomCell cell1, CustomCell cell2)
        {
            return ExcelIOHelper.GetSheetRange(workSheet, cell1, cell2);
        }
        /// <summary>
        /// 获取单元格内容
        /// </summary>
        /// <param name="cell1"></param>
        /// <returns></returns>
        public string GetCellValue(CustomCell cell1)
        {
            Range cell = ExcelIOHelper.GetCell(workSheet, cell1);
            return cell.Value == null ? string.Empty : cell.Value.ToString();
        }
        /// <summary>
        /// 设置单元格内容
        /// </summary>
        /// <param name="cell1"></param>
        /// <param name="content"></param>
        public void SetCellValue(CustomCell cell1, string content)
        {
            Range cell = ExcelIOHelper.GetCell(workSheet, cell1);
            cell.Value = content;
        }
        /// <summary>
        /// 设置单元格内公式
        /// </summary>
        /// <param name="cell1"></param>
        /// <param name="functionContent"></param>
        public void SetCellFucntion(CustomCell cell1, string functionContent)
        {
            ExcelIOHelper.SetCellFunction(workSheet, cell1, functionContent);
        }

        /// <summary>
        /// 设置行高
        /// </summary>
        /// <param name="rowIndex">行id</param>
        /// <param name="height">高度</param>
        public void SetRowHight(int rowIndex, int height)
        {
            ExcelIOHelper.SetRowHight(workSheet, rowIndex, height);
        }

        /// <summary>
        /// 设置列宽
        /// </summary>
        /// <param name="columnIndex">列id</param>
        /// <param name="widtht">列宽</param>
        public void SetColumnWidtht(int columnIndex, int widtht)
        {
            ExcelIOHelper.SetColumnWidtht(workSheet, columnIndex, widtht);
        }


        /// <summary>
        /// 立即保存
        /// </summary>
        public void SaveImmadiatly()
        {
            isSave = true;
            ExcelIOHelper.SaveExcel(app, _wbk, excelPath);
        }

        /// <summary>
        /// 设置表的存储状态，设置为true为销毁时保存
        /// </summary>
        /// <param name="saveState"></param>
        public void SetSave(bool saveState)
        {
            isSave = saveState;
        }

        /// <summary>
        /// 立即退出excel，建议先调用立即保存后再调用该方法
        /// 不建议手动调用该方法，掉用过该方法后，请销毁整个对象
        /// </summary>
        public void QuitExcelImmadiatly()
        {
            isQuit = true;
            ExcelIOHelper.QuitExcel(app, wbks, _wbk);
        }


        public void Dispose()
        {
            if (!isQuit)
            {
                if (isSave)
                {
                    ExcelIOHelper.SaveExcel(app, _wbk, excelPath);
                }
                ExcelIOHelper.QuitExcel(app, wbks, _wbk);
            }
            app = null;
            wbks = null;
            _wbk = null;
        }
    }
}
