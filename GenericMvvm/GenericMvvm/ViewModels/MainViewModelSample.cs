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
            Fotter = "サンプルのフッター";
            ShowProgress = true;
            InitialText = "ロード中です";
        }
    }
}
