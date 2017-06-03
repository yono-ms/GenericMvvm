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

namespace GenericMvvm.Droid
{
    public class TitleTextView : FrameLayout
    {
        public string Text
        {
            get { return FindViewById<TextView>(Resource.Id.textView).Text; }
            set { FindViewById<TextView>(Resource.Id.textView).Text = value; }
        }
        public TitleTextView(Context context) : base(context)
        {
            Initialize();
        }
        public TitleTextView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public TitleTextView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            Inflate(Context, Resource.Layout.TitleTextView, this);
        }
    }
}