using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace TrainingDay.Droid
{
    [Activity(Label = "TrainingDay", Icon = "@drawable/icon", Theme = "@style/splashscreen", MainLauncher = true, NoHistory = true)]
    //[IntentFilter(new string[] { Intent.ActionView, Intent.ActionEdit },
    //    Categories = new string[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    //    DataScheme = "content",
    //    DataHost = "*",
    //    DataMimeType = "application/trday"
    //)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(typeof(MainActivity));
        }
    }
}