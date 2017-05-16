using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace GenericMvvm.UWP
{
    /// <summary>
    /// 【不採用】
    /// コードから作成した単純なTextBox継承クラス
    /// Blendでテンプレートを拡張して機能追加する
    /// テンプレート拡張時にBorderで囲むとフォーカス効果が消えることがわかった
    /// </summary>
    public class ExTextBox : TextBox
    {
        /// <summary>
        /// エラー文字列配列
        /// </summary>
        public IEnumerable Errors
        {
            get { return (IEnumerable)GetValue(ErrorsProperty); }
            set { SetValue(ErrorsProperty, value); }
        }
        /// <summary>
        /// 依存プロパティを作ると外部からはBinding
        /// 内部からはTemplateBindingで接続できる
        /// </summary>
        // Using a DependencyProperty as the backing store for Errors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorsProperty =
            DependencyProperty.Register("Errors", typeof(IEnumerable), typeof(ExTextBox), new PropertyMetadata(null));


        /// <summary>
        /// エラー配色
        /// </summary>
        public Brush ErrorBrush
        {
            get { return (Brush)GetValue(ErrorBrushProperty); }
            set { SetValue(ErrorBrushProperty, value); }
        }
        /// <summary>
        /// コンバーターを作ってboolをBrushにできるようにしておく
        /// 外部はboolをコンバーターでBindingする
        /// 内部はTemplateBindingする
        /// 依存関係プロパティは内部に接続できる形式で作る
        /// </summary>
        // Using a DependencyProperty as the backing store for ErrorBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorBrushProperty =
            DependencyProperty.Register("ErrorBrush", typeof(Brush), typeof(ExTextBox), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0))));


    }
}
