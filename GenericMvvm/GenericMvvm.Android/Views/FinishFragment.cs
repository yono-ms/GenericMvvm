﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Reflection;

namespace GenericMvvm.Droid
{
    public class FinishFragment : Fragment
    {
        const string FORMAT = "----FinishFragment Event---- {0}";

        /// <summary>
        /// 親画面はアタッチイベントで管理する
        /// </summary>
        MainActivity _MainActivity;
        /// <summary>
        /// VMはフォアグラウンド復帰で取得しなおす
        /// </summary>
        FinishViewModel _VM;

        public override void OnCreate(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return inflater.Inflate(Resource.Layout.Finish, container, false);
        }

        public override void OnAttach(Context context)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnAttach(context);

            _MainActivity = context as MainActivity;
        }

        public override void OnDetach()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDetach();

            _MainActivity = null;
        }

        public override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnResume();

            // ビューモデル生成
            _VM = _MainActivity.BizLogic.GetViewModel<FinishViewModel>();

            // コントロールイベント
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click += FinishFragment_Click;

            // 初期値設定
            View.FindViewById<TitleTextView>(Resource.Id.titleTextViewDescription).Text = _VM.Description;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Text = _VM.CommitLabel;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Enabled = _VM.CanCommit;
        }

        public override void OnPause()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnPause();

            View.FindViewById<Button>(Resource.Id.buttonCommit).Click -= FinishFragment_Click;

            _VM = null;
        }

        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDestroy();
        }

        private void FinishFragment_Click(object sender, EventArgs e)
        {
            _VM.Commit();
        }
    }
}