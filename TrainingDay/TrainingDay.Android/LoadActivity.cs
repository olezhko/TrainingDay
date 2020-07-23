using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;

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
                    var type = Intent.Type;
                    Android.Net.Uri fileUri = Intent.Data;

                    var filepath = GetActualPathFromFile(fileUri);
                    var fileContent = File.ReadAllBytes(filepath);
                    var incomingFile = new IncomingFile { Name = filepath, Content = fileContent };
                    (App.Current as App).IncomingFile = incomingFile;
                    Finish();
                }
            }
            // Create your application here
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

        private string GetActualPathFromFile(Android.Net.Uri uri)
        {
            bool isKitKat = Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat;

            if (isKitKat && DocumentsContract.IsDocumentUri(this, uri))
            {
                // ExternalStorageProvider
                if (isExternalStorageDocument(uri))
                {
                    string docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    string[] split = docId.Split(chars);
                    string type = split[0];

                    if ("primary".Equals(type, StringComparison.OrdinalIgnoreCase))
                    {
                        return Android.OS.Environment.ExternalStorageDirectory + "/" + split[1];
                    }
                }
                // DownloadsProvider
                else if (isDownloadsDocument(uri))
                {
                    string id = DocumentsContract.GetDocumentId(uri);

                    Android.Net.Uri contentUri = ContentUris.WithAppendedId(
                                    Android.Net.Uri.Parse("content://downloads/public_downloads"), long.Parse(id));

                    //System.Diagnostics.Debug.WriteLine(contentUri.ToString());

                    return getDataColumn(this, contentUri, null, null);
                }
                // MediaProvider
                else if (isMediaDocument(uri))
                {
                    String docId = DocumentsContract.GetDocumentId(uri);

                    char[] chars = { ':' };
                    String[] split = docId.Split(chars);

                    String type = split[0];

                    Android.Net.Uri contentUri = null;
                    if ("image".Equals(type))
                    {
                        contentUri = MediaStore.Images.Media.ExternalContentUri;
                    }
                    else if ("video".Equals(type))
                    {
                        contentUri = MediaStore.Video.Media.ExternalContentUri;
                    }
                    else if ("audio".Equals(type))
                    {
                        contentUri = MediaStore.Audio.Media.ExternalContentUri;
                    }

                    String selection = "_id=?";
                    String[] selectionArgs = new String[]
                    {
                split[1]
                    };

                    return getDataColumn(this, contentUri, selection, selectionArgs);
                }
            }
            // MediaStore (and general)
            else if ("content".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {

                // Return the remote address
                if (isGooglePhotosUri(uri))
                    return uri.LastPathSegment;

                return getDataColumn(this, uri, null, null);
            }
            // File
            else if ("file".Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return uri.Path;
            }

            return null;
        }

        public static String getDataColumn(Context context, Android.Net.Uri uri, String selection, String[] selectionArgs)
        {
            ICursor cursor = null;
            String column = "_data";
            String[] projection =
            {
        column
    };

            try
            {
                cursor = context.ContentResolver.Query(uri, projection, selection, selectionArgs, null);
                if (cursor != null && cursor.MoveToFirst())
                {
                    int index = cursor.GetColumnIndexOrThrow(column);
                    return cursor.GetString(index);
                }
            }
            finally
            {
                if (cursor != null)
                    cursor.Close();
            }
            return null;
        }

        //Whether the Uri authority is ExternalStorageProvider.
        public static bool isExternalStorageDocument(Android.Net.Uri uri)
        {
            return "com.android.externalstorage.documents".Equals(uri.Authority);
        }

        //Whether the Uri authority is DownloadsProvider.
        public static bool isDownloadsDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.downloads.documents".Equals(uri.Authority);
        }

        //Whether the Uri authority is MediaProvider.
        public static bool isMediaDocument(Android.Net.Uri uri)
        {
            return "com.android.providers.media.documents".Equals(uri.Authority);
        }

        //Whether the Uri authority is Google Photos.
        public static bool isGooglePhotosUri(Android.Net.Uri uri)
        {
            return "com.google.android.apps.photos.content".Equals(uri.Authority);
        }

    }
}