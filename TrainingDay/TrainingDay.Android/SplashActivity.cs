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
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(typeof(MainActivity));
        }
    }


    public class FileLoader
    {
        public static async Task<string> ReadData()
        {
            try
            {

                var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SettingData.txt");

                if (backingFile == null || !File.Exists(backingFile))
                {
                    return null;
                }
                string FileData = string.Empty;
                using (var reader = new StreamReader(backingFile, true))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        FileData = line;
                    }
                }

                return FileData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task SaveData(string Data)
        {
            try
            {
                var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SettingData.txt");
                using (var writer = File.CreateText(backingFile))
                {
                    await writer.WriteLineAsync(Data);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}