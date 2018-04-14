using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelModel;

namespace LogicModel.Excel
{
    public class ExcelHelper
    {
        public ExcelHelper(string excelPath)
        {
            keyMap = new Dictionary<string, CustomCell>();
            excel = new ExcelSimpleMgr(0, excelPath);
            keyMap.Add("场景类目", new CustomCell(1, 1));
            keyMap.Add("场景名称", new CustomCell(1, 2));
            keyMap.Add("场景标签1", new CustomCell(1, 3));
            keyMap.Add("场景标签2", new CustomCell(1, 4));
            keyMap.Add("场景标签3", new CustomCell(1, 5));
            keyMap.Add("商品类目", new CustomCell(1, 6));
            keyMap.Add("商品名称", new CustomCell(1, 7));
            keyMap.Add("商品标签1", new CustomCell(1, 8));
            keyMap.Add("商品标签2", new CustomCell(1, 9));
            keyMap.Add("商品标签3", new CustomCell(1, 10));
            keyMap.Add("商品标签4", new CustomCell(1, 11));
            keyMap.Add("商品标签5", new CustomCell(1, 12));
            keyMap.Add("商品标签6", new CustomCell(1, 13));
            keyMap.Add("商品金额", new CustomCell(1, 14));
            keyMap.Add("商品使用数量", new CustomCell(1, 15));
            keyMap.Add("商品说明", new CustomCell(1, 16));

            WriteKey("场景类目", "场景类目");
            WriteKey("场景名称", "场景名称");
            WriteKey("场景标签1", "场景标签1");
            WriteKey("场景标签2", "场景标签2");
            WriteKey("场景标签3", "场景标签3");
            WriteKey("商品类目", "商品类目");
            WriteKey("商品名称", "商品名称");
            WriteKey("商品标签1", "商品标签1");
            WriteKey("商品标签2", "商品标签2");
            WriteKey("商品标签3", "商品标签3");
            WriteKey("商品标签4", "商品标签4");
            WriteKey("商品标签5", "商品标签5");
            WriteKey("商品标签6", "商品标签6");
            WriteKey("商品金额", "商品金额");
            WriteKey("商品使用数量", "商品使用数量");
            WriteKey("商品说明", "商品说明");
        }

        private ExcelSimpleMgr excel;
        private Dictionary<string, CustomCell> keyMap;

        

        public void WriteKey(string key, string customValue)
        {
            if (keyMap.ContainsKey(key))
            {
                CustomCell cell = keyMap[key];
                excel.SetCellValue(cell, customValue);
                cell.Row++;
            }
        }

        public void QuitExcel()
        {
            excel.Dispose();
            keyMap.Clear();
        }
        
    }
}
