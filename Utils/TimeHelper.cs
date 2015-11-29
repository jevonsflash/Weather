using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Utils
{
    public class TimeHelper
    {
        public static DateTime GetNow()
        {
            return DateTime.Now;
        }

        public static int GetTimeSection(DateTime dt, int seed)
        {
            double result = (dt.Hour + 1) / 24.0;
            int section = (int)Math.Ceiling(result * seed);
            return section;
        }
        public static int GetNowSection()
        {
            int setion = GetTimeSection(TimeHelper.GetNow(), 6);
            return setion;
        }
    }
}
