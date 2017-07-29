using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Windows.UI.Xaml.Media.Imaging;
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

        public CustomFrame ContentFrame { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { "MainPage" });

            // UWPでは起動時サイズ指定可能
            ApplicationView.PreferredLaunchViewSize = new Size { Width = 480, Height = 640 };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            // フレームを構築してBizLogicが無くてもUIだけで表示できる初期ページを表示
            ContentFrame = new CustomFrame();
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
                    _BizLogic = BizLogic.LoadBizLogic(new NativeCallUWP(this));

                    // ページから参照できるようにアプリケーションのプロパティにセット
                    (Application.Current as App).BizLogic = _BizLogic;

                    // UIスレッドに戻す
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
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

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            _VM.GoBack();
        }

        /// <summary>
        /// 現在の画面遷移方向
        /// 進行方向なら真
        /// </summary>
        private bool _IsForward;
        /// <summary>
        /// 画面遷移アニメーション実装
        /// </summary>
        /// <param name="page"></param>
        /// <param name="forward"></param>
        public void NavigateTo(Type page, bool forward)
        {
            _IsForward = forward;

            try
            {
                Task.Run(async () =>
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        // Step 0 ガードを上げる
                        gridGuard.Visibility = Visibility.Visible;

                        // Step 1 現在の表示を画像化
                        var rtb = new RenderTargetBitmap();
                        await rtb.RenderAsync(ContentFrame.Content as Page);

                        // Step 2.1 Imageを差し替えて配置
                        imageOld.Source = rtb;

                        doubleAnimationImage.From = _IsForward ? 0 : 0;
                        doubleAnimationImage.To = _IsForward ? -rtb.PixelWidth : rtb.PixelWidth;
                        doubleAnimationContent.From = _IsForward ? rtb.PixelWidth : -rtb.PixelWidth;
                        doubleAnimationContent.To = _IsForward ? 0 : 0;

                        // Step 2.2 Frameに画面遷移命令
                        ContentFrame.Navigated += ContentFrame_Navigated;
                        ContentFrame.Navigate(page, forward);
                    });
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // ガードを下げる
                gridGuard.Visibility = Visibility.Collapsed;
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            try
            {
                // イベントハンドラを外す
                ContentFrame.Navigated -= ContentFrame_Navigated;

                // Step 3 アニメーション開始
                storyboard.Completed += Storyboard_Completed;
                storyboard.Begin();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // ガードを下げる
                gridGuard.Visibility = Visibility.Collapsed;
            }
        }

        private void Storyboard_Completed(object sender, object e)
        {
            // イベントハンドラを外す
            storyboard.Completed -= Storyboard_Completed;

            // Step 4 後始末
            imageOld.Source = null;

            // ガードを下げる
            gridGuard.Visibility = Visibility.Collapsed;
        }
    }
}
