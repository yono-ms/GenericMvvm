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
	[Activity (Label = "GenericMvvm.Android", MainLauncher = true, Icon = "@drawable/icon", Theme ="@style/AppTheme")]
	public class MainActivity : AppCompatActivity
	{
        const string FORMAT = "----ActivityEvent---- {0}";

        private BizLogic _BizLogic;
        public BizLogic BizLogic { get { return _BizLogic; } }

        private MainViewModel _VM;

        protected override void OnCreate (Bundle bundle)
		{
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnCreate (bundle);
            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            // BizLogicが無くてもUIだけで表示できる初期ページを表示
            var firstFragment = new FirstFragment();
            // ソースで書かないとトランジッションアニメーションが効かない
            var ti = TransitionInflater.From(this);
            firstFragment.EnterTransition = ti.InflateTransition(Resource.Transition.enter_transition);
            firstFragment.ExitTransition = ti.InflateTransition(Resource.Transition.exit_transition);
            SupportFragmentManager.BeginTransaction().Add(Resource.Id.frameLayoutContent, firstFragment).Commit();

            // この構成ではBizLogicの状態だけでやりなす必要があるかを判断できる
            if (_BizLogic == null)
            {
                System.Diagnostics.Debug.WriteLine("BizLogic復元ルート");

                // アプリケーションを復元する
                Task.Run(async () =>
                {
                    _BizLogic = await BizLogic.LoadBizLogicAsync(new NativeCallAndroid(this));
                    RunOnUiThread(() =>
                    {
                        Initialize();
                    });
                });
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("BizLogic生存ルート");

                // アプリケーションが生きているのでUI再生成
                Initialize();
            }
		}
        /// <summary>
        /// UIの再構築
        /// </summary>
        private void Initialize()
        {
            // Mainに限りBizLogic生成を待ってからUI構築を行う
            _VM = _BizLogic.GetViewModel<MainViewModel>();

            // VMイベント
            _VM.PropertyChanged += _VM_PropertyChanged;

            // コントロールイベントは戻るボタンしかない

            // 初期値設定
            FindViewById<TextView>(Resource.Id.textViewFooter).Text = _VM.Footer;
            SupportActionBar.Title = _VM.Title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
            // 初期値設定 Recycler
            var adapter = new ErrorAdapter(_VM.ObjectErrors);
            var recycler = FindViewById<RecyclerView>(Resource.Id.recyclerViewObjectErrors);
            recycler.SetLayoutManager(new LinearLayoutManager(this));
            recycler.SetAdapter(adapter);

            // バインドし終わったら起動する
            _VM.KickStart();
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
        }
        protected override void OnPause()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnPause();
        }
        protected override void OnStop()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnStop();
        }
    }
}


