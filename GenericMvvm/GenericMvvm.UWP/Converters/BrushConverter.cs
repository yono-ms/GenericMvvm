using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace GenericMvvm.UWP
{
    class BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var src = (bool)value;
            if (src)
            {
                return new SolidColorBrush(Color.FromArgb(0x80, 0xff, 0, 0));
            }
            else
            {
                return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
