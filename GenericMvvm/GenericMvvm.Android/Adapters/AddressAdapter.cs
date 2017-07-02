using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GenericMvvm.Droid
{
    class AddressAdapter : RecyclerView.Adapter
    {
        public event EventHandler<AddressAdapterClickEventArgs> ItemClick;
        public event EventHandler<AddressAdapterClickEventArgs> ItemLongClick;

        IEnumerable<ZipCloudResponse.result> items;
        private int _SelectedIndex;

        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set
            {
                var oldIndex = _SelectedIndex;
                _SelectedIndex = value;

                NotifyItemChanged(_SelectedIndex);
                NotifyItemChanged(oldIndex);
            }
        }

        public AddressAdapter(IEnumerable<ZipCloudResponse.result> data)
        {
            items = data;
            _SelectedIndex = -1;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.AddressCell;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new AddressAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            System.Diagnostics.Debug.WriteLine("AddressAdapter {0} {1}", new[] { MethodBase.GetCurrentMethod().Name, position.ToString()});

            var item = items.ElementAt(position);

            // Replace the contents of the view with that element
            var holder = viewHolder as AddressAdapterViewHolder;
            holder.Address.Text = item.address1 + item.address2 + item.address3;
            holder.AddressKana.Text = item.kana1 + item.kana2 + item.kana3;
            holder.ItemView.Selected = (_SelectedIndex == position) ? true : false;
        }

        public override int ItemCount => (items != null) ? items.Count() : 0;

        void OnClick(AddressAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(AddressAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class AddressAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView Address { get; set; }
        public TextView AddressKana { get; set; }

        public AddressAdapterViewHolder(View itemView, Action<AddressAdapterClickEventArgs> clickListener,
                            Action<AddressAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            Address = itemView.FindViewById<TextView>(Resource.Id.textViewAddress);
            AddressKana = itemView.FindViewById<TextView>(Resource.Id.textViewAddressKana);

            itemView.Click += (sender, e) => clickListener(new AddressAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new AddressAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class AddressAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}