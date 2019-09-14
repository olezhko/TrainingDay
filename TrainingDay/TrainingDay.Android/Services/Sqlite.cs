using System;
using System.IO;
using TrainingDay.Droid.Services;
using TrainingDay.Services;
using Xamarin.Forms;


[assembly: Dependency(typeof(SqLiteAndroid))]
namespace TrainingDay.Droid.Services
{
    public class SqLiteAndroid : ISQLite
    {
        public SqLiteAndroid()
        {
        }

        public string GetDatabasePath(string sqliteFilename)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var path = Path.Combine(documentsPath, sqliteFilename);
            return path;
        }
    }
}