using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Reflection;
using System.Threading.Tasks;

namespace GenericMvvm.Droid
{
	[Activity (Label = "GenericMvvm.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
        const string FORMAT = "----ActivityEvent---- {0}";

        BizLogic _BizLogic;

        int count = 1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            if (_BizLogic == null)
            {
                System.Diagnostics.Debug.WriteLine("BizLogic復元ルート");

                // アプリケーションを復元する
                Task.Run(async () =>
                {
                    _BizLogic = await BizLogic.LoadBizLogicAsync(new NativeCallAndroid(this));
                    Initialize();
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
            throw new NotImplementedException();
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


