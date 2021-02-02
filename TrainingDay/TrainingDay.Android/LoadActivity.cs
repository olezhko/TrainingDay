using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using System.IO;

namespace TrainingDay.Droid
{
    [Activity(Name = "TrainingDay.Android.LoadActivity", LaunchMode = LaunchMode.SingleTask, Label = "TrainingDay")]
    [IntentFilter(new string[] { Intent.ActionView, Intent.ActionEdit },
        Categories = new string[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "content",
        DataHost = "*",
        DataMimeType = "application/trday"
    )]
    public class LoadActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CheckAppPermissions();
            if (Intent.Action.Equals(Intent.ActionView))
            {
                if (Intent.Scheme.Equals(ContentResolver.SchemeContent))
                {
                    Android.Net.Uri fileUri = Intent.Data;
                    var fileContent = LoadBytesFromIntent(fileUri);
                    
                    var incomingFile = new IncomingFile { Name = "loaded", Content = fileContent };
                    (App.Current as App).IncomingFile = incomingFile;
                    Finish();
                }
            }
        }

        private byte[] LoadBytesFromIntent(Android.Net.Uri uri)
        {
            Stream stream = ContentResolver.OpenInputStream(uri);
            byte[] byteArray;

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                byteArray = memoryStream.ToArray();
            }
            return byteArray;
        }

        private void CheckAppPermissions()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                return;
            }
            else
            {
                if (PackageManager.CheckPermission(Manifest.Permission.ReadExternalStorage, PackageName) != Permission.Granted
                    && PackageManager.CheckPermission(Manifest.Permission.WriteExternalStorage, PackageName) != Permission.Granted)
                {
                    var permissions = new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };
                    RequestPermissions(permissions, 1);
                }
            }
        }
    }
}