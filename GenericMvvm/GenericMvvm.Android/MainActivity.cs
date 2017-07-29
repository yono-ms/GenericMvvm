using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Android.Support.V7.Widget;
using System.Collections.ObjectModel;
using Android.Transitions;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Fragment = Android.Support.V4.App.Fragment;
using System.ComponentModel;

namespace GenericMvvm.Droid
{
    /// <summary>
    /// ActivityのonResumeに至るルートは５種類
    /// １．onCreate(null) >> onResume　初期起動
    /// ２．onCreate(bundle) >> onRestoreInstanceState(bundle) >> onResume　回転
    /// ３．onNewIntent(bundle) >> onRestoreInstanceState(bundle) >> onResume　リサイクルで起動
    /// ４．onRestoreInstanceState(bundle) >> onResume　onDestroyからの復帰
    /// ５．（なし） >> onResume　onStopからの復帰
    /// </summary>
	[Activity (Label = "GenericMvvm.Android", MainLauncher = true, Icon = "@drawable/icon", Theme ="@style/AppTheme", WindowSoftInputMode = SoftInput.AdjustPan)]
	public class MainActivity : AppCompatActivity
	{
        const string FORMAT = "----ActivityEvent---- {0}";

        private BizLogic _BizLogic;
        public BizLogic BizLogic { get { return _BizLogic; } }

        private MainViewModel _VM;

        bool kickStart = false;

        protected override void OnCreate (Bundle bundle)
		{
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnCreate (bundle);
            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            if (bundle == null)
            {
                System.Diagnostics.Debug.WriteLine("START NO BUNDLE");

                // 初期起動／レイアウト復元なし／インスタンス変数なし

                kickStart = true;

                // BizLogicが無くてもUIだけで表示できる初期ページを表示
                var firstFragment = new FirstFragment();
                SupportFragmentManager.BeginTransaction()
                    .Add(Resource.Id.frameLayoutContent, firstFragment)
                    .Commit();

                // インスタンス変数を復元する
                RestoreInstanceState();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("START BUNDLE");

                // リサイクル／レイアウト復元される／インスタンス変数なし

                kickStart = false;

                // onDestroy後にリサイクルされた場合はonRestartとなるため
                // 一括してonRestoreInstanceStateでインスタンス変数を復元する
            }

		}
        /// <summary>
        /// 同期でインスタンス変数を復元する
        /// </summary>
        private void RestoreInstanceState()
        {
            System.Diagnostics.Debug.WriteLine("RestoreInstanceState START");

            // この構成ではBizLogicの状態だけでやりなす必要があるかを判断できる
            if (_BizLogic == null)
            {
                System.Diagnostics.Debug.WriteLine("BizLogic復元ルート");

                // アプリケーションを復元する
                _BizLogic = BizLogic.LoadBizLogic(new NativeCallAndroid(this));
            }
            else
            {
                // ここを通るのはおかしい
                System.Diagnostics.Debug.WriteLine("BizLogic生存ルート !!!!!!!!");
            }

            System.Diagnostics.Debug.WriteLine("RestoreInstanceState END");
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    _VM.GoBack();
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("MainActivity Unknown option item " + item.ItemId);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void _VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = sender as MainViewModel;
            switch (e.PropertyName)
            {
                case nameof(vm.Title):
                    SupportActionBar.Title = vm.Title;
                    break;

                case nameof(vm.Footer):
                    FindViewById<TextView>(Resource.Id.textViewFooter).Text = vm.Footer;
                    break;

                case nameof(vm.ObjectErrors):
                    // アダプターの入れ替え
                    var adapter = new ErrorAdapter(vm.ObjectErrors);
                    FindViewById<RecyclerView>(Resource.Id.recyclerViewObjectErrors).SetAdapter(adapter);
                    break;

                case nameof(vm.ShowProgress):
                    var view = FindViewById(Resource.Id.layoutGuard);
                    view.Visibility = vm.ShowProgress ? ViewStates.Visible : ViewStates.Invisible;
                    break;

                case nameof(vm.ShowBackButton):
                    SupportActionBar.SetDisplayHomeAsUpEnabled(vm.ShowBackButton);
                    break;

                case nameof(vm.Errors):
                case nameof(vm.IsError):
                case nameof(vm.CanCommit):
                    // ここは何もしない
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("MainActivity Unknown property " + e.PropertyName);
                    break;
            }
        }

