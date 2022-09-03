using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
namespace SandBoxWV
{
    public class HistoryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private IEnumerable<string> _items;
        public HistoryAdapter(IEnumerable<string> items)
        {
            _items = items;
        }

        public override int ItemCount => _items.Count();

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as HistoryViewHolder;
            vh.Title.Text = _items.ElementAt(position);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item, parent, false);
            var vh = new HistoryViewHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
    public class HistoryViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        public HistoryViewHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Title = itemview.FindViewById<TextView>(Resource.Id.title_id);
            itemview.Click += (sender, e) => listener(base.Position);
        }
    }

}