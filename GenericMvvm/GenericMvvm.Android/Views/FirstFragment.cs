using Android.Content;
using Android.OS;
using Android.Views;
using System.Reflection;
using Android.Support.V4.App;

namespace GenericMvvm.Droid
{
    public class FirstFragment : Fragment
    {
        const string FORMAT = "----FirstFragmentEvent---- {0}";

        public override void OnCreate(Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            return inflater.Inflate(Resource.Layout.First, container, false);
        }

        public override void OnAttach(Context context)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnAttach(context);
        }

        public override void OnDetach()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDetach();
        }

        public override void OnResume()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnResume();
        }

        public override void OnPause()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnPause();
        }

        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            base.OnDestroy();
        }
    }
}