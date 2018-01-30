using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainingDay.Helpers;
using TrainingDay.Model;
using TrainingDay.ViewModel;
using Xamarin.Forms;

namespace TrainingDay
{
    public partial class App : Application
    {
        public const string DATABASE_NAME = "exercise.db";
        public static Repository database;
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


        public App()
        {
            InitializeComponent();
            if (Device.OS != TargetPlatform.WinPhone)
            {
                Resource.Culture = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            }
            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
