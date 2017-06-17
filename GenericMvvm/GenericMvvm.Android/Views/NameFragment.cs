using Android.Content;
using Android.OS;
using Android.Views;
using System.Reflection;
using Android.Support.V4.App;
using Android.Widget;
using Android.Support.Design.Widget;
using System.Collections.Generic;

namespace GenericMvvm.Droid
{
    /// <summary>
    /// 寿命を予測できないインスタンス変数は持たない
    /// 各プロパティは使うときにOSが保証するライフサイクルから取り出す
    /// </summary>
    public class NameFragment : Fragment
    {
        const string FORMAT = "----NameFragment Event---- {0}";

        /// <summary>
        /// 親画面はアタッチイベントで管理する
        /// </summary>
        MainActivity _MainActivity;
        /// <summary>
        /// VMはフォアグラウンド復帰で取得しなおす
        /// </summary>
        NameViewModel _VM;
        /// <summary>
        /// VMに依存するためバインド情報もフォアグラウンド復帰で生成しなおす
        /// </summary>
        Dictionary<string, BindingInfo> Bindings;

        public override void OnCreate(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            return inflater.Inflate(Resource.Layout.Name, container, false);

            // テーマに設定したトランジッションアニメーションが効かないのでここでセットしてみるが無駄
            //var wrapper = new ContextThemeWrapper(Activity, Resource.Style.AppTheme);
            //var li = inflater.CloneInContext(wrapper);
            //return li.Inflate(Resource.Layout.Name, container, false);
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

        /// <summary>
        /// フォアグラウンド復帰
        /// </summary>
        public override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnResume();

            // バインド情報
            Bindings = new Dictionary<string, BindingInfo>();

            // ビューモデル生成
            _VM = _MainActivity.BizLogic.GetViewModel<NameViewModel>();

            // 姓
            var lastName = View.FindViewById<TextInputView>(Resource.Id.textInputViewLastName);
            lastName.Hint = _VM.LastNameTitle;
            Bindings.Add(nameof(_VM.LastName), new BindingInfo { Control = lastName, ControlProperty = nameof(lastName.Text) });

            // 名
            var firstName = View.FindViewById<TextInputView>(Resource.Id.textInputViewFirstName);
            firstName.Hint = _VM.FirstNameTitle;
            Bindings.Add(nameof(_VM.FirstName), new BindingInfo { Control = firstName, ControlProperty = nameof(firstName.Text) });

            // 固定値設定
            View.FindViewById<TitleTextView>(Resource.Id.titleTextViewDescription).Text = _VM.Description;

            // VMイベント
            _VM.PropertyChanged += _VM_PropertyChanged;

            // コントロールイベント
            View.FindViewById<TextInputView>(Resource.Id.textInputViewLastName).TextChanged += NameFragment_TextChanged;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewFirstName).TextChanged += NameFragment_TextChanged;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click += NameFragment_Click;

            // TwoWay初期値設定
            BindingInfo.Start(_VM, Bindings);
        }

        /// <summary>
        /// バックグラウンド移行
        /// </summary>
        public override void OnPause()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnPause();

            // ここでバインド解除する
            View.FindViewById<TextInputView>(Resource.Id.textInputViewLastName).TextChanged -= NameFragment_TextChanged;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewFirstName).TextChanged -= NameFragment_TextChanged;

            _VM.PropertyChanged -= _VM_PropertyChanged;

            Bindings.Clear();

            _VM = null;
        }

        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDestroy();
        }

        /// <summary>
        /// コントロールからのイベントは無条件にVMに設定する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameFragment_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var v = sender as TextInputView;
            switch (v.Id)
            {
                case Resource.Id.textInputViewLastName:
                    _VM.LastName = e.Text.ToString();
                    break;

                case Resource.Id.textInputViewFirstName:
                    _VM.FirstName = e.Text.ToString();
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("unknown CTRL EVENT " + v.Id);
                    break;
            }
        }

        private void NameFragment_Click(object sender, System.EventArgs e)
        {
            var v = sender as Button;
            switch (v.Id)
            {
                case Resource.Id.buttonCommit:
                    _VM.Commit();
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("unknown CTRL EVENT " + v.Id);
                    break;
            }
        }

        /// <summary>
        /// VMからのイベントは循環しないように同じ値を設定しない
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("-- PropertyChanged {0} {1}", new[] { MethodBase.GetCurrentMethod().Name, e.PropertyName });
            switch (e.PropertyName)
            {
                case nameof(_VM.CanCommit):
                    View.FindViewById<Button>(Resource.Id.buttonCommit).Enabled = _VM.CanCommit;
                    break;

                case nameof(_VM.Errors):
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewLastName).Errors = _VM.Errors?[nameof(_VM.LastName)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewFirstName).Errors = _VM.Errors?[nameof(_VM.FirstName)];
                    break;

                case nameof(_VM.IsError):
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewLastName).IsError = _VM.IsError[nameof(_VM.LastName)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewFirstName).IsError = _VM.IsError[nameof(_VM.FirstName)];
                    break;

                default:
                    if (Bindings.ContainsKey(e.PropertyName))
                    {
                        var v = sender.GetType().GetProperty(e.PropertyName).GetValue(sender);
                        var c = Bindings[e.PropertyName].Control;
                        var oldValue = c.GetType().GetProperty(Bindings[e.PropertyName].ControlProperty).GetValue(c);
                        if (v.Equals(oldValue))
                        {
                            System.Diagnostics.Debug.WriteLine("SAME VALUE {0} {1}", new[] { e.PropertyName, v });
                        }
                        else
                        {
                            c.GetType().GetProperty(Bindings[e.PropertyName].ControlProperty).SetValue(c, v);
                        }
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