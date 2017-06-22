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
        TextInputViewBind _TextInputViewBind;

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

            // ビューモデル生成
            _VM = _MainActivity.BizLogic.GetViewModel<NameViewModel>();

            // カスタムコントロールバインド情報
            _TextInputViewBind = new TextInputViewBind(View, _VM);
            _TextInputViewBind.Add(nameof(_VM.LastName), Resource.Id.textInputViewLastName, _VM.LastNameTitle);
            _TextInputViewBind.Add(nameof(_VM.FirstName), Resource.Id.textInputViewFirstName, _VM.FirstNameTitle);

            // VMイベント
            _VM.PropertyChanged += _VM_PropertyChanged;

            // コントロールイベント
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click += NameFragment_Click;

            // 固定値設定
            View.FindViewById<TitleTextView>(Resource.Id.titleTextViewDescription).Text = _VM.Description;

            // TwoWay初期値設定
            _TextInputViewBind.Start();
        }

        /// <summary>
        /// バックグラウンド移行
        /// </summary>
        public override void OnPause()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnPause();

            // ここでバインド解除する
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click += NameFragment_Click;

            _TextInputViewBind.Stop();

            _VM.PropertyChanged -= _VM_PropertyChanged;

            _TextInputViewBind = null;

            _VM = null;
        }

        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDestroy();
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

                default:
                    if (_TextInputViewBind.ContainsKey(e.PropertyName))
                    {
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