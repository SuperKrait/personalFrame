using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModel.NetMgr
{
    public static class TimeExtensions
    {
        private static long javaOriTime = (new DateTime(1990, 1, 1)).Ticks;
        public static long GetJavaTime(this Int64 timeTicks)
        {
            return timeTicks - javaOriTime;
        }
    }
}
