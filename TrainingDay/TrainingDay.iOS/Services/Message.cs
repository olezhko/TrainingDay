using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using TrainingDay.iOS.Services;
using TrainingDay.Services;
using UIKit;
using UserNotifications;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(MessageIOS))]
namespace TrainingDay.iOS.Services
{
    public class MessageIOS : IMessage
    {
        const double LONG_DELAY = 3.5;
        const double SHORT_DELAY = 1.0;
        NSTimer alertDelay;
        UIAlertController alert;
        public void ShowMessage(string message, string title)
        {
            UIAlertView alert = new UIAlertView()
            {
                Title = title,
                Message = message
            };
            alert.AddButton("OK");

            alert.Clicked += delegate (object a, UIButtonEventArgs b) {
                Console.WriteLine("Button " + b.ButtonIndex.ToString() + " clicked");
            };
            alert.Show();
        }

        public void LongAlert(string message)
        {
            ShowAlert(message, LONG_DELAY);
        }
        public void ShortAlert(string message)
        {
            ShowAlert(message, SHORT_DELAY);
        }
        
        void ShowAlert(string message, double seconds)
        {
            ShowMessage(message, "");

            //alertDelay = NSTimer.CreateScheduledTimer(seconds, (obj) =>
            //{
            //    DismissMessage();
            //});
            //alert = UIAlertController.Create(null, message, UIAlertControllerStyle.Alert);
            //UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
        }

        void DismissMessage()
        {
            if (alert != null)
            {
                alert.DismissViewController(true, null);
            }
            if (alertDelay != null)
            {
                alertDelay.Dispose();
            }
        }

        public void ShowNotification(int id, string title, string message, bool isUpdateCurrent, bool isSilent)
        {
            // EARLY OUT: app doesn't have permissions
            if (!hasNotificationsPermission)
            {
                Initialize();
                return;
            }

            var content = new UNMutableNotificationContent()
            {
                Title = title,
                Subtitle = "",
                Body = message,
                Badge = 1
            };

            // Local notifications can be time or location based
            // Create a time-based trigger, interval is in seconds and must be greater than 0
            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);

            var request = UNNotificationRequest.FromIdentifier(id.ToString(), content, trigger);
            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                {
                    throw new Exception($"Failed to schedule notification: {err}");
                }
            });
        }

        public void CancelNotification(int id)
        {
            UNUserNotificationCenter.Current.RemoveDeliveredNotifications(new []{id.ToString()});
        }

        bool hasNotificationsPermission;
        public void Initialize()
        {
            // request the permission to use local notifications
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) =>
            {
                hasNotificationsPermission = approved;
            });
        }
    }

    public class iOSNotificationReceiver : UNUserNotificationCenterDelegate
    {
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            DependencyService.Get<IMessage>().ShowNotification(notification.Handle.ToInt32(), notification.Request.Content.Title, notification.Request.Content.Body,false,false);

            // alerts are always shown for demonstration but this can be set to "None"
            // to avoid showing alerts if the app is in the foreground
            completionHandler(UNNotificationPresentationOptions.Alert);
        }
    }
}