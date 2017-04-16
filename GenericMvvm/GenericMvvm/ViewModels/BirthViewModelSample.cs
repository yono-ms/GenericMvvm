using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    public class BirthViewModelSample : BirthViewModel
    {
        public BirthViewModelSample()
        {
            Year = 2017;
            Month = 1;
            Day = 1;
            _Dict.Add("Year", new ObservableCollection<string>(new[] { "エラーサンプル1", "サンプル2" }));
            _Dict.Add("Month", new ObservableCollection<string>(new[] { "エラーサンプル1", "サンプル2" }));
            _Dict.Add("Day", new ObservableCollection<string>(new[] { "エラーサンプル1", "サンプル2" }));
        }
    }
}
