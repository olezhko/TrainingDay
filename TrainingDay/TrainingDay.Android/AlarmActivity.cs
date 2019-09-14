using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Media;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using TrainingDay.Droid.Render;
using TrainingDay.Services;

namespace TrainingDay.Droid
{
    [Activity(Label = "AlarmActivity", NoHistory = true, Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AlarmActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        Alarm _alarm;
        //AlarmApp.Models.Settings _settings;

        MediaPlayer _mediaPlayer = new MediaPlayer();

        public AlarmActivity()
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "Constructor");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "OnCreate");
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AlarmActivity);
            var closeButton = FindViewById<Button>(Resource.Id.closeButton);
            closeButton.Click += CloseButton_Click;

            // add flags to turn screen on and appear over lock screen
            Window.AddFlags(WindowManagerFlags.ShowWhenLocked);
            Window.AddFlags(WindowManagerFlags.DismissKeyguard);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            Window.AddFlags(WindowManagerFlags.TurnScreenOn);

            Bundle bundle = Intent.Extras;

            if (bundle == null) return;

            var id = (int)bundle.Get("id");
            var timeTextView = FindViewById<TextView>(Resource.Id.timeTextView);
            var nameTextView = FindViewById<TextView>(Resource.Id.nameTextView);
            _alarm = App.Database.GetAlarmItem(id);

            timeTextView.Text = _alarm.TimeOffset.ToLocalTime().ToString(@"hh\:mm");
            nameTextView.Text = _alarm.Name;

            _mediaPlayer = MediaPlayer.Create(this, Resource.Raw.alarmsound);

            _mediaPlayer.Looping = true;
            _mediaPlayer.Start();
        }

        void CloseButton_Click(object sender, EventArgs e)
        {
            _mediaPlayer?.Stop();
            FinishAndRemoveTask();
            AlarmSetterAndroid.StartAlarm(_alarm);
        }
    }
}