using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    public class ConfirmViewModel : BaseViewModel
    {
        public string Description { get { return "入力内容をご確認ください。"; } }

        public class ConfirmCell : BaseCell
        {
            private string _Title;

            public string Title
            {
                get { return _Title; }
                set { _Title = value; ValidateProperty(nameof(Title), value); }
            }

            private string _Value;

            public string Value
            {
                get { return _Value; }
                set { _Value = value; ValidateProperty(nameof(Value), value); }
            }

        }

        private ObservableCollection<ConfirmCell> _ConfirmList;

        public ObservableCollection<ConfirmCell> ConfirmList
        {
            get { return _ConfirmList; }
            set { _ConfirmList = value; ValidateProperty(nameof(ConfirmList), value); }
        }

        public ConfirmViewModel()
        {
            ConfirmList = new ObservableCollection<ConfirmCell>();
        }
    }
}
