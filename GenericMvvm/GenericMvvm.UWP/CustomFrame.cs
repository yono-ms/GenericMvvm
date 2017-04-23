using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CustomFrame()
        {
        }
        /// <summary>
        /// 画面遷移
        /// </summary>
        /// <param name="type">画面</param>
        /// <param name="forward">進むなら真</param>
        public void Navigate(Type type, bool forward)
        {
            Debug.WriteLine("BackStackDepth=" + BackStackDepth);
            foreach (var item in BackStack)
            {
                Debug.WriteLine(item.SourcePageType);
            }

            if (forward)
            {
                // 進む
                Navigate(type);
            }
            else
            {
                if (CanGoBack)
                {
                    // 戻る
                    if (BackStack.Last().SourcePageType == type)
                    {
                        Debug.WriteLine("Simple GoBack");
                        GoBack();
                    }
                    else
                    {
                        while (CanGoBack && BackStack.Last().SourcePageType != type)
                        {
                            var item = BackStack.Last();
                            Debug.WriteLine("Remove stack " + item.SourcePageType);
                            BackStack.Remove(item);
                        }
                        if (CanGoBack)
                        {
                            Debug.WriteLine("Skip GoBack");
                            GoBack();
                        }
                        else
                        {
                            Debug.WriteLine("Unknown page stack clear navigate");
                            Navigate(type);
                        }
                    }
                }
                else
                {
                    // 戻れない
                    Debug.WriteLine("Unknown page navigate");
                    Navigate(type);
                }

            }
        }
    }
}
