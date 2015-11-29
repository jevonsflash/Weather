using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Weather.App.Convert
{
    class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                Color col = Weather.Utils.ColorHelper.GetColorFromHEX(value);
                SolidColorBrush brush = new SolidColorBrush(col);
                return brush;

            }
            catch (Exception e)
            {

                return new SolidColorBrush(Colors.Black);
            }

        }



        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
