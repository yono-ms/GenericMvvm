using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace GenericMvvm.UWP
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string FORMAT = "----MainPageEvent---- {0}";

        private BizLogic _BizLogic;

        public MainPage()
        {
            this.InitializeComponent();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { "MainPage" });

            // UWPでは起動時サイズ指定可能
            ApplicationView.PreferredLaunchViewSize = new Size { Width = 480, Height = 640 };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            if (_BizLogic == null)
            {
                System.Diagnostics.Debug.WriteLine("BizLogic復元ルート");

                // 復元する
                Task.Run(async () =>
                {
                    _BizLogic = await BizLogic.LoadBizLogicAsync(null);
                    Initialize();
                });
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("BizLogic生存ルート");
                Initialize();
            }
        }

        private void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
