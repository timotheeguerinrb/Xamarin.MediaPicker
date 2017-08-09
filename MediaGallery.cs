using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using MediaPickerLibrary.Fragment;

namespace MediaPickerLibrary
{
  [Activity]
  public class MediaGallery : AppCompatActivity
  {
    private TabLayout tabLayout;
    private ViewPager viewPager;

    protected override void OnCreate(Bundle savedInstanceState)
    {
      base.OnCreate(savedInstanceState);

      Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
      SetContentView(Resource.Layout.activity_gallery);

      var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
      SetSupportActionBar(toolbar);
      toolbar.SetNavigationIcon(Resource.Drawable.arrow_back);

      toolbar.NavigationClick += (sender, args) =>
      {
        OnBackPressed();
      };

      var fab = (FloatingActionButton)FindViewById(Resource.Id.fab);
      fab.Click += (sender, args) =>
      {
        ReturnResult();
      };

      var title = Intent.GetStringExtra("title") ?? "Gallery";
      var maxNbrSelection = Intent.GetIntExtra("maxNumberSelection",1);
      Title = title;

      viewPager = (ViewPager)FindViewById(Resource.Id.viewpager);
      SetupViewPager(viewPager);
      tabLayout = (TabLayout)FindViewById(Resource.Id.tabs);
      tabLayout.SetupWithViewPager(viewPager);

      OpenGallery.maxNbrSelection = maxNbrSelection;
      OpenGallery.selected.Clear();
      OpenGallery.imagesSelected.Clear();
    }

    protected override void OnPostResume()
    {
      base.OnPostResume();

      if (OpenGallery.imagesSelected.Count > 0 && OpenGallery.isFloatingBtnPressed)
        ReturnResult();
    }

    //This method set up the tab view for images and videos
    private void SetupViewPager(ViewPager viewPager)
    {
      ViewPagerAdapter adapter = new ViewPagerAdapter(SupportFragmentManager);
      adapter.addFragment(new OneFragment(), "Images");
      //adapter.addFragment(new TwoFragment(), "Videos");
      viewPager.Adapter = adapter;
    }

    private void ReturnResult()
    {
      Intent returnIntent = new Intent();
      returnIntent.PutStringArrayListExtra("result", OpenGallery.imagesSelected);
      SetResult(Result.Ok, returnIntent);
      Finish();
    }

    class ViewPagerAdapter : FragmentPagerAdapter
    {
      private List<Android.Support.V4.App.Fragment> mFragmentList = new List<Android.Support.V4.App.Fragment>();
      private List<string> mFragmentTitleList = new List<string>();

      public ViewPagerAdapter(Android.Support.V4.App.FragmentManager fm) : base(fm)
      {
      }

      public void addFragment(Android.Support.V4.App.Fragment fragment, string title)
      {
        mFragmentList.Add(fragment);
        mFragmentTitleList.Add(title);
      }

      public override int Count => mFragmentList.Count;

      public override Android.Support.V4.App.Fragment GetItem(int position)
      {
        return mFragmentList[position];
      }
    }
  }
}