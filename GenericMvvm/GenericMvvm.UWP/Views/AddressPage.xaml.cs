using System;
using System.Collections.Generic;
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

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace GenericMvvm.UWP
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class AddressPage : Page
    {
        AddressViewModel _VM;

        public AddressPage()
        {
            this.InitializeComponent();

            _VM = (Application.Current as App).BizLogic.GetViewModel<AddressViewModel>();
            DataContext = _VM;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _VM.Commit();
        }

        private void buttonGet_Click(object sender, RoutedEventArgs e)
        {
            _VM.CommandGet();
        }

        private void buttonCopy_Click(object sender, RoutedEventArgs e)
        {
            _VM.CommandCopy();
        }
    }
}
