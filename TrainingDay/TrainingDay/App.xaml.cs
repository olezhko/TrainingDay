﻿using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.Views;
using TrainingDay.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Device = Xamarin.Forms.Device;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TrainingDay
{
    public partial class App : Application
    {
        public const string DATABASE_NAME = "exercise.db";
        public const string Version = "1.0.6.13";
        private static Repository database;

        private static object lockBase = new object();
        public static Repository Database
        {
            get
            {
                lock (lockBase)
                {
                    if (database == null)
                    {
                        database = new Repository(DATABASE_NAME);
                    }
                    return database;
                }
            }
        }

        private static ImageQueueCacheDownloader imageCache;
        public static ImageQueueCacheDownloader ImageDownloader
        {
            get
            {
                if (imageCache == null)
                {
                    imageCache = new ImageQueueCacheDownloader();
                }
                return imageCache;
            }
        }
        public static double ScreenWidth; // need for pinchtozoomcontainer
        public static double ScreenHeight;// need for pinchtozoomcontainer

        public App(bool isLight)
        {
            InitializeComponent();
            AppCenter.Start(Device.RuntimePlatform == Device.iOS
                    ? "59807e71-013b-4d42-9306-4a6044d9dc5f"
                    : "96acc322-4770-4aa3-876b-16ce5a802a38", typeof(Analytics), typeof(Crashes));

            Resource.Culture = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                System.Exception ex = (System.Exception) args.ExceptionObject;
                Crashes.TrackError(ex);
            };

            if (Device.RuntimePlatform == Device.iOS)
            {
                MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                var last = Database.GetUpdates();
                if (last == null || Version.CompareTo(last.Version) > 0)
                {
                    MainPage = new UpdatesPage();
                }
                else
                {
                    MainPage = new NavigationPage(new MainPage());
                }
            }
            

            IsLightTheme = isLight;
            Resources = !IsLightTheme ? Resources.MergedDictionaries.First() : Resources.MergedDictionaries.Last();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjA3MzYyQDMxMzcyZTM0MmUzMFJnOEZnSm9wNmwzdU1MSEpiMmtjR2w0THgvTkpmSFRvaktXaUc0aTM5VUU9");
        }
        
        protected override void OnStart()
        {
            SendRegistrationToServer(Settings.Token);
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
            set
            {
                DependencyService.Get<IDeviceConfig>().SetTheme(value);
                Settings.IsLightTheme = value;
            }
        }

        public void SetIncomingFile(IncomingFile file)
        {
            var vm = TrainingSerialize.LoadFromFile(file.Content);
            if (vm != null)
                IncomingTraining(vm);
        }

        private void IncomingTraining(TrainingSerialize vm)
        {
            var exercises = Database.GetExerciseItems().ToList();
            var superSets = new List<SuperSet>();

            var id = Database.SaveTrainingItem(new Training() { Title = vm.Title });
            int exerciseId;
            foreach (var item in vm.Items)
            {
                var exercise = exercises.FirstOrDefault(a => a.CodeNum == item.CodeNum);
                if (exercise != null)
                    exerciseId = exercise.Id;
                else
                {
                    var newItem = new Exercise()
                    {
                        Description = item.ShortDescription,
                        ExerciseImageUrl = item.ExerciseImageUrl,
                        ExerciseItemName = item.ExerciseItemName,
                        MusclesString = item.Muscles,
                        TagsValue = item.TagsValue,
                        CodeNum = -1
                    };
                    exerciseId = Database.SaveExerciseItem(newItem);

                    newItem.Id = exerciseId;
                    exercises.Add(newItem);
                }

                int superSetId;
                if (item.SuperSetId == 0)
                    superSetId = 0;
                else
                {
                    var superSet = superSets.FirstOrDefault(a => a.Count == item.SuperSetId);

                    if (superSet != null)
                        superSetId = superSet.Id;
                    else
                    {
                        var newItem = new SuperSet()
                        {
                            TrainingId = id,
                        };
                        superSetId = Database.SaveSuperSetItem(newItem);

                        newItem.Count = item.SuperSetId;
                        newItem.Id = superSetId;
                        superSets.Add(newItem);
                    }
                }

                Database.SaveTrainingExerciseItem(new TrainingExerciseComm()
                {
                    OrderNumber = item.OrderNumber,
                    TrainingId = id,
                    SuperSetId = superSetId,
                    ExerciseId = exerciseId,
                    WeightAndRepsString = item.WeightAndRepsString,
                });
            }
        }

        private IncomingFile _incomingFile;
        public IncomingFile IncomingFile
        {
            get { return _incomingFile; }
            set
            {
                _incomingFile = value;
                SetIncomingFile(_incomingFile);
            }
        }

        public static int TrainingNotificationId { get; set; } = 100;
        public static int WeightNotificationId { get; set; } = 101;
        public static int TrainingImplementTimeId { get; set; } = 102;
        public static int NewBlogId { get; set; } = 103;

        public static bool WeightNotificationState { get; set; } 
        public static bool TrainingNotificationState { get; set; }

        public string NewWorkoutMessageTitle { get; set; } = Resource.TrainingString;
        public string NewWorkoutMessage = Resource.ReturnToTrainingMessage;
        public bool IsShowNewWorkoutNotify()
        {
            var lastTrainings = Database.GetLastTrainingItems();
            if (lastTrainings.Any())
            {
                lastTrainings = lastTrainings.OrderBy(item => item.Time);
                if (DateTime.Now - lastTrainings.Last().Time > TimeSpan.FromDays(7))
                {
                    return true;
                }
            }

            return false;
        }

        public string WeightMessageTitle { get; set; } = Resource.WeightString;


        public static int SyncFinishedId { get; set; } = 103;


        public string WeightMessage = Resource.PleaseEnterYourNewWeight;
        public bool IsShowWeightNotify()
        {
            if (Settings.IsWeightNotify)
            {
                var weight = Database.GetWeightNotesItems();
                if (weight.Any())
                {
                    weight = weight.OrderBy(note => note.Date);
                    var last = weight.Last();
                    if (DateTime.Now - last.Date > TimeSpan.FromDays(Settings.WeightNotifyFreq))
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public static async void SendRegistrationToServer(string token)
        {
            try
            {
                // send token and language
                if (string.IsNullOrEmpty(token))
                {
                    return;
                }
                Settings.IsTokenSavedOnServer = false;

                var language = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                var zone = TimeZoneInfo.Local.BaseUtcOffset;
                var res = await SiteService.SendTokenToServer(token, language.Name, zone, Settings.WeightNotifyFreq);
                Settings.IsTokenSavedOnServer = res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }


    public class IncomingFile
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}
