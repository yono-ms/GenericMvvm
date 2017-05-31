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
using Android.Support.V7.Widget;
using System.Collections.ObjectModel;
using Android.Transitions;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Fragment = Android.Support.V4.App.Fragment;

namespace GenericMvvm.Droid
{
	[Activity (Label = "GenericMvvm.Android", MainLauncher = true, Icon = "@drawable/icon", Theme ="@style/AppTheme")]
	public class MainActivity : AppCompatActivity
	{
        const string FORMAT = "----ActivityEvent---- {0}";

        private BizLogic _BizLogic;
        public BizLogic BizLogic { get { return _BizLogic; } }

        private MainViewModel _VM;

        private TextView _TextViewFooter;
        private RecyclerView _RecyclerView;
        private ProgressBar _ProgressBar;
        private LinearLayout _LinearLayoutGuard;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _TextViewFooter = FindViewById<TextView>(Resource.Id.textViewFooter);

            _RecyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerViewObjectErrors);
            _RecyclerView.SetLayoutManager(new LinearLayoutManager(this));

            _ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            _LinearLayoutGuard = FindViewById<LinearLayout>(Resource.Id.linearLayoutGuard);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            // BizLogicが無くてもUIだけで表示できる初期ページを表示
            var firstFragment = new FirstFragment();
            // ソースで書かないとトランジッションアニメーションが効かない
            var ti = TransitionInflater.From(this);
            firstFragment.EnterTransition = ti.InflateTransition(Resource.Transition.enter_transition);
            firstFragment.ExitTransition = ti.InflateTransition(Resource.Transition.exit_transition);
            firstFragment.ReenterTransition = ti.InflateTransition(Resource.Transition.reenter_transition);
            firstFragment.ReturnTransition = ti.InflateTransition(Resource.Transition.return_transition);
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

            // バインド（初期値）
            _TextViewFooter.Text = _VM.Footer;
            SupportActionBar.Title = _VM.Title;

            // バインド（イベント辞書）
            Dictionary<string, BindingInfo> bindings = new Dictionary<string, BindingInfo>();
            bindings.Add(nameof(_VM.Footer), new BindingInfo() { Control = _TextViewFooter, ControlProperty = nameof(_TextViewFooter.Text) });
            bindings.Add(nameof(_VM.Title), new BindingInfo() { Control = SupportActionBar, ControlProperty = nameof(SupportActionBar.Title) });

            _VM.PropertyChanged += (s, e) =>
            {
                //var vm = s as MainViewModel;
                if (bindings.ContainsKey(e.PropertyName))
                {
                    var v = s.GetType().GetProperty(e.PropertyName).GetValue(s);

                    var c = bindings[e.PropertyName].Control;
                    c.GetType().GetProperty(bindings[e.PropertyName].ControlProperty).SetValue(c, v);
                }
                else if (e.PropertyName.Equals(nameof(_VM.ObjectErrors)))
                {
                    // アダプターの入れ替え
                    var adapter = new RecyclerAdapter(LayoutInflater, _VM.ObjectErrors);
                    _RecyclerView.SetAdapter(adapter);
                }
                else if (e.PropertyName.Equals(nameof(_VM.ShowProgress)))
                {
                    // コンバーターが必要
                    _LinearLayoutGuard.Visibility = _VM.ShowProgress ? ViewStates.Visible : ViewStates.Visible;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Unknown property " + e.PropertyName);
                }
            };

            // バインドし終わったら起動する
            _VM.KickStart();
        }

        /// <summary>
        /// 画面遷移
        /// </summary>
        /// <param name="page"></param>
        /// <param name="forward"></param>
        public void NavigateTo(Type page, bool forward)
        {
            var fragment = Activator.CreateInstance(page) as Fragment;
            // ソースで書かないとトランジッションアニメーションが効かない
            var ti = TransitionInflater.From(this);
            fragment.EnterTransition = ti.InflateTransition(Resource.Transition.enter_transition);
            fragment.ExitTransition = ti.InflateTransition(Resource.Transition.exit_transition);
            fragment.ReenterTransition = ti.InflateTransition(Resource.Transition.reenter_transition);
            fragment.ReturnTransition = ti.InflateTransition(Resource.Transition.return_transition);
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.frameLayoutContent, fragment).Commit();
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
        }
        protected override void OnStart()
        {
            base.OnStart();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
        }
        protected override void OnResume()
        {
            base.OnResume();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
        }
        protected override void OnPause()
        {
            base.OnPause();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
        }
        protected override void OnStop()
        {
            base.OnStop();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
        }

        class RecyclerAdapter : RecyclerView.Adapter
        {
            private LayoutInflater _LayoutInflater;
            private ObservableCollection<string> _ObjectErrors;

            public RecyclerAdapter(LayoutInflater layoutInflater, ObservableCollection<string> objectErrors)
            {
                this._LayoutInflater = layoutInflater;
                this._ObjectErrors = objectErrors;
            }

            public override int ItemCount => (_ObjectErrors == null) ? 0 : _ObjectErrors.Count;

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var h = holder as ViewHolder;
                h._TextView.Text = _ObjectErrors[position];
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                return new ViewHolder(_LayoutInflater.Inflate(Resource.Layout.ObjectErrorsCell, parent));
            }

            class ViewHolder : RecyclerView.ViewHolder
            {
                public TextView _TextView;

                public ViewHolder(View itemView) : base(itemView)
                {
                    _TextView = itemView.FindViewById<TextView>(Resource.Id.textView);
                }
            }
        }
    }
}


