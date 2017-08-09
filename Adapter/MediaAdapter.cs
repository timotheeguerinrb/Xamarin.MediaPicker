using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;

namespace MediaPickerLibrary.Adapter
{
  public class MyViewHolder : RecyclerView.ViewHolder
  {
    public ImageView thumbnail, check;

    public MyViewHolder(View view, Action<int> listener) : base(view)
    {
      thumbnail = (ImageView)view.FindViewById(Resource.Id.image);
      check = (ImageView)view.FindViewById(Resource.Id.image2);
      thumbnail.Click += (sender, e) => listener(Position);
    }
  }

  public class MediaAdapter : RecyclerView.Adapter
  {
    private List<string> bitmapList;
    private List<bool> selected;
    private Context context;
    public event EventHandler<int> ItemClick;


    public MediaAdapter(List<string> bitmapList, List<bool> selected, Context context)
    {
      this.bitmapList = bitmapList;
      this.context = context;
      this.selected = selected;
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
      var viewHolder = (holder as MyViewHolder);
      if (viewHolder == null)
        return;

      Glide.With(context).Load("file://" + bitmapList[position]).Override(153, 160).DontAnimate().SkipMemoryCache(true).Into(viewHolder.thumbnail);

      if (selected[position].Equals(true))
      {
        viewHolder.check.Visibility = ViewStates.Visible;
        viewHolder.check.SetAlpha(150);
      }
      else
      {
        viewHolder.check.Visibility = ViewStates.Gone;
      }
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
      View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.media_item, parent, false);
      return new MyViewHolder(itemView, OnClick);
    }

    public override int ItemCount => bitmapList.Count;

    private void OnClick(int position)
    {
      ItemClick?.Invoke(this, position);
    }
  }
}