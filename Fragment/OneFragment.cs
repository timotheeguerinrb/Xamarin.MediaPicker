using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using MediaPickerLibrary.Adapter;
using String = System.String;

namespace MediaPickerLibrary.Fragment
{
  public class OneFragment : Android.Support.V4.App.Fragment
  {
    private static RecyclerView recyclerView;
    private BucketsAdapter mAdapter;

    private string[] projection = { MediaStore.Images.Media.InterfaceConsts.BucketDisplayName, MediaStore.Images.Media.InterfaceConsts.Data };
    private string[] projection2 = { MediaStore.Images.Media.InterfaceConsts.DisplayName, MediaStore.Images.Media.InterfaceConsts.Data };

    private List<string> bucketNames = new List<string>();
    private List<string> bitmapList = new List<string>();
    public static List<string> imagesList = new List<string>();
    public static List<bool> selected = new List<bool>();


    public override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);

      //Bucket names reloaded
      bitmapList.Clear();
      imagesList.Clear();
      bucketNames.Clear();
      GetPicBuckets();
    }

    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
      // Inflate the layout for this fragment
      View v = inflater.Inflate(Resource.Layout.fragment_one, container, false);
      recyclerView = (RecyclerView)v.FindViewById(Resource.Id.recycler_view);
      PopulateRecyclerView();
      return v;
    }

    private void PopulateRecyclerView()
    {
      mAdapter = new BucketsAdapter(bucketNames, bitmapList, Context);
      mAdapter.ItemClick += OnItemClick;
      RecyclerView.LayoutManager mLayoutManager = new GridLayoutManager(Context, 3);
      recyclerView.SetLayoutManager(mLayoutManager);
      recyclerView.SetItemAnimator(new DefaultItemAnimator());
      recyclerView.SetAdapter(mAdapter);

      mAdapter.NotifyDataSetChanged();
    }

    void OnItemClick(object sender, int position)
    {
      GetPictures(bucketNames[position]);
      Intent intent = new Intent(Context, typeof(OpenGallery));
      intent.PutExtra("FROM", "Images");
      StartActivity(intent);
    }

    public void GetPicBuckets()
    {
      var cursor = Application.Context.ContentResolver.Query(MediaStore.Images.Media.ExternalContentUri,
        projection,
        null,
        null,
        MediaStore.Images.Media.InterfaceConsts.DateAdded);

      List<string> bucketNamesTEMP = new List<string>();
      List<string> bitmapListTEMP = new List<string>();

      HashSet<string> albumSet = new HashSet<string>();

      File file;
      if (cursor.MoveToLast())
      {
        do
        {
          if (Thread.Interrupted())
          {
            return;
          }
          String album = cursor.GetString(cursor.GetColumnIndex(projection[0]));
          String image = cursor.GetString(cursor.GetColumnIndex(projection[1]));
          file = new File(image);
          if (file.Exists() && !albumSet.Contains(album))
          {
            bucketNamesTEMP.Add(album);
            bitmapListTEMP.Add(image);
            albumSet.Add(album);
          }
        } while (cursor.MoveToPrevious());
      }
      cursor.Close();

      bucketNames.Clear();
      bitmapList.Clear();
      bucketNames.AddRange(bucketNamesTEMP);
      bitmapList.AddRange(bitmapListTEMP);
    }

    public void GetPictures(String bucket)
    {
      selected.Clear();

      var cursor = Application.Context.ContentResolver.Query(MediaStore.Images.Media.ExternalContentUri,
                                projection2,
                                MediaStore.Images.Media.InterfaceConsts.BucketDisplayName + " =?",
                                new string[] { bucket },
                                MediaStore.Images.Media.InterfaceConsts.DateAdded);

      List<String> imagesTEMP = new List<string>();
      HashSet<string> albumSet = new HashSet<string>();
      File file;

      if (cursor.MoveToLast())
      {
        do
        {
          if (Thread.Interrupted())
          {
            return;
          }
          String path = cursor.GetString(cursor.GetColumnIndex(projection2[1]));
          file = new File(path);
          if (file.Exists() && !albumSet.Contains(path))
          {
            imagesTEMP.Add(path);
            albumSet.Add(path);
            selected.Add(false);
          }
        } while (cursor.MoveToPrevious());
      }
      cursor.Close();

      imagesList.Clear();
      imagesList.AddRange(imagesTEMP);
    }

    public interface ClickListener
    {
      void onClick(View view, int position);
      void onLongClick(View view, int position);
    }

    public class GestureListener : GestureDetector.SimpleOnGestureListener
    {
      private RecyclerView rView;
      private ClickListener cListener;

      public GestureListener(RecyclerView recyclerView, ClickListener clickListener)
      {
        rView = recyclerView;
        cListener = clickListener;
      }

      public override bool OnSingleTapUp(MotionEvent e)
      {
        return true;
      }

      public override void OnLongPress(MotionEvent e)
      {
        View child = rView.FindChildViewUnder(e.GetX(), e.GetY());
        if (child != null && cListener != null)
        {
          cListener.onLongClick(child, rView.GetChildPosition(child));
        }
      }
    }

    public class RecyclerTouchListener : RecyclerView.IOnItemTouchListener
    {
      private GestureDetector gestureDetector;
      private ClickListener clickListener;

      public RecyclerTouchListener(Context context, RecyclerView recyclerView, ClickListener clickListener)
      {
        this.clickListener = clickListener;
        gestureDetector = new GestureDetector(context, new GestureListener(recyclerView, clickListener));
      }

      public void Dispose()
      {
      }

      public IntPtr Handle { get; }

      public void OnLongPress(MotionEvent e)
      {
        View child = recyclerView.FindChildViewUnder(e.GetX(), e.GetY());
        if (child != null && clickListener != null)
        {
          clickListener.onLongClick(child, recyclerView.GetChildPosition(child));
        }
      }

      public bool OnInterceptTouchEvent(RecyclerView recyclerView, MotionEvent @event)
      {
        View child = recyclerView.FindChildViewUnder(@event.GetX(), @event.GetY());
        if (child != null && clickListener != null && gestureDetector.OnTouchEvent(@event))
        {
          clickListener.onClick(child, recyclerView.GetChildPosition(child));
        }
        return false;
      }

      public void OnRequestDisallowInterceptTouchEvent(bool disallow)
      {
      }

      public void OnTouchEvent(RecyclerView recyclerView, MotionEvent @event)
      {
      }
    }
  }
}