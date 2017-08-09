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
  public class BucketsAdapter : RecyclerView.Adapter
  {
    private List<String> bucketNames, bitmapList;
    private Context context;
    public event EventHandler<int> ItemClick;

    public class MyViewHolder : RecyclerView.ViewHolder
    {
      public TextView title;
      public ImageView thumbnail;

      public MyViewHolder(View view, Action<int> listener) : base(view)
      {
        title = (TextView)view.FindViewById(Resource.Id.title);
        thumbnail = (ImageView)view.FindViewById(Resource.Id.image);
        thumbnail.Click += (sender, e) => listener(Position);
      }
    }

    public BucketsAdapter(List<string> bucketNames, List<string> bitmapList, Context context)
    {
      this.bucketNames = bucketNames;
      this.bitmapList = bitmapList;
      this.context = context;
    }


    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
      var vHolder = holder as MyViewHolder;
      if (vHolder == null)
        return;

      vHolder.title.Text = bucketNames[position];
      Glide.With(context).Load("file://" + bitmapList[position]).Override(300, 300).Into(vHolder.thumbnail);
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
      View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.album_item, parent, false);
      return new MyViewHolder(itemView, OnClick);
    }

    public override int ItemCount => bucketNames.Count;

    private void OnClick(int position)
    {
      ItemClick?.Invoke(this, position);
    }
  }
}