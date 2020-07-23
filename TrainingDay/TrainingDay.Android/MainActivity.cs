using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Android.Util;
using Newtonsoft.Json;
using TrainingDay.Services;

namespace TrainingDay.Droid
{
    [Activity(Label = "TrainingDay", Icon = "@drawable/icon", 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Android.Gms.Ads.MobileAds.Initialize(ApplicationContext, "ca-app-pub-8728883017081055~6404858576");
            var width = Resources.DisplayMetrics.WidthPixels;
            var height = Resources.DisplayMetrics.HeightPixels;
            var density = Resources.DisplayMetrics.Density;

            App.ScreenWidth = (width - 0.5f) / density;
            App.ScreenHeight = (height - 0.5f) / density;


            LoadSettings();

            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.SetFlags(new string[]
                {"CarouselView_Experimental", "IndicatorView_Experimental", "Expander_Experimental"});
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Android.Glide.Forms.Init(this, debug: true);
            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();
            CheckAppPermissions();

            LoadApplication(new App(local.IsLightTheme));


            IsPlayServicesAvailable();
            CreateNotificationChannel();
        }
        SettingsLocal local = new SettingsLocal();
        private void LoadSettings()
        {
            var data = FileLoader.ReadData().Result;
            if (data != null)
            {
                local = JsonConvert.DeserializeObject<SettingsLocal>(data);
            }
            else
            {
                local.IsLightTheme = false;
                FileLoader.SaveData(JsonConvert.SerializeObject(local));
            }
            

            int theme = 0;
            if (!local.IsLightTheme)
            {
                theme = Resource.Style.MainTheme;
            }
            else
            {
                theme = Resource.Style.MyCustomTheme;
            }

            SetTheme(theme);
        }


        private void CheckAppPermissions()
        {
            if ((int) Build.VERSION.SdkInt < 23)
            {
                return;
            }
            else
            {
                if (PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, PackageName) !=
                    Permission.Granted
                    && PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, PackageName) !=
                    Permission.Granted)
                {
                    var permissions = new string[]
                        {Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage};
                    RequestPermissions(permissions, 1);
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            var type = Intent.GetStringExtra("type");
            if (type == "weight")
            {

            }


            //if (Intent.Extras != null)
            //{
            //    foreach (var key in Intent.Extras.KeySet())
            //    {
            //        var value = Intent.Extras.GetString(key);
            //        Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
            //    }
            //}

            //if (Intent.Extras != null)
            //{
            //    var page = Intent.Extras.GetString("type");
            //    if (page == "weight")
            //    {

            //    }
            //    else
            //    {
            //        (App.Current as App).OpenPage(page);
            //    }
            //}
        }

        static readonly string TAG = "MainActivity";

        internal static readonly string CHANNEL_ID = "my_notification_channel";

        private bool IsPlayServicesAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            return resultCode == ConnectionResult.Success;
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification 
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID, "my_notification_channel", NotificationImportance.Max)
            {
                Description = "Description"
            };

            var notificationManager = (NotificationManager) GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }

    public class SettingsLocal
    {
        public bool IsLightTheme { get; set; }
    }
}