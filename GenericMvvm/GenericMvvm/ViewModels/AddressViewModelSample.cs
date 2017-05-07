using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    public class AddressViewModelSample : AddressViewModel
    {
        public AddressViewModelSample()
        {
            PostalCode = "1234567";
            Address = "東京都八王子市ほげほげ村";
            AddressKana = "アイウエオカキクケコ";
            Address1 = "東京都";
            Address2 = "八王子市";
            Address3 = "帆げ歩げ村";
            AddressKana1 = "ﾄｳｷｮｳﾄ";
            AddressKana2 = "ﾊﾁｵｳｼﾞｼ";
            AddressKana3 = "ﾎｹﾞﾎｹﾞﾑﾗ";

            _Dict.Add("PostalCode", new ObservableCollection<string>(new[] { "エラーサンプル1", "サンプル2" }));
        }
    }
}
