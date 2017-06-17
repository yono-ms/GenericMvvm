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

namespace GenericMvvm.Droid
{
    class ConfirmAdapter : RecyclerView.Adapter
    {
        IEnumerable<ConfirmViewModel.ConfirmCell> ConfirmList;

        public ConfirmAdapter(IEnumerable<ConfirmViewModel.ConfirmCell> confirmList)
        {
            ConfirmList = confirmList;
        }

        public override int ItemCount => ConfirmList.Count();

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = ConfirmList.ElementAt(position);
            var vh = holder as ConfirmAdapterViewHolder;
            vh.Title.Text = item.Title;
            vh.Value.Text = item.Value;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var inflater = LayoutInflater.From(parent.Context);
            var itemView = inflater.Inflate(Resource.Layout.ConfirmCell, parent, false);
            var vh = new ConfirmAdapterViewHolder(itemView);
            return vh;
        }
    }

    public class ConfirmAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        public TextView Value { get; set; }

        public ConfirmAdapterViewHolder(View itemView) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.textViewTitle);
            Value = itemView.FindViewById<TextView>(Resource.Id.textViewValue);
        }
    }
}