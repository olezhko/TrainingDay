using Android.App;
using Android.Content;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TrainingDay.Controls;
using TrainingDay.Droid.Render;
using TrainingDay.Services;
using TrainingDay.ViewModels;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Xamarin.Forms.Dependency(typeof(AlarmSetterAndroid))]
namespace TrainingDay.Droid.Render
{
    public class AlarmSetterAndroid : IAlarmSetter
    {
        public static string AlarmTag = "Al4rm";

        public void SetAlarm(Alarm alarm)
        {
            StartAlarm(alarm);
        }

        public void SetRepeatingAlarm(Alarm alarm)
        {

        }

        public static void StartAlarm(Alarm alarm)
        {
            if (!alarm.IsActive)
            {
                return;
            }
            var alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            alarmIntent.PutExtra("id", alarm.Id);
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, GetAlarmId(alarm), alarmIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(pendingIntent); // possible here deletealarm

            //var difference = alarm.TimeOffset.LocalDateTime.TimeOfDay.Subtract(DateTime.Now.ToLocalTime().TimeOfDay);
            var difference = GetNextTime(alarm);
            var differenceAsMillis = difference.TotalMilliseconds;

            Debug.WriteLine($"Alarm activated: {difference} {alarm.Name}");
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat)
            {
                // only for kitkat and newer versions
                alarmManager.SetExact(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + (long)differenceAsMillis, pendingIntent);
            }
            else
            {
                alarmManager.Set(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + (long)differenceAsMillis, pendingIntent);
            }
        }

        private static TimeSpan GetNextTime(Alarm alarm)
        {
            var days = DaysOfWeek.Parse(alarm.Days);

            int i = DateTime.Now.DayOfWeek.ConvertToSimple();
            while (true)
            {
                if (days.Contains(WrapDay(i)))
                {
                    var timeSub = alarm.TimeOffset.LocalDateTime + TimeSpan.FromHours((i - DateTime.Now.DayOfWeek.ConvertToSimple()) * 24);
                    var diffTimeSpan = timeSub - DateTime.Now;
                    if (diffTimeSpan > TimeSpan.Zero)
                    {
                        return diffTimeSpan;
                    }
                    else
                    {
                        i++;
                        continue;
                    }
                }
                else
                {
                    i++;
                    continue;
                }
            }
        }

        static int WrapDay(int i)
        {
            for (; i >=7; i-=7)
            {
                
            }

            return i;
        }

        public void DeleteAlarm(Alarm alarm)
        {
            var alarmIntent = new Intent(Application.Context, typeof(AlarmReceiver));
            alarmIntent.SetFlags(ActivityFlags.IncludeStoppedPackages);
            alarmIntent.PutExtra("id", alarm.Id);

            var alarmToDeleteId = GetAlarmId(alarm);
            var alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
            var toDeletePendingIntent = PendingIntent.GetBroadcast(Application.Context, alarmToDeleteId, alarmIntent, PendingIntentFlags.CancelCurrent);
            alarmManager.Cancel(toDeletePendingIntent);
            Debug.WriteLine($"Alarm deactivated: {alarm.Name}");
        }

        static int GetAlarmId(Alarm alarm)
        {
            return alarm.Id;
        }

        public void DeleteAllAlarms(List<Alarm> alarms)
        {
            foreach (var alarm in alarms)
            {
                DeleteAlarm(alarm);
            }
        }
    }

    [BroadcastReceiver]
    public class AlarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Log.Debug(AlarmSetterAndroid.AlarmTag, "OPEN THE THING");
            var id = intent.GetIntExtra("id",0);

            var disIntent = new Intent(context, typeof(AlarmActivity));
            disIntent.PutExtra("id", id);
            disIntent.SetFlags(ActivityFlags.NewTask);
            context.StartActivity(disIntent);
            Log.Debug(AlarmSetterAndroid.AlarmTag, "START ACTIVITY");
        }
    }
}