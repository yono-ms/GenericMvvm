using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    public class ConfirmViewModelSample : ConfirmViewModel
    {
        public ConfirmViewModelSample()
        {
            Name = "山田 太郎";
            Birth = "2000年12月31日";
            Address = "神奈川県横浜市神奈川区羽沢南";
            AddressKana = "あいうえおかきくけこ";
        }
    }
}
