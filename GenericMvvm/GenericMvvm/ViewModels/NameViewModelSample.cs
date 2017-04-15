using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    public class NameViewModelSample : NameViewModel
    {
        public NameViewModelSample()
        {
            FirstName = "鈴木";
            LastName = "一郎";
            _Dict.Add("FirstName", new ObservableCollection<string>(new[] { "エラーサンプル" }));
        }
    }
}
