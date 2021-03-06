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
            return inflater.Inflate(Resource.Layout.Name, container, false);
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
        /// </summary>
        public override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnResume();

            // ビューモデル生成
            _VM = _MainActivity.BizLogic.GetViewModel<NameViewModel>();

            // カスタムコントロールバインド情報
            _TextInputViewBind = new TextInputViewBind(View, _VM);
            _TextInputViewBind.Add(new TextInputViewBind.Info
            {
                PropName = nameof(_VM.LastName),
                ResId = Resource.Id.textInputViewLastName,
                InputType = Android.Text.InputTypes.ClassText,
                Hint = _VM.LastNameTitle
            });
            _TextInputViewBind.Add(new TextInputViewBind.Info
            {
                PropName = nameof(_VM.FirstName),
                ResId = Resource.Id.textInputViewFirstName,
                InputType = Android.Text.InputTypes.ClassText,
                ImeOption = Android.Views.InputMethods.ImeAction.Done,
                Hint = _VM.FirstNameTitle
            });

            // VMイベント
            _VM.PropertyChanged += _VM_PropertyChanged;

            // コントロールイベント
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click += NameFragment_Click;

            // 固定値設定
            View.FindViewById<TitleTextView>(Resource.Id.titleTextViewDescription).Text = _VM.Description;
            View.FindViewById<Button>(Resource.Id.buttonCommit).Text = _VM.CommitLabel;

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
            View.FindViewById<Button>(Resource.Id.buttonCommit).Click -= NameFragment_Click;

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
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// VMからの状態変化通知
        /// VMからのイベントは循環しないように同じ値を設定しない
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("-- NameFragment {0} {1}", new[] { MethodBase.GetCurrentMethod().Name, e.PropertyName });
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