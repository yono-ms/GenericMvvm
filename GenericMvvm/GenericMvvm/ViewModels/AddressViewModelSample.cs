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

            var list = new List<ZipCloudResponse.result>();
            list.Add(new ZipCloudResponse.result()
            {
                address1 = "東京都",
                address2 = "八王子市",
                address3 = "帆げ歩げ村",
                kana1 = "ﾄｳｷｮｳﾄ",
                kana2 = "ﾊﾁｵｳｼﾞｼ",
                kana3 = "ﾎｹﾞﾎｹﾞﾑﾗ"
            });

            list.Add(new ZipCloudResponse.result()
            {
                address1 = "東京都",
                address2 = "八王子市",
                address3 = "ぴよぴよ村",
                kana1 = "ﾄｳｷｮｳﾄ",
                kana2 = "ﾊﾁｵｳｼﾞｼ",
                kana3 = "ﾋﾟﾖﾋﾟﾖﾑﾗ"
            });

            ResponseResults = list;

            _Dict.Add("PostalCode", new ObservableCollection<string>(new[] { "エラーサンプル1", "サンプル2" }));
        }
    }
}
