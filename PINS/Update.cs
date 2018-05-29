
using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android;

using Android.App;
using Android.OS;

namespace PINS
{

    public interface IMainInterface
    {
        void installprepar();
    }

    [Activity(Label = "PINS")]
    public class UpdateActivity : Activity, IMainInterface
    {
        TextView download;
        DownloadManager downloadManager;
        Android.Database.ICursor cursor;
        public static DownLoadReceiver receiver;
        public static long downloadId;
        string FileName = "ProjectD.ProjectD.apk";
        string FilePatch = "ProjectDupdate";
        Java.IO.File file;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.UpDate);
            download = FindViewById<TextView>(Resource.Id.downloadtext);
            // Create your application here
            downloadManager = (DownloadManager)GetSystemService(DownloadService);

            run("http://www.nakago.cc/APPrelease/PINS.apk");
        }

        public async void run(string uri)
        {
            await Task.Delay(10);
            try
            {
                file = Android.OS.Environment.GetExternalStoragePublicDirectory(FilePatch);
                file = new Java.IO.File(file, FileName);
                file.Delete();//如果文件存在就删除
                if (DownLoad(uri) == false)//开启下载
                {
                    this.RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "下载出错请重试", ToastLength.Short).Show();
                    });
                    return;
                }
                /*创建监视*/
                /*广播监视*/
                Listener();
                /*
                DownloadManager.Query query = new DownloadManager.Query();
                while (true)
                {
                    await Task.Delay(1000);
                    cursor = downloadManager.InvokeQuery(query.SetFilterById(downloadId));
                    //Android.Database.ICursor cursor = downloadManager.InvokeQuery(query);

                    if (!cursor.MoveToFirst())
                    {
                        cursor.Close();
                        break;
                    }
                    DownloadStatus status = (DownloadStatus)cursor.GetInt(cursor.GetColumnIndex(DownloadManager.ColumnStatus));
                    long downloadedSoFar = cursor.GetLong(cursor.GetColumnIndex(DownloadManager.ColumnBytesDownloadedSoFar));// 下载文件的总字节大小
                    long totalSize = cursor.GetLong(cursor.GetColumnIndex(DownloadManager.ColumnTotalSizeBytes));
                    download.Text = downloadedSoFar.ToString() + "/" + totalSize.ToString() + "\n" + status.ToString();
                    if (status == DownloadStatus.Successful)
                    {
                        cursor.Close();
                        break;
                    }
                }
                */
            }
            catch (Exception ex)
            {
                this.RunOnUiThread(() =>
                {
                    Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
                });
            }
        }

        private bool DownLoad(string Url)
        {
            try
            {
                /*检查是否有访问存储权限*/
                if (requestAllPower() == false)
                {
                    return false;
                }

                DownloadManager.Request request = new DownloadManager.Request(Android.Net.Uri.Parse(Url));
                request.SetAllowedNetworkTypes(Android.App.DownloadNetwork.Wifi);//设定只有wifi环境下载
                request.SetTitle("我的下载");
                request.SetDescription("下载一个大文件");
                request.SetMimeType("application/vnd.android.package-archive");
                request.SetNotificationVisibility(Android.App.DownloadVisibility.Visible | Android.App.DownloadVisibility.VisibleNotifyCompleted);
                request.SetDestinationUri(Android.Net.Uri.FromFile(file));
                this.RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "正在下载", ToastLength.Short).Show();
                });
                downloadId = downloadManager.Enqueue(request);
                return true;
            }
            catch (Exception ex)
            {
                this.RunOnUiThread(() =>
                {
                    Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
                });
                return false;
            }
        }
        public void installprepar()
        {
            if (!PackageManager.CanRequestPackageInstalls())
            {
                var packageURI = Android.Net.Uri.Parse("package:" + ApplicationContext.PackageName);
                //注意这个是8.0新API
                Intent intent_1 = new Intent(Android.Provider.Settings.ActionManageUnknownAppSources, packageURI);
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "开启权限", ToastLength.Short).Show();
                });
                StartActivityForResult(intent_1, 10086);
                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "安装出错请重试", ToastLength.Short).Show();
                });
            }
            if (file.Exists())
            {
                installApk();
            }
            else
            {
                this.RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "安装出错请重试", ToastLength.Short).Show();
                });
            }
        }
        private void installApk()
        {
            if (this == null)
                return;
            try
            {
                var Uri = FileProvider.GetUriForFile(this, this.ApplicationContext.PackageName + ".com.package.name.provider", file);
                Intent intent = new Intent(Intent.ActionView);
                intent.SetFlags(ActivityFlags.NewTask);
                intent.PutExtra(Intent.ExtraNotUnknownSource, true);
                intent.SetData(Uri);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                //intent.SetType("application/vnd.android.package-archive");
                if (intent.ResolveActivity(this.ApplicationContext.PackageManager) != null)
                {
                    this.RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "启动安装", ToastLength.Short).Show();
                        this.StartActivity(intent);
                        UnregisterReceiver(receiver);
                    });
                }
            }
            catch (Exception ex)
            {
                this.RunOnUiThread(() =>
                {
                    Toast.MakeText(this, ex.ToString(), ToastLength.Short).Show();
                });
            }
        }
        public bool requestAllPower()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == Android.Content.PM.Permission.Denied)
            {
                if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.WriteExternalStorage))
                {
                    return false;
                }
                else
                {
                    ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, 1);
                    return false;
                }
            }
            return true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            if (requestCode == 1)
            {

                // Check if the only required permission has been granted
                if ((grantResults.Length == 1) && (grantResults[0] == Android.Content.PM.Permission.Granted))
                {
                    // Location permission has been granted, okay to retrieve the location of the device.
                }
                else
                {

                }
            }
            else
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }
        protected override void OnActivityResult(Int32 requestCode, [Android.Runtime.GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok && requestCode == 10086)
            {
                installprepar();
            }

        }

        private void Listener()
        {
            // 注册广播监听系统的下载完成事件。  
            IntentFilter intentFilter = new IntentFilter(DownloadManager.ActionDownloadComplete);
            receiver = new DownLoadReceiver();
            receiver.mainInterface = this;
            RegisterReceiver(receiver, intentFilter);
            //UnregisterReceiver(receiver);
        }
    }

    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { DownloadManager.ActionDownloadComplete })]
    public class DownLoadReceiver : BroadcastReceiver
    {
        public IMainInterface mainInterface;
        public override void OnReceive(Context context, Intent intent)
        {
            if (mainInterface != null)
            {
                long ID = intent.GetLongExtra(DownloadManager.ExtraDownloadId, -1);
                if (ID == UpdateActivity.downloadId)
                {
                    mainInterface.installprepar();
                }
            }
        }

    }

}
