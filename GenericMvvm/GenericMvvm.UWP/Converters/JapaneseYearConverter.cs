using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace GenericMvvm.UWP
{
    class JapaneseYearConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                var d = new DateTime((int)value, 1, 1);
                var ci = new CultureInfo("ja-JP");
                ci.DateTimeFormat.Calendar = new JapaneseCalendar();
                return d.ToString("gy", ci);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
