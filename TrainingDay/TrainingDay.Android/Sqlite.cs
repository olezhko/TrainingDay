using System;
using System.IO;
using TrainingDay.Droid;
using TrainingDay.Model;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_Android))]
namespace TrainingDay.Droid
{
    public class SQLite_Android : ISQLite
    {
        public SQLite_Android()
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