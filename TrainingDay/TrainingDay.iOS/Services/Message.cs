using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using TrainingDay.iOS.Services;
using TrainingDay.Services;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(MessageIOS))]
namespace TrainingDay.iOS.Services
{
    public class MessageIOS : IMessage
    {
        const double LONG_DELAY = 3.5;
        const double SHORT_DELAY = 2.0;

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

        public void ShowNotification(string title, string message, int openedPage)
        {
        }


        void ShowAlert(string message, double seconds)
        {
            alertDelay = NSTimer.CreateScheduledTimer(seconds, (obj) =>
            {
                dismissMessage();
            });
            alert = UIAlertController.Create(null, message, UIAlertControllerStyle.Alert);
            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
        }

        void dismissMessage()
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
    }
}