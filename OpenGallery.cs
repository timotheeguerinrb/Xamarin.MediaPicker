using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using MediaPickerLibrary.Adapter;
using MediaPickerLibrary.Fragment;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace MediaPickerLibrary
{
  [Activity]
  public class OpenGallery : AppCompatActivity
  {
    private RecyclerView recyclerView;
    private MediaAdapter mAdapter;
    private List<string> mediaList = new List<string>();
    public static List<bool> selected = new List<bool>();
    public static List<string> imagesSelected = new List<string>();
    public static string parent;
    public static bool isFloatingBtnPressed;
    public static int maxNbrSelection = 1;

    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);

      Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
      SetContentView(Resource.Layout.activity_open_gallery);
      Toolbar toolbar = (Toolbar)FindViewById(Resource.Id.toolbar);
      SetSupportActionBar(toolbar);
      FloatingActionButton fab = (FloatingActionButton)FindViewById(Resource.Id.fab);

      fab.Click += (sender, args) =>
      {
        isFloatingBtnPressed = true;
        Finish();
      };

      toolbar.SetNavigationIcon(Resource.Drawable.arrow_back);
      Title = "Select your photo";

      if (imagesSelected.Count > 0)
      {
        Title = imagesSelected.Count + " Selected";
      }

      toolbar.NavigationClick += (sender, args) =>
      {
        isFloatingBtnPressed = false;
        OnBackPressed();
      };

      recyclerView = (RecyclerView)FindViewById(Resource.Id.recycler_view);
      parent = Intent.GetStringExtra("FROM");
      mediaList.Clear();
      selected.Clear();
      if (parent.Equals("Images"))
      {
        mediaList.AddRange(OneFragment.imagesList);
        selected.AddRange(OneFragment.selected);
      }
      else
      {
        //mediaList.AddRange(TwoFragment.videosList);
        //selected.AddRange(TwoFragment.selected);
      }
      populateRecyclerView();
    }


    private void populateRecyclerView()
    {
      for (int i = 0; i < selected.Count; i++)
      {
        if (imagesSelected.Contains(mediaList[i]))
        {
          selected[i] = true;
        }
        else
        {
          selected[i] = false;
        }
      }
      mAdapter = new MediaAdapter(mediaList, selected, ApplicationContext);
      mAdapter.ItemClick += OnItemClick;
      RecyclerView.LayoutManager mLayoutManager = new GridLayoutManager(ApplicationContext, 3);
      recyclerView.SetLayoutManager(mLayoutManager);
      recyclerView.GetItemAnimator().ChangeDuration = 0;
      recyclerView.SetAdapter(mAdapter);

      void OnItemClick(object sender, int position)
      {
        if (!selected[position].Equals(true))
        {
          if (maxNbrSelection == 1 && imagesSelected.Count > 0)
          {
            var i = 0;
            foreach (var item in selected)
            {
              if (item)
              {
                selected[i] = false;
                break;
              }
              i++;
            }
            imagesSelected.Clear();
            mAdapter.NotifyItemChanged(i);
          }

          if (imagesSelected.Count == maxNbrSelection && maxNbrSelection > 1)
            return;

          imagesSelected.Add(mediaList[position]);
        }
        else
        {
          if (imagesSelected.IndexOf(mediaList[position]) != -1)
          {
            imagesSelected.RemoveAt(imagesSelected.IndexOf(mediaList[position]));
          }
        }
        selected[position] = !selected[position];
        mAdapter.NotifyItemChanged(position);
        if (imagesSelected.Count != 0)
        {
          Title = imagesSelected.Count + " Selected";
        }
        else
        {
          Title = "Select your photo";
        }
      }
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
        var child = rView.FindChildViewUnder(e.GetX(), e.GetY());
        if (child != null)
        {
          cListener?.onLongClick(child, rView.GetChildPosition(child));
        }
      }
    }

    public class RecyclerTouchListener : RecyclerView.IOnItemTouchListener
    {
      private GestureDetector gestureDetector;
      private ClickListener clickListener;

      public void Dispose() { }
      public IntPtr Handle { get; }

      public RecyclerTouchListener(Context context, RecyclerView recyclerView, ClickListener clickListener)
      {
        this.clickListener = clickListener;
        gestureDetector = new GestureDetector(context, new GestureListener(recyclerView, clickListener));
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
