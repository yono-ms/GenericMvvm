using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GenericMvvm.UWP
{
    public sealed partial class CustomTextBox : UserControl
    {
        public CustomTextBox()
        {
            this.InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CustomTextBox), new PropertyMetadata(null, (s, e) =>
            {
                Debug.WriteLine("Text {0} -> {1}", new[] { e.OldValue, e.NewValue });
                if (e.NewValue.Equals(e.OldValue))
                {
                    Debug.WriteLine("-- IGNORE");
                }
                else
                {
                    // コントロールに値を設定する
                    (s as CustomTextBox).textBox.Text = e.NewValue.ToString();
                }
            }));


        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaceholderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(CustomTextBox), new PropertyMetadata(null, (s, e) =>
            {
                Debug.WriteLine("PlaceholderText {0} -> {1}", new[] { e.OldValue, e.NewValue });
                // コントロールに値を設定する
                (s as CustomTextBox).textBox.PlaceholderText = e.NewValue.ToString();
            }));


        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(CustomTextBox), new PropertyMetadata(null, (s, e) =>
            {
                Debug.WriteLine("Header {0} -> {1}", new[] { e.OldValue, e.NewValue });
                // コントロールに値を設定する
                (s as CustomTextBox).textBox.Header = e.NewValue.ToString();
            }));


        public InputScope InputScope
        {
            get { return (InputScope)GetValue(InputScopeProperty); }
            set { SetValue(InputScopeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputScope.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputScopeProperty =
            DependencyProperty.Register("InputScope", typeof(InputScope), typeof(CustomTextBox), new PropertyMetadata(InputScopeNameValue.Text, (s, e) =>
            {
                Debug.WriteLine("InputScope {0} -> {1}", new[] { e.OldValue.ToString(), e.NewValue.ToString() });
                // コントロールに値を設定する
                (s as CustomTextBox).textBox.InputScope = e.NewValue as InputScope;
            }));


        public IEnumerable ItemsSource
        {
            get { return (IEnumerable )GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(CustomTextBox), new PropertyMetadata(null, (s, e) =>
            {
                var oldStrings = (e.OldValue == null) ? "(null)" : string.Join(",", e.OldValue as ObservableCollection<string>);
                var newStrings = (e.NewValue == null) ? "(null)" : string.Join(",", e.NewValue as ObservableCollection<string>);
                Debug.WriteLine("ItemsSource {0} -> {1}", new[] { oldStrings, newStrings });
                // コントロールに値を設定する
                (s as CustomTextBox).itemsControl.ItemsSource = e.NewValue;
            }));

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 依存プロパティに値を設定する
            var tb = sender as TextBox;
            Text = tb.Text;
        }
    }
}
