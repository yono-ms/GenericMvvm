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

namespace GenericMvvm.Droid
{
    public class TextInputView : LinearLayout
    {
        public event EventHandler<Android.Text.TextChangedEventArgs> TextChanged;

        public ImeAction ImeOptions
        {
            get { return FindViewById<TextInputEditText>(Resource.Id.textInputEditText).ImeOptions; }
            set { FindViewById<TextInputEditText>(Resource.Id.textInputEditText).ImeOptions = value; }
        }

        public InputTypes InputType
        {
            get { return FindViewById<TextInputEditText>(Resource.Id.textInputEditText).InputType; }
            set { FindViewById<TextInputEditText>(Resource.Id.textInputEditText).InputType = value; }
        }

        public string Text
        {
            get { return FindViewById<TextInputEditText>(Resource.Id.textInputEditText).Text; }
            set { FindViewById<TextInputEditText>(Resource.Id.textInputEditText).Text = value; }
        }

        public string Hint
        {
            get { return FindViewById<TextInputLayout>(Resource.Id.textInputLayout).Hint; }
            set { FindViewById<TextInputLayout>(Resource.Id.textInputLayout).Hint = value; }
        }

        public IEnumerable<string> Errors
        {
            set { FindViewById<TextInputLayout>(Resource.Id.textInputLayout).Error = (value==null) ? null : string.Join("\n", value); }
        }

        public bool IsError
        {
            set { FindViewById<TextInputLayout>(Resource.Id.textInputLayout).ErrorEnabled = value; }
        }

        public TextInputView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public TextInputView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            Inflate(Context, Resource.Layout.TextInputView, this);

            var textInputEditText = FindViewById<TextInputEditText>(Resource.Id.textInputEditText);

            textInputEditText.TextChanged += TextInputView_TextChanged;
            textInputEditText.SetOnEditorActionListener(new OnEditorActionListener());
        }

        private void TextInputView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }

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
                    // 終わりのアクション
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