using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.ObjectModel;
using System.Reflection;

namespace GenericMvvm.Droid
{
    public class ErrorAdapter : RecyclerView.Adapter
    {
        const string FORMAT = "----ErrorAdapter---- {0} {1}";

        private ObservableCollection<string> _Errors;

        public ErrorAdapter(ObservableCollection<string> errors)
        {
            _Errors = errors;
        }

        public override int ItemCount => (_Errors == null) ? 0 : _Errors.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (ItemCount > position)
            {
                var vh = holder as ViewHolder;
                vh.Error.Text = _Errors.ElementAt(position);
            }
            else
            {
                // 異常ルート
                System.Diagnostics.Debug.WriteLine(FORMAT, new[] { MethodBase.GetCurrentMethod().Name, string.Format("!(Count={0} > position={1})", _Errors.Count, position) });
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(parent.Context);
            var itemView = inflater.Inflate(Resource.Layout.ObjectErrorsCell, parent, false);
            return new ViewHolder(itemView);
        }

        class ViewHolder : RecyclerView.ViewHolder
        {
            public TextView Error { get; set; }

            public ViewHolder(View itemView) : base(itemView)
            {
                Error = itemView.FindViewById<TextView>(Resource.Id.textView);
            }
        }
    }
}