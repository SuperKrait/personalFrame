using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelModel
{
    /// <summary>
    /// 用户自定义的单元位置信息
    /// </summary>
    public class CustomCell
    {
        public CustomCell(int r, int c)
        {
            Row = r;
            Column = c;
        }
        private int row;
        private int column;
        public string content => Row + "," + Column + "\n";

        public int Row
        {
            get
            {
                return row;
            }

            set
            {
                row = value;
            }
        }

        public int Column
        {
            get
            {
                return column;
            }

            set
            {
                column = value;
            }
        }
    }
}
