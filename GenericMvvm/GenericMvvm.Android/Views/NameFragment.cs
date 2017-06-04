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
    public class NameFragment : Fragment
    {
        const string FORMAT = "----NameFragmentEvent---- {0}";

        MainActivity _MainActivity;
        NameViewModel _VM;
        Dictionary<string, BindingInfo> Bindings;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            return inflater.Inflate(Resource.Layout.Name, container, false);

            // テーマに設定したトランジッションアニメーションが効かないのでここでセットしてみるが無駄
            //var wrapper = new ContextThemeWrapper(Activity, Resource.Style.AppTheme);
            //var li = inflater.CloneInContext(wrapper);
            //return li.Inflate(Resource.Layout.Name, container, false);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            _MainActivity = context as MainActivity;
        }

        public override void OnDetach()
        {
            base.OnDetach();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            _MainActivity = null;
        }

        public override void OnResume()
        {
            base.OnResume();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

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
            _VM.PropertyChanged += VM_PropertyChanged;

            // コントロールイベント
            View.FindViewById<TextInputView>(Resource.Id.textInputViewLastName).TextChanged += NameFragment_TextChanged;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewFirstName).TextChanged += NameFragment_TextChanged;

            // TwoWay初期値設定
            //BindingInfo.Start(_VM, Bindings);
        }

        public override void OnPause()
        {
            base.OnPause();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            // ここでバインド解除する
            View.FindViewById<TextInputView>(Resource.Id.textInputViewLastName).TextChanged -= NameFragment_TextChanged;
            View.FindViewById<TextInputView>(Resource.Id.textInputViewFirstName).TextChanged -= NameFragment_TextChanged;

            _VM.PropertyChanged -= VM_PropertyChanged;

            Bindings.Clear();

            _VM = null;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
        }

        /// <summary>
        /// コントロールからのイベント
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

        /// <summary>
        /// VMからのイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_VM.CanCommit):
                    View.FindViewById<Button>(Resource.Id.buttonCommit).Enabled = _VM.CanCommit;
                    break;

                case nameof(_VM.Errors):
                    System.Diagnostics.Debug.WriteLine("-- PropertyChanged " + e.PropertyName);
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewLastName).Errors = _VM.Errors?[nameof(_VM.LastName)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewFirstName).Errors = _VM.Errors?[nameof(_VM.FirstName)];
                    break;

                case nameof(_VM.IsError):
                    System.Diagnostics.Debug.WriteLine("-- PropertyChanged " + e.PropertyName);
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewLastName).IsError = _VM.IsError[nameof(_VM.LastName)];
                    View.FindViewById<TextInputView>(Resource.Id.textInputViewFirstName).IsError = _VM.IsError[nameof(_VM.FirstName)];
                    break;

                default:
                    if (Bindings.ContainsKey(e.PropertyName))
                    {
                        System.Diagnostics.Debug.WriteLine("-- PropertyChanged " + e.PropertyName);
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