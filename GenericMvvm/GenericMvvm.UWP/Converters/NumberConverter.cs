using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace GenericMvvm.UWP
{
    /// <summary>
    /// NULL許容しない数値型の初期値ゼロを空欄表示する
    /// </summary>
    public class NumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var src = (int)value;
            if (src == 0)
            {
                return "";
            }
            else
            {
                return src.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var src = value as string;
            if (string.IsNullOrEmpty(src))
            {
                return 0;
            }
            else
            {
                return int.Parse(src);
            }
        }
    }
}
