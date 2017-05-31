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
            base.OnCreate(savedInstanceState);
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            //return base.OnCreateView(inflater, container, savedInstanceState);

            return inflater.Inflate(Resource.Layout.First, container, false);
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
        }

        public override void OnDetach()
        {
            base.OnDetach();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
        }

        public override void OnResume()
        {
            base.OnResume();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
        }

        public override void OnPause()
        {
            base.OnPause();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name });
        }
    }
}