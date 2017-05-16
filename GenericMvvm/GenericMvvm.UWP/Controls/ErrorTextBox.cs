using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace GenericMvvm.UWP
{
    /// <summary>
    /// 【不採用】
    /// テンプレートコントロールから作ったカスタムテキストボックス
    /// 基本クラスをTextBoxにするとTextBoxが持つプロパティはそのまま使える
    /// テンプレートはGeneric.xamlにできているがTextBoxの拡張ではないので機能低下する
    /// UserControlと同じことをテンプレートで作れるがデザイナーが使えるか不明
    /// </summary>
    public sealed class ErrorTextBox : TextBox
    {
        public ErrorTextBox()
        {
            this.DefaultStyleKey = typeof(ErrorTextBox);
        }


        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ErrorTextBox), new PropertyMetadata(null));


    }
}
