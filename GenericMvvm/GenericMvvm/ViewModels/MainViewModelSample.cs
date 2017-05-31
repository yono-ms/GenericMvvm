using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMvvm
{
    public class MainViewModelSample : MainViewModel
    {
        public MainViewModelSample()
        {
            Title = "サンプルのタイトル";
            Footer = "サンプルのフッター";
            ShowProgress = true;
            ShowBackButton = true;
            var errors = new System.Collections.ObjectModel.ObservableCollection<string>();
            errors.Add("システムエラー");
            errors.Add("スタックトレース");
            errors.Add("メッセージ");
            ObjectErrors = errors;
        }
    }
}
