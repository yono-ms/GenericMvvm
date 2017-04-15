using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace GenericMvvm.UWP
{
    public class CustomFrame : Frame
    {
        const string FORMAT = "----CustomFrameEvent---- {0}";

        private Image _OldImage;

        private bool Waiting = false;

        public void Navigate(Type type, bool forward)
        {
            try
            {
                Task.Run(async () =>
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                    {
                        // 親のGridを得る
                        var grid = Parent as Grid;

                        // 画像を作る
                        var rtb = new RenderTargetBitmap();
                        await rtb.RenderAsync(grid);
                        _OldImage = new Image();
                        _OldImage.Source = rtb;
                        //_OldImage.Width = rtb.PixelWidth;
                        //_OldImage.Height = rtb.PixelHeight;

                        // 作った画像を最前面に挿入
                        grid.Children.Add(_OldImage);

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

                Task.Run(async () => await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // アニメーションで古い画像を消す
                    var sb = new Storyboard();
                    var anim = new DoubleAnimationUsingKeyFrames();
                    sb.Children.Add(anim);
                    anim.KeyFrames.Add(new LinearDoubleKeyFrame { Value = 100, KeyTime = TimeSpan.FromSeconds(0) });
                    anim.KeyFrames.Add(new LinearDoubleKeyFrame { Value = 50, KeyTime = TimeSpan.FromSeconds(1) });
                    anim.KeyFrames.Add(new LinearDoubleKeyFrame { Value = 0, KeyTime = TimeSpan.FromSeconds(2) });
                    //anim.EnableDependentAnimation = true;
                    Storyboard.SetTargetProperty(anim, "Opacity");
                    Storyboard.SetTarget(anim, _OldImage);

                    sb.Completed += (s, e) =>
                    {
                        // 親のGridを得る
                        var grid = Parent as Grid;

                        // 画像を外す
                        grid.Children.Remove(_OldImage);

                        // 廃棄
                        _OldImage = null;
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
