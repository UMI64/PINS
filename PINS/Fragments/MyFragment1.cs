using Android.OS;
using Android.Views;
using Android.Widget;
using Fragment = Android.Support.V4.App.Fragment;
namespace PINS.Fragments
{
    public class MyFragment1 : Fragment
    {
        private string content { get; set; }
        public MyFragment1(string content)
        {
            this.content = content;
        }
        public override View OnCreateView(LayoutInflater inflate, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflate.Inflate(Resource.Layout.M1, container, false);
            TextView txt_content = view.FindViewById<TextView>(Resource.Id.txt_content);
            txt_content.Text = "页面1"+content;
            return view;
        }
    }
}