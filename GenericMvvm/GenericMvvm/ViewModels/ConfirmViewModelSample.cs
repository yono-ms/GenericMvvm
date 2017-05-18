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
            ConfirmList.Add(new ConfirmCell { Title = "お名前", Value = "山田 太郎" });
            ConfirmList.Add(new ConfirmCell { Title = "生年月日", Value = "2000年12月31日" });
            ConfirmList.Add(new ConfirmCell { Title = "住所", Value = "神奈川県横浜市神奈川区羽沢南" });
            ConfirmList.Add(new ConfirmCell { Title = "住所（ふりがな）", Value = "あいうえおかきくけこ" });
        }
    }
}
