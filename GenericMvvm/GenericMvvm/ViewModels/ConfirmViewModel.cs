using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    public class ConfirmViewModel : BaseViewModel
    {
        public string Description { get { return "入力内容をご確認ください。"; } }

        public string NameTitle { get { return "お名前"; } }
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; ValidateProperty(nameof(Name), value); }
        }

        public string BirthTitle { get { return "生年月日"; } }
        private string _Birth;

        public string Birth
        {
            get { return _Birth; }
            set { _Birth = value; ValidateProperty(nameof(Birth), value); }
        }

        public string AddressTitle { get { return "住所"; } }
        private string _Address;

        public string Address
        {
            get { return _Address; }
            set { _Address = value; ValidateProperty(nameof(Address), value); }
        }

        public string AddressKanaTitle { get { return "住所（ふりがな）"; } }
        private string _AddressKana;

        public string AddressKana
        {
            get { return _AddressKana; }
            set { _AddressKana = value; ValidateProperty(nameof(AddressKana), value); }
        }

    }
}
