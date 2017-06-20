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

        public override void OnCreate(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            return inflater.Inflate(Resource.Layout.Birth, container, false);
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
            _VM = _MainActivity.BizLogic.GetViewModel<BirthViewModel>();

            // VMイベント
            _VM.PropertyChanged += _VM_PropertyChanged;

            // コントロールイベント
            View.FindViewById<TextInputView>(Resource.Id.textInputViewYear).TextChanged += BirthFragment_TextChanged;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewMonth).TextChanged += BirthFragment_TextChanged;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewDay).TextChanged += BirthFragment_TextChanged;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click += BirthFragment_Click;

            // イベントをバインドした後にコントロールに初期設定するとエラー状態が確定する
            // 初期値設定 OneWay
            View.FindViewById<TitleTextView>(Resource.Id.titleTextViewDescription).Text = _VM.Description;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Text = _VM.CommitLabel;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Enabled = _VM.CanCommit;
            // 初期値設定 EditText
            View.FindViewById<TextInputView>(Resource.Id.textInputViewYear).Hint = _VM.YearTitle;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewYear).Text = _VM.Year.ToString();
            View.FindViewById<TextInputView>(Resource.Id.textInputViewYear).InputType = Android.Text.InputTypes.ClassNumber;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewMonth).Hint = _VM.MonthTitle;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewMonth).Text = _VM.Month.ToString();
            View.FindViewById<TextInputView>(Resource.Id.textInputViewMonth).InputType = Android.Text.InputTypes.ClassNumber;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewDay).Hint = _VM.DayTitle;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewDay).Text = _VM.Day.ToString();
            View.FindViewById<TextInputView>(Resource.Id.textInputViewDay).InputType = Android.Text.InputTypes.ClassNumber;
        }

        public override void OnPause()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnPause();

            // ここでバインド解除する
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click -= BirthFragment_Click;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewYear).TextChanged -= BirthFragment_TextChanged;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewMonth).TextChanged -= BirthFragment_TextChanged;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewDay).TextChanged -= BirthFragment_TextChanged;

            _VM.PropertyChanged -= _VM_PropertyChanged;

            _VM = null;
        }

        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDestroy();
        }

        private void BirthFragment_Click(object sender, EventArgs e)
        {
            _VM.Commit();
        }

        private void BirthFragment_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            // 数値変換
            int num;
            if (!int.TryParse(e.Text.ToString(), out num))
            {
                System.Diagnostics.Debug.WriteLine("invalid value " + e.Text.ToString());
                num = 0;
            }

            var v = sender as TextInputView;
            switch (v.Id)
            {
                case Resource.Id.textInputViewYear:
                    _VM.Year = num;
                    break;

                case Resource.Id.textInputViewMonth:
                    _VM.Month = num;
                    break;

                case Resource.Id.textInputViewDay:
                    _VM.Day = num;
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("unknown CTRL EVENT " + v.Id);
                    break;
            }
        }

        private void _VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("-- PropertyChanged {0} {1}", new[] { MethodBase.GetCurrentMethod().Name, e.PropertyName });
            switch (e.PropertyName)
            {
                case nameof(_VM.CanCommit):
                    View.FindViewById<Button>(Resource.Id.buttonCommit).Enabled = _VM.CanCommit;
                    break;

                case nameof(_VM.Errors):
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewYear).Errors = _VM.Errors?[nameof(_VM.Year)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewMonth).Errors = _VM.Errors?[nameof(_VM.Month)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewDay).Errors = _VM.Errors?[nameof(_VM.Day)];
                    break;

                case nameof(_VM.IsError):
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewYear).IsError = _VM.IsError[nameof(_VM.Year)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewMonth).IsError = _VM.IsError[nameof(_VM.Month)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewDay).IsError = _VM.IsError[nameof(_VM.Day)];
                    break;

                case nameof(_VM.Year):
                case nameof(_VM.Month):
                case nameof(_VM.Day):
                    var v = sender.GetType().GetProperty(e.PropertyName).GetValue(sender).ToString();
                    if (v.Equals("0"))
                    {
                        v = "";
                    }
                    TextInputView c = null;
                    switch (e.PropertyName)
                    {
                        case nameof(_VM.Year):
                            c = View.FindViewById<TextInputView>(Resource.Id.textInputViewYear);
                            break;
                        case nameof(_VM.Month):
                            c = View.FindViewById<TextInputView>(Resource.Id.textInputViewMonth);
                            break;
                        case nameof(_VM.Day):
                            c = View.FindViewById<TextInputView>(Resource.Id.textInputViewDay);
                            break;
                    }
                    if (!c.Text.Equals(v))
                    {
                        c.Text = v;
                    }
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("unknown VM EVENT " + e.PropertyName);
                    break;
            }

        }
    }
}