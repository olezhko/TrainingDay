using System;
using System.Linq;
using Plugin.LocalNotifications;
using TrainingDay.Resources;
using TrainingDay.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TrainingDay.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TrainingDay
{
    public partial class App : Application
    {
        public const string DATABASE_NAME = "exercise.db";
        private static Repository database;
        public static Repository Database
        {
            get
            {
                if (database == null)
                {
                    database = new Repository(DATABASE_NAME);
                }
                return database;
            }
        }
        public static double ScreenWidth; // need for pinchtozoomcontainer
        public static double ScreenHeight;// need for pinchtozoomcontainer
        public App()
        {
            InitializeComponent();

            Resource.Culture = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();

            MainPage = new MainPage();
            Resources = !IsLightTheme ? Resources.MergedDictionaries.First() : Resources.MergedDictionaries.Last();
        }

        public static int WeightNotificationId = 1;
        protected override void OnStart()
        {
            // Handle when your app starts
            if (Settings.IsWeightNotify)
            {
                var weight = Database.GetWeightNotesItems();
                if (weight.Any())
                {
                    weight = weight.OrderBy(note => note.Date);
                    var last = weight.Last();
                    if (DateTime.Now - last.Date > TimeSpan.FromDays(Settings.WeightNotifyFreq))
                    {
                        CrossLocalNotifications.Current.Show(Resource.WeightString, Resource.PleaseEnterYourNewWeight, WeightNotificationId);
                    }
                }
                else
                {
                        CrossLocalNotifications.Current.Show(Resource.WeightString, Resource.PleaseEnterYourNewWeight, WeightNotificationId);
                }
                //if (weight.All(note => note.Date.Date != DateTime.Now.Date))
                //{
                //}
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public bool IsLightTheme
        {
            get => Settings.IsLightTheme;
            set => Settings.IsLightTheme = value;
        }
    }
}
