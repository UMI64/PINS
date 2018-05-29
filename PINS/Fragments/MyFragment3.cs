using Android.Views;
using Android.Widget;
using Android.OS;
using Fragment = Android.Support.V4.App.Fragment;
namespace PINS.Fragments
{
    public class MyFragment3 : Fragment
    {
        private string content { get; set; }
        public MyFragment3(string content)
        {
            this.content = content;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.M2, container, false);
            TextView txt_content = (TextView)view.FindViewById(Resource.Id.txt_content);
            txt_content.Text = "页面3" + content;
            return view;
        }
    }
}