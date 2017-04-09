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

        private MainViewModel _VM;

        public Frame ContentFrame { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { "MainPage" });

            // UWPでは起動時サイズ指定可能
            ApplicationView.PreferredLaunchViewSize = new Size { Width = 480, Height = 640 };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            // フレームを構築して初期ページを表示
            ContentFrame = new Frame();
            gridContent.Children.Add(ContentFrame);
            ContentFrame.Navigate(typeof(FirstPage));

            // アプリケーションの生存判定（UWPの場合は必ずnull）
            if (_BizLogic == null)
            {
                System.Diagnostics.Debug.WriteLine("BizLogic復元ルート");

                // 復元する
                Task.Run(async () =>
                {
                    // 非同期なので別スレッドで実行する
                    _BizLogic = await BizLogic.LoadBizLogicAsync(new NativeCallUWP(this));
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        // UIスレッドに戻す
                        Initialize();
                    });
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
            // メイン画面の場合はアプリケーションの起動を待ってからVM開始
            _VM = _BizLogic.GetViewModel<MainViewModel>();
            // バインド
            DataContext = _VM;
            // 起動
            _VM.KickStart();
        }
    }
}
