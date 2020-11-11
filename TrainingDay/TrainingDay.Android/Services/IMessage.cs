using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.App;
using Android.Widget;
using TrainingDay.Droid.Services;
using TrainingDay.Services;
using Xamarin.Forms;
using AlertDialog = Android.App.AlertDialog;
using Application = Android.App.Application;

[assembly: Xamarin.Forms.Dependency(typeof(MessageAndroid))]
namespace TrainingDay.Droid.Services
{
    class MessageAndroid : IMessage
    {
        public void ShowMessage(string message, string title)
        {
            AlertDialog.Builder dialog = new AlertDialog.Builder(Forms.Context);
            dialog.SetTitle(title);
            dialog.SetMessage(message);
            dialog.SetPositiveButton("OK", (senderAlert, args) => {

            });
            var alertdialog = dialog.Create();
            alertdialog.Show();
        }

        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }

        public void ShowNotification(int id, string title, string message,bool isUpdateCurrent, bool isSilent)
        {
            Intent intent = new Intent(Application.Context, typeof(MainActivity));
            intent.PutExtra(title, title);
            intent.PutExtra(message, message);

            PendingIntent pendingIntent = PendingIntent.GetActivity(Application.Context, pendingIntentId, intent, 
                isUpdateCurrent? PendingIntentFlags.UpdateCurrent: PendingIntentFlags.OneShot);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(Application.Context, isSilent?MainActivity.Silent_CHANNEL_ID:MainActivity.CHANNEL_ID)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetLargeIcon(BitmapFactory.DecodeResource(Application.Context.Resources, Resource.Drawable.main))
                .SetSmallIcon(Resource.Drawable.main)
                .SetPriority((int)NotificationPriority.Low)
                .SetVisibility((int)NotificationVisibility.Public)
                .SetOngoing(true); // disable swipe

            Notification notification = builder.Build();
            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.Notify(id, notification);
        }

        public void CancelNotification(int id)
        {
            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.Cancel(id);
        }

        const int pendingIntentId = 0;


        //public string EnterAlert(string title, string oldValue)
        //{
        //    EditText et = new EditText(Forms.Context);
        //    et.Text = oldValue;
        //    AlertDialog.Builder alert = new AlertDialog.Builder(Forms.Context);
        //    alert.SetTitle(title);
        //    alert.SetView(et);

        //    alert.SetPositiveButton("OK", (senderAlert, args) => {

        //    });

        //    alert.SetNegativeButton("Cancel", (senderAlert, args) => {

        //    });

        //    var alertdialog = alert.Create();
        //    alertdialog.Show();
        //    return et.Text;
        //}
    }
}