        /// <summary>
        /// 画面遷移
        /// </summary>
        /// <param name="page"></param>
        /// <param name="forward"></param>
        public void NavigateTo(Type page, bool forward)
        {
            var next = Activator.CreateInstance(page) as Fragment;
            var prev = SupportFragmentManager.FindFragmentById(Resource.Id.frameLayoutContent);

            // ソースで書かないとトランジッションアニメーションが効かない
            var ti = TransitionInflater.From(this);
            next.EnterTransition = ti.InflateTransition(forward ? Resource.Transition.enter_transition : Resource.Transition.exit_transition);
            prev.ExitTransition = ti.InflateTransition(forward ? Resource.Transition.exit_transition : Resource.Transition.enter_transition);
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frameLayoutContent, next).Commit();
        }
        protected override void OnNewIntent(Intent intent)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnNewIntent(intent);
        }
        protected override void OnRestart()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnRestart();
        }
        protected override void OnStart()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnStart();
        }
        protected override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnResume();

            // Mainに限りBizLogic生成を待ってからUI構築を行う必要がある
            // onCreate/onRestoreInstanceStateでBizLogicの永続性が確保されるため
            // このイベントは必ずBizLogic生成後となる
            _VM = _BizLogic.GetViewModel<MainViewModel>();

            // VMイベント
            _VM.PropertyChanged += _VM_PropertyChanged;

            // コントロールイベントは戻るボタンしかない

            // 初期値設定
            FindViewById<TextView>(Resource.Id.textViewFooter).Text = _VM.Footer;
            SupportActionBar.Title = _VM.Title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(_VM.ShowBackButton);
            // 初期値設定 Recycler
            var adapter = new ErrorAdapter(_VM.ObjectErrors);
            var recycler = FindViewById<RecyclerView>(Resource.Id.recyclerViewObjectErrors);
            recycler.SetLayoutManager(new LinearLayoutManager(this));
            recycler.SetAdapter(adapter);

            // バインドし終わったら起動する
            if (kickStart)
            {
                kickStart = false;
                // 遅延処理はハンドラーを使わないとうまく動かない
                new Handler().PostDelayed(() => { _VM.KickStart(); }, 300);
            }
        }
        protected override void OnPause()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnPause();

            // バインド解除する
            _VM.PropertyChanged -= _VM_PropertyChanged;
            _VM = null;
        }
        protected override void OnStop()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnStop();
        }
        protected override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDestroy();
        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnSaveInstanceState(outState);

            // 保存タイミングを知るイベントだけ必要
            if (!_BizLogic.CurrentPage.Equals("Finish"))
            {
                // 最終画面の場合はすでに消されているので保存させてはいけない
                _BizLogic.SaveBizLogic();
            }
        }
        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnRestoreInstanceState(savedInstanceState);

            // インスタンス変数を復元する
            RestoreInstanceState();
        }
        /// <summary>
        /// カスタムダイアログフラグメントのボタンイベント
        /// ボタン押下時にフラグメントから実行する
        /// </summary>
        /// <param name="label">押されたボタンのラベル</param>
        public void OnDialogClick(string label)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            DialogClick?.Invoke(this, new DialogEventArgs() { Label = label });
        }
        /// <summary>
        /// カスタムダイアログフラグメントのボタンイベント引数
        /// </summary>
        public class DialogEventArgs : EventArgs
        {
            public string Label { get; set; }
        }
        /// <summary>
        /// カスタムダイアログフラグメントのボタンイベントハンドラ
        /// </summary>
        public event EventHandler<DialogEventArgs> DialogClick;
    }
}


