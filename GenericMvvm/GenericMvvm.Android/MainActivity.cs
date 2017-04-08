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

namespace GenericMvvm.Droid
{
	[Activity (Label = "GenericMvvm.Android", MainLauncher = true, Icon = "@drawable/icon", Theme ="@style/AppTheme")]
	public class MainActivity : Activity
	{
        const string FORMAT = "----ActivityEvent---- {0}";

        private BizLogic _BizLogic;

        private MainViewModel _VM;

        private TextView _TextViewFooter;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _TextViewFooter = FindViewById<TextView>(Resource.Id.textViewFooter);

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
            ActionBar.Title = _VM.Title;

            // バインド（イベント辞書）
            Dictionary<string, BindingInfo> bindings = new Dictionary<string, BindingInfo>();
            bindings.Add(nameof(_VM.Footer), new BindingInfo() { Control = _TextViewFooter, ControlProperty = nameof(_TextViewFooter.Text) });
            bindings.Add(nameof(_VM.Title), new BindingInfo() { Control = ActionBar, ControlProperty = nameof(ActionBar.Title) });

            _VM.PropertyChanged += (s, e) =>
            {
                //var vm = s as MainViewModel;
                if (bindings.ContainsKey(e.PropertyName))
                {
                    var v = s.GetType().GetProperty(e.PropertyName).GetValue(s);

                    var c = bindings[e.PropertyName].Control;
                    c.GetType().GetProperty(bindings[e.PropertyName].ControlProperty).SetValue(c, v);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Unknown property " + e.PropertyName);
                }
            };

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
    }
}


