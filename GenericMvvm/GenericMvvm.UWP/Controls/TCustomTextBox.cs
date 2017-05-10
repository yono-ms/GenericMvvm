using System;
using System.Collections;
using System.Collections.Generic;
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
    /// テンプレートコントロールで作成するカスタムテキストボックス。
    /// 使用するときにバインドしたいプロパティを依存関係プロパティで追加する。
    /// テンプレートはGeneric.xamlにできているのでUIElementを適当に追加する。
    /// テンプレートの中でTemplateBindingを使って依存関係プロパティと連結する。
    /// 最も手間がかからないカスタムコントロールの実装方法。
    /// </summary>
    public sealed class TCustomTextBox : Control
    {
        public TCustomTextBox()
        {
            this.DefaultStyleKey = typeof(TCustomTextBox);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TCustomTextBox), new PropertyMetadata(null));


        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(TCustomTextBox), new PropertyMetadata(null));


        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaceholderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(TCustomTextBox), new PropertyMetadata(null));


        /// <summary>
        /// 外部バインド用
        /// </summary>
        public InputScopeNameValue InputScope
        {
            get { return (InputScopeNameValue)GetValue(InputScopeProperty); }
            set { SetValue(InputScopeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputScope.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputScopeProperty =
            DependencyProperty.Register("InputScope", typeof(InputScopeNameValue), typeof(TCustomTextBox), new PropertyMetadata(null, (s, e) =>
            {
                // バインドされた値を変換して設定する
                var inputScopeName = new InputScopeName();
                inputScopeName.NameValue = (InputScopeNameValue)e.NewValue;

                var inputScope = new InputScope();
                inputScope.Names.Add(inputScopeName);

                var ctrl = s as TCustomTextBox;
                ctrl.InputScopeParent = inputScope;
            }));

        /// <summary>
        /// 内部テンプレートバインド用
        /// </summary>
        public InputScope InputScopeParent
        {
            get { return (InputScope)GetValue(InputScopeParentProperty); }
            set { SetValue(InputScopeParentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputScopeParent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputScopeParentProperty =
            DependencyProperty.Register("InputScopeParent", typeof(InputScope), typeof(TCustomTextBox), new PropertyMetadata(null));


        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(TCustomTextBox), new PropertyMetadata(null));


    }
}
