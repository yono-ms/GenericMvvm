using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace GenericMvvm.UWP
{
    public class CustomFrame : Frame
    {
        const string FORMAT = "----CustomFrameEvent---- {0}";

        private Image _LeftImage;
        private Image _RightImage;

        /// <summary>
        /// アニメーション用の土台
        /// </summary>
        private StackPanel _StackPanel;
        /// <summary>
        /// 画面遷移待ち
        /// </summary>
        private bool Waiting = false;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CustomFrame()
        {
            _LeftImage = new Image();
            _RightImage = new Image();

            _StackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            _StackPanel.Children.Add(_LeftImage);
            _StackPanel.Children.Add(_RightImage);
            _StackPanel.RenderTransform = new CompositeTransform();
        }
        /// <summary>
        /// 進むなら真
        /// </summary>
        private bool Forward;
        /// <summary>
        /// 遷移前の画像
        /// </summary>
        public Image OldImage { get { return Forward ? _LeftImage : _RightImage; } }
        /// <summary>
        /// 遷移後の画像
        /// </summary>
        public Image NewImage { get { return Forward ? _RightImage : _LeftImage; } }
        /// <summary>
        /// 画面遷移
        /// </summary>
        /// <param name="type">画面</param>
        /// <param name="forward">進むなら真</param>
        public void Navigate(Type type, bool forward)
        {
            Forward = forward;

            try
            {
                Task.Run(async () =>
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        // 画像を作る
                        var rtb = new RenderTargetBitmap();
                        await rtb.RenderAsync(Content as Page);
                        OldImage.Source = rtb;

                        // 土台を最前面に挿入
                        var grid = Parent as Grid;
                        grid.Children.Add(_StackPanel);

                        // 画面遷移を実行
                        Waiting = true;
                        Navigate(type);
                    });
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { "OnContentChanged" });

            try
            {
                if (!Waiting)
                {
                    // 最初の画面でも来る
                    return;
                }
                else
                {
                    Waiting = false;
                }

                Task.Run(async () => await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    // 画像を作る
                    var rtb = new RenderTargetBitmap();
                    await rtb.RenderAsync(Content as Page);
                    NewImage.Source = rtb;

                    // アニメーションで土台を移動
                    var sb = new Storyboard();
                    var dest = (Forward ? -1 : 1) * rtb.PixelWidth;
                    var anim = new DoubleAnimation
                    {
                        From = 0,
                        To = dest,
                        Duration = new Duration(TimeSpan.FromMilliseconds(2000))
                    };
                    Storyboard.SetTargetProperty(anim, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
                    Storyboard.SetTarget(anim, _StackPanel);
                    sb.Children.Add(anim);

                    sb.Completed += (s, e) =>
                    {
                        // 親のGridを得る
                        var grid = Parent as Grid;

                        // 画像を外す
                        grid.Children.Remove(_StackPanel);

                        // 廃棄
                        _LeftImage.Source = null;
                        _RightImage.Source = null;
                    };

                    // スタート
                    sb.Begin();
                }));

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(FORMAT, new[] { ex.ToString() });
            }
        }
    }
}
