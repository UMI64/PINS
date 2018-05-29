using System;
using System.Collections.Generic;
using Android.Views;
using Android.Support.V4.App;
using Fragment = Android.Support.V4.App.Fragment;
namespace PINS.Fragments
{
    public class MyFragmentPagerAdapter : FragmentPagerAdapter
    {
        private const int TabItemCount = 3;
        private MyFragment1 myFragment1 = null;
        private MyFragment2 myFragment2 = null;
        private MyFragment3 myFragment3 = null;
        public MainActivity MainActivity { get; set; }
        private readonly List<Tuple<string, Type>> tabList = new List<Tuple<string, Type>>();
        public MyFragmentPagerAdapter(FragmentManager fm, int TabItemCount) : base(fm)
        {
            AddTab<MyFragment1>("热点");
            AddTab<MyFragment2>("社会");
            AddTab<MyFragment3>("体育");
        }
        public void AddTab<T>(string title)
        {
            tabList.Add(Tuple.Create(title, typeof(T)));
        }
        public override int Count
        {
            get
            {
                return TabItemCount;
            }
        }
        public new string GetPageTitle(int position)
        {
            return tabList[position].Item1;
        }
        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            return base.InstantiateItem(container, position);
        }
        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object objectValue)
        {
            base.DestroyItem(container, position, objectValue);
        }
        public override Fragment GetItem(int position)
        {
            var type = tabList[position].Item2;
            //var retFragment = new ProjectD.Fragments.MyFragment1("aaa");
            var retFragment = Activator.CreateInstance(type,"haha") as Fragment;
            return retFragment;
        }
    }
}