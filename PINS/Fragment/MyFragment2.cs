using Android.Views;
using Android.Widget;
using Android.OS;
using Fragment = Android.Support.V4.App.Fragment;
namespace ProjectD.Fragments
{
    public class MyFragment2 : Fragment
    {
        private string content { get; set; }
        public MyFragment2(string content)
        {
            this.content = content;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.M1, container, false);
            TextView txt_content = (TextView)view.FindViewById(Resource.Id.txt_content);
            txt_content.Text = "页面2" + content;
            return view;
        }
    }
}