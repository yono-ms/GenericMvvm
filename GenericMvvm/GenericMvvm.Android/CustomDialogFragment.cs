using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Support.V4.App;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.Reflection;
using Android.App;

namespace GenericMvvm.Droid
{
    /// <summary>
    /// ダイアログフラグメントでの実装
    /// </summary>
    public class CustomDialogFragment : DialogFragment
    {
        const string FORMAT = "----CustomDialogFragment Event---- {0}";

        const string KEY_TITLE = "title";
        const string KEY_MESSAGE = "message";
        const string KEY_YES = "labelYes";
        const string KEY_NO = "labelNo";

        /// <summary>
        /// 引数をバンドルしてインスタンスを生成する
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="labelYes"></param>
        /// <param name="labelNo"></param>
        /// <returns></returns>
        public static CustomDialogFragment NewInstance(string title, string message, string labelYes, string labelNo)
        {
            // バンドルで渡しておくとOSが再構築時に引数を再現する
            var bundle = new Bundle();
            bundle.PutString(KEY_TITLE, title);
            bundle.PutString(KEY_MESSAGE, message);
            bundle.PutString(KEY_YES, labelYes);
            bundle.PutString(KEY_NO, labelNo);

            var fragment =  new CustomDialogFragment();
            fragment.Arguments = bundle;

            return fragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            // バンドルから引数を回収する
            var title = Arguments.GetString(KEY_TITLE);
            var message = Arguments.GetString(KEY_MESSAGE);
            var labelYes = Arguments.GetString(KEY_YES);
            var labelNo = Arguments.GetString(KEY_NO);

            var dialog = new AlertDialog.Builder(Activity).SetTitle(title).SetMessage(message);

            dialog.SetPositiveButton(labelYes, (s, e)=>
            {
                // メインのコールバックを実行する
                (Activity as MainActivity).OnDialogClick(labelYes);
            });

            if (!string.IsNullOrEmpty(labelNo))
            {
                dialog.SetNegativeButton(labelNo, (s, e) =>
                {
                    // メインのコールバックを実行する
                    (Activity as MainActivity).OnDialogClick(labelNo);
                });
            }

            // 枠外キャンセルさせない場合はフラグメント側に設定する
            Cancelable = false;

            return dialog.Create();
        }
    }
}