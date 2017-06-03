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

namespace GenericMvvm.Droid
{
    public class TextInputView : LinearLayout
    {
        public event EventHandler<Android.Text.TextChangedEventArgs> TextChanged;

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

            FindViewById<TextInputEditText>(Resource.Id.textInputEditText).TextChanged += TextInputView_TextChanged;
        }

        private void TextInputView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }
    }
}