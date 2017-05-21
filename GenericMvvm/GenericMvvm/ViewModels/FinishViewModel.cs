using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    public class FinishViewModel : BaseViewModel
    {
        public string Description { get { return "ありがとうございます。\n登録が完了しました。"; } }
        public new string CommitLabel
        { get { return "入力内容を削除してアプリを終了する"; } }

        public override void Commit()
        {
            base.Commit();

            _BizLogic.Commit();
        }
    }
}
