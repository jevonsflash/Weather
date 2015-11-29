using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Weather.Utils
{
    public class ColorHelper
    {
        public static Color GetColorFromHEX(object value)
        {
            uint color = System.Convert.ToUInt32(value.ToString(), fromBase: 16);
            byte A = (byte)((color & 0xFF000000) >> 24);
            byte R = (byte)((color & 0x00FF0000) >> 16);
            byte G = (byte)((color & 0x0000FF00) >> 8);
            byte B = (byte)((color & 0x000000FF) >> 0);
            Color col = Color.FromArgb(A, R, G, B);
            if (col.A == 0)
            {
                col.A = 255;
            }

            return col;
        }
    }
}
