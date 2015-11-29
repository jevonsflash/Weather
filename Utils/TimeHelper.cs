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

        public static int GetTimeSectionByEqu(DateTime dt, int seed)
        {
            double result = (dt.Hour + 1) / 24.0;
            int section = (int)Math.Ceiling(result * seed);
            return section;
        }

        public static int GetTimeSectionByWeight(DateTime dt, int seed)
        {
            int section = 1;
            if (dt.Hour > 6 && dt.Hour < 22)
            {
                double result = (dt.Hour - 6) / 16.0;
                section = (int)Math.Ceiling(result * seed);
            }
            else
            {
                section = seed;
            }
            return section;
        }
        public static int GetNowSection()
        {
            int setion = GetTimeSectionByEqu(TimeHelper.GetNow(), 6);
            return setion;
        }
        public static int GetNowSectionByWeight()
        {
            int setion = GetTimeSectionByWeight(TimeHelper.GetNow(), 6);
            return setion;
        }
    }
}
