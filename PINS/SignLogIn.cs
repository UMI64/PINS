using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content;
using HttpCommunication;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Threading.Tasks;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace PINS
{
    [Activity(Label = "登录")]
    public class SignLogIn : Activity
    {
        List<String> Users = new List<String> { };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.SignLogin);
            Button SignInButton = FindViewById<Button>(Resource.Id.SignIn);
            Button LogInButton = FindViewById<Button>(Resource.Id.LogIn);
            EditText PassWordText = FindViewById<EditText>(Resource.Id.PassWord);
            AutoCompleteTextView UserNameText = FindViewById<AutoCompleteTextView>(Resource.Id.autocomplete_country);
            /*获取数据库文件路径*/
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            /*创建链接*/
            var db = new SQLiteConnection(dbPath);
            /*创建数据表*/
            db.CreateTable<UserTable>();
            /*自动补全用户名功能在此*/
            db.Table<UserTable>();
            foreach (UserTable Count in db.Table<UserTable>().ToList())
            {
                string user = Count.UserName;
                Users.Add(user);
            }
            Users.Add("aaa");
            var adapter = new ArrayAdapter<String>(this, Resource.Layout.list_item, Users);
            UserNameText.Adapter = adapter;
            /*自动补全功能启动完毕*/

            /*登录按钮按下执行的东西*/
            SignInButton.Click += (s, arg) =>
            {
                //JsonObject JSONresult;
                if (!SignInButton.Text.Equals("登录中"))
                {
                    this.RunOnUiThread(() =>
                    {
                        SignInButton.Text = "登陆中";
                    });
                    Task startupWork = new Task(() => { Signin(UserNameText.Text,PassWordText.Text,SignInButton); });
                    startupWork.Start();
                }
                else
                {
                    Android.Widget.Toast.MakeText(this, "请输入用户名", Android.Widget.ToastLength.Short).Show();
                }
            };
            /*注册按钮按下执行的东西*/
            LogInButton.Click += (s, arg) =>
            {
                //JsonObject JSONresult;
                if (!LogInButton.Text.Equals("注册中"))
                {
                    this.RunOnUiThread(() =>
                    {
                        LogInButton.Text = "注册中";
                    });
                    Task startupWork = new Task(() => { Login(UserNameText.Text, PassWordText.Text, LogInButton); });
                    startupWork.Start();
                }
            };
        }
        private async void Login(string name, string password, Button LogInButton)
        {
            await Task.Delay(10);
            /*获取数据库文件路径*/
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            /*创建链接*/
            var db = new SQLiteConnection(dbPath);
            /*创建数据表*/
            db.CreateTable<UserTable>();
            if (NameCheck(name).Equals("通过"))
            {
                var postuser = new HttpPost();
                var result = postuser.PostUserByJson("http://www.nakago.cc/User/PostUser", name, MD5Encrypt(password));//这里是网站地址（字符串）,和传入的参数（字符串）顺便得到一个服务器的返回数据
                if (result.Equals("true"))
                {
                    this.RunOnUiThread(() =>
                    {
                        LogInButton.Text = "注册";
                        Task startupWork = new Task(() => { ToMain(name); });
                        Android.Widget.Toast.MakeText(this, "注册成功", Android.Widget.ToastLength.Short).Show();
                        var data = new UserTable();
                        data.UserName = name;
                        data.PassWord = MD5Encrypt(password);
                        db.Insert(data);
                        startupWork.Start();
                    });
                }
                else
                {
                    this.RunOnUiThread(() =>
                    {
                        LogInButton.Text = "注册";
                        Android.Widget.Toast.MakeText(this, "注册失败:" + result, Android.Widget.ToastLength.Short).Show();
                    });
                }
            }
            else
            {
                this.RunOnUiThread(() =>
                {
                    LogInButton.Text = "注册";
                    Android.Widget.Toast.MakeText(this, "注册失败:" + NameCheck(name), Android.Widget.ToastLength.Short).Show();
                });
            }
        }
        private async void Signin(string name,string password , Button SignInButton)
        {
            await Task.Delay(10);
            /*获取数据库文件路径*/
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormdemo.db3");
            /*创建链接*/
            var db = new SQLiteConnection(dbPath);
            /*创建数据表*/
            db.CreateTable<UserTable>();
            if (NameCheck(name).Equals("通过"))
            {
                var getuser = new HttpPost();
                var result = getuser.CheckUserByJson("http://www.nakago.cc/User/CheckUser", name, MD5Encrypt(password));//这里是网站地址（字符串）,和传入的参数（字符串）顺便得到一个服务器的返回数据
                if (result.Equals("true"))//判断字符串是否一样
                {
                    this.RunOnUiThread(() =>
                    {
                        SignInButton.Text = "登陆";
                        Task startupWork = new Task(() => { ToMain(name); });
                        Android.Widget.Toast.MakeText(this, "登录成功", Android.Widget.ToastLength.Short).Show();
                        var table = db.Table<UserTable>();//查找本地数据库是否有此用户
                        foreach (var user in table)
                        {
                            if (user.UserName.Equals(name) && user.PassWord.Equals(MD5Encrypt(password)))
                            {
                                startupWork.Start();
                                return;//查到存在则退出
                            }
                        }
                        var data = new UserTable();//没找到就会执行到这
                        data.UserName = name;
                        data.PassWord = MD5Encrypt(password);
                        db.Insert(data);//向本地数据库插入一个用户
                        startupWork.Start();
                    });
                }
                else
                {
                    this.RunOnUiThread(() =>
                    {
                        SignInButton.Text = "登陆";
                        Android.Widget.Toast.MakeText(this, "登录失败:" + result, Android.Widget.ToastLength.Short).Show();
                    });
                }
            }
            else
            {
                this.RunOnUiThread(() =>
                {
                    SignInButton.Text = "登陆";
                    Android.Widget.Toast.MakeText(this, "登录失败:"+ NameCheck(name), Android.Widget.ToastLength.Short).Show();
                });
            }
        }
        private string NameCheck(string name)
        {
            if (name.Length > 10) return "用户名超出长度";
            if (name.Length == 0) return "请输入用户名";
            if (name.Contains("#")) return "检测到非法字符";
            else return "通过";
        }
        private static string MD5Encrypt(string EncryptString)
        {
            MD5 m_ClassMD5 = new MD5CryptoServiceProvider();
            string m_strEncrypt = "";
            try
            {
                m_strEncrypt = BitConverter.ToString(m_ClassMD5.ComputeHash(Encoding.Default.GetBytes(EncryptString))).Replace("-", "");

            }
            catch (ArgumentException ex)
            {
                throw ex;
            }
            catch (CryptographicException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                m_ClassMD5.Clear();
            }
            return m_strEncrypt;
        }
        private async void ToMain(string name)
        {
            await Task.Delay(500);
            var MyActive = new Intent(this, typeof(MainActivity));
            MyActive.PutExtra("User", name);
            StartActivity(MyActive);
        }
    }
    /*数据库格式声明*/
    public class UserTable
    {
        public string UserName { get; set; }

        public string PassWord { get; set; }
    }
}