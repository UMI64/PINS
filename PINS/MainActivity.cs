using Android;
using Android.Views;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using PINS.Fragments;
using Android.Support.V4.View;
using Android.Support.Design.Widget;
using System.Threading.Tasks;
using Java.Lang.Reflect;



namespace PINS
{
    [Activity(Label = "PINS", MainLauncher = true, Theme = "@style/BaseAppTheme")]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            /*tablayout*/
            TabLayout tab = FindViewById<TabLayout>(Resource.Id.tabMain);
            tab.AddTab(tab.NewTab().SetText("小猪"));
            tab.AddTab(tab.NewTab().SetText("社会"));
            tab.AddTab(tab.NewTab().SetText("教育"));
            // 设置TabLayout的“长度”
            SetIndicator(tab, 25, 25);
            /*viewpage*/
            var viewPager = FindViewById<ViewPager>(Resource.Id.ly_content);
            var mAdapter = new MyFragmentPagerAdapter(SupportFragmentManager, tab.TabCount);
            viewPager.Adapter = mAdapter;
            viewPager.CurrentItem = 0;
            //Tab 选择事件  
            tab.TabSelected += (s, e) =>
            {
                viewPager.CurrentItem = e.Tab.Position;
            };
            viewPager.AddOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tab));//关联Tablayout+Viewpager
            /*toolbar右侧菜单*/
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            toolbar.InflateMenu(Resource.Menu.actionMenu); //填充actionMenu菜单项  
            toolbar.MenuItemClick += (s, e) => //菜单项单击事件  
            {
                if (e.Item.ItemId == Resource.Id.menu_add)
                {
                    Toast.MakeText(this, "添加菜单项", ToastLength.Short).Show();
                }
                else if (e.Item.ItemId == Resource.Id.menu_edit)
                {
                    Toast.MakeText(this, "编辑菜单项", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "搜索菜单项", ToastLength.Short).Show();
                }
            };
            /*左菜单的list*/
            ListView listview_leftMenu = FindViewById<ListView>(Resource.Id.left_menu);
            string[] menus = new string[] { "登录", "检查更新", "关于我们" };
            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleExpandableListItem1, menus);
            listview_leftMenu.Adapter = new MyCustomeAdapter(this, menus);
            listview_leftMenu.ItemClick += (o, e) =>
            {
                if (menus[e.Position] == "检查更新")
                {
                    Task startupWork = new Task(() => { ToUpDate(); });
                    startupWork.Start();
                }
            };
            /*左菜单呼出按钮*/
            DrawerLayout drawerLayout = FindViewById<DrawerLayout>(Resource.Id.left_Active);
            Button ToolBarUser_button = FindViewById<Button>(Resource.Id.ToolBarUser_button);
            ToolBarUser_button.Click += (o, e) =>
            {
                drawerLayout.OpenDrawer((int)GravityFlags.Start);
            };
            /*登录按钮*/
            Button LoginOrSign = FindViewById<Button>(Resource.Id.LoginOrSign);
            LoginOrSign.Click += (o, e) =>
            {
                Task startupWork = new Task(() => { ToLoginOrSign(); });
                startupWork.Start();
            };
            /*设置状态栏*/
            StatusBarUtil.SetColorStatusBar(this);
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }
        /*
         * 转到登录界面
         */
        private async void ToLoginOrSign()
        {
            await Task.Delay(10);
            var MyActive = new Intent(this, typeof(SignLogIn));
            StartActivity(MyActive);
        }
        /*
         * 转到升级界面
         */
        private async void ToUpDate()
        {
            await Task.Delay(10);
            var MyActive = new Intent(this, typeof(UpdateActivity));
            StartActivity(MyActive);
        }
        /*
         * 透明导航栏或者状态栏
         */
        public class StatusBarUtil
        {
            private static View _statusBarView;
            /// <summary>
            /// 设置颜色状态栏
            /// </summary>
            /// <param name="activity"></param>
            public static void SetColorStatusBar(Activity activity)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    var color = activity.Resources.GetColor(Resource.Color.primary);
                    //清除透明状态栏，使内容不再覆盖状态栏  
                    activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    activity.Window.SetStatusBarColor(color);
                    //透明导航栏部分手机导航栏不是虚拟的
                    //activity.Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
                    //activity.Window.SetNavigationBarColor(color);
                }
                else if (Build.VERSION.SdkInt == BuildVersionCodes.Kitkat)
                {
                    SetKKStatusBar(activity, Resource.Color.primary);
                }
            }
            //设置透明状态栏，android4.4以上都支持透明化状态
            public static void SetTransparentStausBar(Activity activity)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
                {
                    //状态栏透明  
                    activity.Window.AddFlags(WindowManagerFlags.TranslucentStatus);
                    //透明导航栏  
                    activity.Window.AddFlags(WindowManagerFlags.TranslucentNavigation);
                }
            }
            public static void SetKKStatusBar(Activity activity, int statusBarColor)
            {
                SetTransparentStausBar(activity);//先透明化("去掉"状态栏)
                ViewGroup contentView = activity.FindViewById<ViewGroup>(Android.Resource.Id.Content);
                _statusBarView = contentView.GetChildAt(0);
                //防止重复添加statusBarView
                if (_statusBarView != null && _statusBarView.MeasuredHeight == GetStatusBarHeight(activity))
                {
                    _statusBarView.SetBackgroundColor(activity.Resources.GetColor(statusBarColor));
                    return;
                }
                _statusBarView = new View(activity);
                ViewGroup.LayoutParams lp = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, GetStatusBarHeight(activity));
                _statusBarView.SetBackgroundColor(activity.Resources.GetColor(statusBarColor));//填充的到状态栏的view设置颜色
                contentView.AddView(_statusBarView, lp);
            }
            private static int GetStatusBarHeight(Context context)
            {
                int resourceId = context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
                return context.Resources.GetDimensionPixelSize(resourceId);
            }
        }
        /*
         * 左菜单的listview适配器
         */
        public class MyCustomeAdapter : BaseAdapter<string>
        {
            string[] items;
            Activity activity;

            public MyCustomeAdapter(Activity context, string[] values) : base()
            {
                activity = context;
                items = values;
            }

            public override string this[int position]
            {
                get { return items[position]; }
            }

            public override int Count
            {
                get { return items.Length; }
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View v = convertView;
                if (v == null) v = activity.LayoutInflater.Inflate(Resource.Layout.LeftListview, parent, false);

                var MainText = v.FindViewById<TextView>(Resource.Id.leftmenutext);

                MainText.Text = items[position];

                return v;
            }
        }
        /*
        设置Tablayout元素的外边距 
        */
        // 具体方法（通过反射的方式）
        public void SetIndicator(TabLayout tabs, int leftDip, int rightDip)
        {
            Java.Lang.Class tabLayout = tabs.Class;
            Field tabStrip = null;
            try
            {
                tabStrip = tabLayout.GetDeclaredField("mTabStrip");
            }
            catch (Java.Lang.NoSuchFieldException e)
            {
                //e.printStackTrace();
            }

            tabStrip.Accessible = true;
            LinearLayout llTab = null;
            try
            {
                llTab = (LinearLayout)tabStrip.Get(tabs);
            }
            catch (Java.Lang.IllegalAccessException e)
            {
                //e.printStackTrace();
            }

            int left = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, leftDip, Resources.DisplayMetrics);
            int right = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, rightDip, Resources.DisplayMetrics);

            for (int i = 0; i < llTab.ChildCount; i++)
            {
                View child = llTab.GetChildAt(i);
                //child.GetPadding(0, 0, 0, 0);

                var p = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.MatchParent, 1);
                p.LeftMargin = left;
                p.RightMargin = right;
                child.LayoutParameters =p;
                child.Invalidate();
            }
        }



    }
}
        /*
        //侧边视图已经打开时调用
        public void OnDrawerClosed(View drawerView)
        {
            System.Diagnostics.Debug.Write("侧边视图已经关闭");
        }
        //侧边视图已经关闭时调用
        public void OnDrawerOpened(View drawerView)
        {
            System.Diagnostics.Debug.Write("侧边视图已经打开");
        }
        //侧边视图正在滑动时调用
        public void OnDrawerSlide(View drawerView, float slideOffset)
        {
            System.Diagnostics.Debug.Write("侧边视图已经关闭");
        }
        //侧边视图滑动状态发生改变时被调用
        public void OnDrawerStateChanged(int newState)
        {
            System.Diagnostics.Debug.Write("drawer的状态" + newState);
            //状态值STATE_IDLE:0是闲置状态,STATE_DRAGGING:1拖拽状态,STATE_SETTLING：2(固定状态)
        }
        
    }
}
*/

