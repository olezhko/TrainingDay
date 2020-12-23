using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firebase.CloudMessaging;
using UIKit;

namespace TrainingDay.iOS.Services
{
    class FirebaseService: MessagingDelegate
    {
        public override void DidReceiveRegistrationToken(Messaging messaging, string fcmToken)
        {
            Console.WriteLine($"Firebase registration token: {fcmToken}");
            base.DidReceiveRegistrationToken(messaging, fcmToken);
        }
    }
}