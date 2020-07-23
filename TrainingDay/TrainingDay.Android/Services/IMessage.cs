using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Widget;
using Java.Lang;
using TrainingDay.Droid.Services;
using TrainingDay.Services;
using Xamarin.Forms;
using AlertDialog = Android.App.AlertDialog;
using Application = Android.App.Application;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;

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

        //public void ShowNotification(int id, string title, string message, string openedPage, PageEnum key)
        //{
        //    // Pass the current button press count value to the next activity:
        //    var valuesForActivity = new Bundle();
        //    valuesForActivity.PutString(key.ToString(), openedPage);

        //    // When the user clicks the notification, SecondActivity will start up.
        //    var resultIntent = new Android.Content.Intent(Application.Context, typeof(MainActivity));
        //    // Pass some values to SecondActivity:
        //    resultIntent.PutExtras(valuesForActivity);

        //    // Construct a back stack for cross-task navigation:
        //    var stackBuilder = TaskStackBuilder.Create(Application.Context);
        //    stackBuilder.AddParentStack(Class.FromType(typeof(MainActivity)));
        //    stackBuilder.AddNextIntent(resultIntent);

        //    // Create the PendingIntent with the back stack:            
        //    var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

        //    // Build the notification:
        //    var builder = new NotificationCompat.Builder(Application.Context, )
        //        .SetAutoCancel(true) // Dismiss the notification from the notification area when the user clicks on it
        //        .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
        //        .SetContentTitle(title) // Set the title
        //        .SetSmallIcon(Resource.Drawable.main) // This is the icon to display
        //        .SetContentText(message); // the message to display.

        //    // Finally, publish the notification:
        //    var notificationManager = NotificationManagerCompat.From(Application.Context);
        //    notificationManager.Notify(id, builder.Build());
        //}

        public void CancelNotification(int id)
        {
            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.Cancel(id);
        }


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