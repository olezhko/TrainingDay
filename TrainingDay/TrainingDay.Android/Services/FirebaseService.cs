using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Firebase.Messaging;
using System;
using System.Collections.Generic;
using TrainingDay.Services;
using Application = Android.App.Application;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;

namespace TrainingDay.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
        public override void OnNewToken(string p0)
        {
            base.OnNewToken(p0);
            Log.Debug(TAG, "Refreshed token: " + p0);
            App.SendRegistrationToServer(p0);
            Settings.Token = p0;
        }


        public override void OnMessageReceived(RemoteMessage message)
        {
            var app = (App.Current as App);
            var notify = message.GetNotification();
            string body = "";
            try
            {
                if (notify != null)
                {
                    body = notify.Body;
                }
                else
                {
                    message.Data.TryGetValue("type", out string backBody);
                    if (backBody != null)
                    {
                        body = backBody;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (string.IsNullOrEmpty(body))
            {
                return;   
            }
            switch (body)
            {
                case "Weight":
                case "weight":
                    if (app.IsShowWeightNotify())
                    {
                        SendNotification(app.WeightMessageTitle, app.WeightMessage, message.Data, App.WeightNotificationId);
                        App.WeightNotificationState = true;
                    }
                    break;
                case "NewWorkout":
                    if (app.IsShowNewWorkoutNotify())
                    {
                        SendNotification(app.NewWorkoutMessageTitle, app.NewWorkoutMessage, message.Data, App.TrainingNotificationId);
                        App.TrainingNotificationState = true;
                    }
                    break;
            }
        }

        void SendNotification(string title, string messageBody, IDictionary<string, string> data, int id)
        {
            //var intent = new Intent(Application.Context, typeof(MainActivity));
            //intent.AddFlags(ActivityFlags.ClearTop);
            //foreach (var key in data.Keys)
            //{
            //    intent.PutExtra(key, data[key]);
            //}

            //var pendingIntent = PendingIntent.GetActivity(this, id, intent, PendingIntentFlags.UpdateCurrent);



            var valuesForActivity = new Bundle();
            foreach (var key in data.Keys)
            {
                valuesForActivity.PutString(key, data[key]);
            }
            var resultIntent = new Android.Content.Intent(Application.Context, typeof(MainActivity));
            resultIntent.PutExtras(valuesForActivity);

            var stackBuilder = TaskStackBuilder.Create(Application.Context);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
            stackBuilder.AddNextIntent(resultIntent);

            var pendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);


            var notificationBuilder = new NotificationCompat.Builder(Application.Context, MainActivity.CHANNEL_ID)
                .SetSmallIcon(Resource.Drawable.main)
                .SetContentTitle(title)
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetPriority((int)NotificationPriority.Max)
                .SetCategory(Notification.CategoryMessage)
                .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.Notify(id, notificationBuilder.Build());
        }
    }
}