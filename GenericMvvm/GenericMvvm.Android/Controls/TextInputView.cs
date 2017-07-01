using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;
using Android.Text;
using static Android.Widget.TextView;
using Android.Views.InputMethods;
using System.Reflection;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V4.Content;

namespace GenericMvvm.Droid
{
    /// <summary>
    /// バインドできるテキスト入力コントロール
    /// 追加したプロパティにバインドする
    /// </summary>
    public class TextInputView : LinearLayout
    {
        /// <summary>
        /// テキスト入力イベント
        /// </summary>
        public event EventHandler<Android.Text.TextChangedEventArgs> TextChanged;
        /// <summary>
        /// IMEリターンキー設定
        /// 次へ・完了など
        /// </summary>
        public ImeAction ImeOptions
        {
            get { return FindViewById<TextInputEditText>(Resource.Id.textInputEditText).ImeOptions; }
            set { FindViewById<TextInputEditText>(Resource.Id.textInputEditText).ImeOptions = value; }
        }
        /// <summary>
        /// IMEタイプ
        /// 数字・文字入力など
        /// </summary>
        public InputTypes InputType
        {
            get { return FindViewById<TextInputEditText>(Resource.Id.textInputEditText).InputType; }
            set { FindViewById<TextInputEditText>(Resource.Id.textInputEditText).InputType = value; }
        }
        /// <summary>
        /// 入力した文字列
        /// </summary>
        public string Text
        {
            get { return FindViewById<TextInputEditText>(Resource.Id.textInputEditText).Text; }
            set { FindViewById<TextInputEditText>(Resource.Id.textInputEditText).Text = value; }
        }
        /// <summary>
        /// プレースホルダー兼タイトル
        /// フローティングラベルの場合はタイトルが未入力時のプレースホルダーになっている
        /// </summary>
        public string Hint
        {
            get { return FindViewById<TextInputLayout>(Resource.Id.textInputLayout).Hint; }
            set { FindViewById<TextInputLayout>(Resource.Id.textInputLayout).Hint = value; }
        }
        /// <summary>
        /// エラー文字列の配列
        /// </summary>
        public IEnumerable<string> Errors
        {
            set
            {
                // フローティングラベルが未入力エラー時に誤動作する対応
                // 必須バリデーションの判定
                if (value != null && value.Count() > 0 && string.IsNullOrEmpty(Text))
                {
                    // 必須でエラーの場合
                    FindViewById<TextInputLayout>(Resource.Id.textInputLayout).Error = null;
                }
                else
                {
                    FindViewById<TextInputLayout>(Resource.Id.textInputLayout).Error = (value == null) ? null : string.Join("\n", value);
                }
            }
        }
        /// <summary>
        /// エラー発生中は真
        /// </summary>
        public bool IsError
        {
            set
            {
                // フローティングラベルが未入力エラー時に誤動作する対応
                if (value)
                {
                    // エラー
                    if (string.IsNullOrEmpty(Text))
                    {
                        // 必須エラー
                        FindViewById<TextInputLayout>(Resource.Id.textInputLayout).ErrorEnabled = false;
                        FindViewById<ImageView>(Resource.Id.imageViewRequired).SetImageDrawable(GetRequiredDrawable(Resource.Color.colorRequiredError));
                    }
                    else
                    {
                        // その他のエラー
                        FindViewById<TextInputLayout>(Resource.Id.textInputLayout).ErrorEnabled = value;
                        FindViewById<ImageView>(Resource.Id.imageViewRequired).SetImageDrawable(GetRequiredDrawable(Resource.Color.colorRequiredInfo));
                    }

                    FindViewById<ImageView>(Resource.Id.imageViewRequired).Visibility = ViewStates.Visible;
                }
                else
                {
                    // 正常
                    FindViewById<TextInputLayout>(Resource.Id.textInputLayout).ErrorEnabled = value;
                    FindViewById<ImageView>(Resource.Id.imageViewRequired).Visibility = ViewStates.Gone;
                }
            }
        }
        /// <summary>
        /// 着色した必須アイコン画像を得る
        /// 下位互換のためサポートライブラリで着色する
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private Drawable GetRequiredDrawable(int c)
        {
            var d = DrawableCompat.Wrap(ContextCompat.GetDrawable(Context, Android.Resource.Drawable.IcDialogAlert));
            DrawableCompat.SetTint(d, ContextCompat.GetColor(Context, c));
            DrawableCompat.SetTintMode(d, PorterDuff.Mode.SrcIn);
            return d;
        }

        /// <summary>
        /// コンストラクタ２
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attrs"></param>
        public TextInputView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }
        /// <summary>
        /// コンストラクタ３
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attrs"></param>
        /// <param name="defStyle"></param>
        public TextInputView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }
        /// <summary>
        /// コンストラクタ実体
        /// </summary>
        private void Initialize()
        {
            Inflate(Context, Resource.Layout.TextInputView, this);

            var textInputEditText = FindViewById<TextInputEditText>(Resource.Id.textInputEditText);

            textInputEditText.TextChanged += TextInputView_TextChanged;
            textInputEditText.SetOnEditorActionListener(new OnEditorActionListener());
        }
        /// <summary>
        /// テキスト入力イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextInputView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }
        /// <summary>
        /// IMEイベント
        /// </summary>
        public class OnEditorActionListener : Java.Lang.Object, IOnEditorActionListener
        {
            public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
            {
                System.Diagnostics.Debug.WriteLine("{0} ACTION={1} EVENT={2}", new[]
                {
                    MethodBase.GetCurrentMethod().Name,
                    actionId.ToString(),
                    e?.ToString()
                });

                if (actionId == ImeAction.Done)
                {
                    // 終わりのアクションでIMEを消す
                    var imm = v.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
                    imm.HideSoftInputFromWindow(v.WindowToken, HideSoftInputFlags.None);

                    // ここでクリアすると先頭に移動するらしい
                    v.ClearFocus();

                    // イベント消費完了の場合は真を返す
                    return true;
                }

                // ここでは何もせずにシステムに渡すときは偽を返す
                return false;
            }
        }
    }
}