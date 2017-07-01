using System;
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
    public class BirthFragment : Android.Support.V4.App.Fragment
    {
        const string FORMAT = "----BirthFragment Event---- {0}";

        /// <summary>
        /// 親画面はアタッチイベントで管理する
        /// </summary>
        MainActivity _MainActivity;
        /// <summary>
        /// VMはフォアグラウンド復帰で取得しなおす
        /// </summary>
        BirthViewModel _VM;
        /// <summary>
        /// バインド情報
        /// </summary>
        TextInputViewBind _TextInputViewBind;
        /// <summary>
        /// ここでは何もしない
        /// </summary>
        /// <param name="savedInstanceState"></param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnCreate(savedInstanceState);
        }
        /// <summary>
        /// レイアウトからビューを構築する
        /// </summary>
        /// <param name="inflater"></param>
        /// <param name="container"></param>
        /// <param name="savedInstanceState"></param>
        /// <returns></returns>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            return inflater.Inflate(Resource.Layout.Birth, container, false);
        }
        /// <summary>
        /// アクティビティを保存する
        /// </summary>
        /// <param name="context"></param>
        public override void OnAttach(Context context)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnAttach(context);

            _MainActivity = context as MainActivity;
        }
        /// <summary>
        /// アクティビティをクリアする
        /// </summary>
        public override void OnDetach()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDetach();

            _MainActivity = null;
        }
        /// <summary>
        /// フォアグラウンド復帰
        /// この画面の生成
        /// </summary>
        public override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnResume();

            // ビューモデル生成
            _VM = _MainActivity.BizLogic.GetViewModel<BirthViewModel>();

            // カスタムコントロールバインド情報
            _TextInputViewBind = new TextInputViewBind(View, _VM);
            _TextInputViewBind.Add(new TextInputViewBind.Info
            {
                PropName = nameof(_VM.Year),
                PropType = TextInputViewBind.ConverterType.INT,
                ResId = Resource.Id.textInputViewYear,
                InputType = Android.Text.InputTypes.ClassNumber,
                Hint = _VM.YearTitle
            });
            _TextInputViewBind.Add(new TextInputViewBind.Info
            {
                PropName = nameof(_VM.Month),
                PropType = TextInputViewBind.ConverterType.INT,
                ResId = Resource.Id.textInputViewMonth,
                InputType = Android.Text.InputTypes.ClassNumber,
                Hint = _VM.MonthTitle
            });
            _TextInputViewBind.Add(new TextInputViewBind.Info
            {
                PropName=nameof(_VM.Day),
                PropType = TextInputViewBind.ConverterType.INT,
                ResId = Resource.Id.textInputViewDay,
                InputType = Android.Text.InputTypes.ClassNumber,
                ImeOption = Android.Views.InputMethods.ImeAction.Done,
                Hint = _VM.DayTitle
            });

            // VMイベント
            _VM.PropertyChanged += _VM_PropertyChanged;

            // コントロールイベント
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click += BirthFragment_Click;

            // イベントをバインドした後にコントロールに初期設定するとエラー状態が確定する
            // 初期値設定 OneWay
            View.FindViewById<TitleTextView>(Resource.Id.titleTextViewDescription).Text = _VM.Description;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Text = _VM.CommitLabel;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Enabled = _VM.CanCommit;

            // 初期値設定 TwoWay
            _TextInputViewBind.Start();
        }
        /// <summary>
        /// バックグラウンド移行
        /// この画面の廃棄
        /// </summary>
        public override void OnPause()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnPause();

            // ここでバインド解除する
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click -= BirthFragment_Click;

            _TextInputViewBind.Stop();

            _VM.PropertyChanged -= _VM_PropertyChanged;

            _TextInputViewBind = null;

            _VM = null;
        }
        /// <summary>
        /// ここでは何もしない
        /// </summary>
        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDestroy();
        }
        /// <summary>
        /// ボタンイベント
        /// この画面はボタンが1個
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BirthFragment_Click(object sender, EventArgs e)
        {
            _VM.Commit();
        }
        /// <summary>
        /// VMからの状態変化通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("-- BirthFragment {0} {1}", new[] { MethodBase.GetCurrentMethod().Name, e.PropertyName });
            switch (e.PropertyName)
            {
                case nameof(_VM.CanCommit):
                    // ボタン活性化
                    View.FindViewById<Button>(Resource.Id.buttonCommit).Enabled = _VM.CanCommit;
                    break;

                default:
                    if (_TextInputViewBind.ContainsKey(e.PropertyName))
                    {
                        // カスタムコントロールの状態変化
                        _TextInputViewBind.PropertyChanged(sender, e);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("unknown VM EVENT " + e.PropertyName);
                    }
                    break;
            }

        }
    }
}