using System;
using Foundation;
using Google.MobileAds;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.XForms.iOS.EffectsView;
using TrainingDay.iOS.Services;
using UIKit;
using Firebase.Core;
using UserNotifications;
using Firebase.CloudMessaging;

namespace TrainingDay.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            MobileAds.SharedInstance.Start(completionHandler:null);
            global::Xamarin.Forms.Forms.Init();
            SfListViewRenderer.Init();
            SfEffectsViewRenderer.Init();  //Initialize only when effects view is added to Listview.
            LoadApplication(new App(false));

            Firebase.Core.App.Configure();
            Messaging.SharedInstance.Delegate = this;
            RegisterFirebase();
            Firebase.InstanceID.InstanceId.Notifications.ObserveTokenRefresh((sender, e) =>
            {
                var token = Messaging.SharedInstance.FcmToken;
                App.SendRegistrationToServer(token);
                //var newToken = Firebase.InstanceID.InstanceId.SharedInstance.GetTokenAsync();
                // if you want to send notification per user, use this token
                //System.Diagnostics.Debug.WriteLine(newToken);

                //connectFCM();
            });

            App.ScreenWidth = UIScreen.MainScreen.Bounds.Width;
            App.ScreenHeight = UIScreen.MainScreen.Bounds.Height;
            return base.FinishedLaunching(app, options);
        }

        #region Firebase
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            // Get current device token
            var DeviceToken = deviceToken.Description;
            if (!string.IsNullOrWhiteSpace(DeviceToken))
            {
                DeviceToken = DeviceToken.Trim('<').Trim('>');
            }

            // Get previous device token
            var oldDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");

            // Has the token changed?
            if (string.IsNullOrEmpty(oldDeviceToken) || !oldDeviceToken.Equals(DeviceToken))
            {
                App.SendRegistrationToServer(DeviceToken);
            }

            // Save new device token
            NSUserDefaults.StandardUserDefaults.SetString(DeviceToken, "PushDeviceToken");
        }

        [Export("messaging:didReceiveRegistrationToken:")]
        public void DidReceiveRegistrationToken(Messaging messaging, string fcmToken)
        {
            Firebase.Core.App.Configure();
            // Monitor token generation: To be notified whenever the token is updated.
            //Console.WriteLine("Received token: " + fcmToken);
            // Handle here how your app is storing token locally or send it to server
            // Note: This callback is fired at each app startup and whenever a new token is generated.
        }

        private void RegisterFirebase()
        {
            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {

                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this;

                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) => {

                });
            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            UIApplication.SharedApplication.RegisterForRemoteNotifications();
        }

        //public override void DidEnterBackground(UIApplication uiApplication)
        //{
        //    Messaging.SharedInstance.Disconnect();
        //}

        //public override void OnActivated(UIApplication uiApplication)
        //{
        //    connectFCM();
        //    base.OnActivated(uiApplication);
        //}

        //        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        //        {
        //#if DEBUG
        //            Firebase.InstanceID.InstanceId.SharedInstance.SetApnsToken(deviceToken, Firebase.InstanceID.ApnsTokenType.Sandbox);
        //#endif
        //#if RELEASE
        //			Firebase.InstanceID.InstanceId.SharedInstance.SetApnsToken(deviceToken, Firebase.InstanceID.ApnsTokenType.Prod);
        //#endif
        //        }

        // iOS 9 <=, fire when recieve notification foreground
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            Messaging.SharedInstance.AppDidReceiveMessage(userInfo);

            // Generate custom event
            NSString[] keys = { new NSString("Event_type") };
            NSObject[] values = { new NSString("Recieve_Notification") };
            var parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(keys, values, keys.Length);

            // Send custom event
            Firebase.Analytics.Analytics.LogEvent("CustomEvent", parameters);

            if (application.ApplicationState == UIApplicationState.Active)
            {
                System.Diagnostics.Debug.WriteLine(userInfo);
                var aps_d = userInfo["aps"] as NSDictionary;
                var alert_d = aps_d["alert"] as NSDictionary;
                var body = alert_d["body"] as NSString;
                var title = alert_d["title"] as NSString;
                debugAlert(title, body);
            }
        }

        // iOS 10, fire when recieve notification foreground
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            var title = notification.Request.Content.Title;
            var body = notification.Request.Content.Body;
            debugAlert(title, body);
        }

        private void debugAlert(string title, string message)
        {
            var alert = new UIAlertView(title ?? "Title", message ?? "Message", null, "Cancel", "OK");
            alert.Show();
        }
        #endregion
    }
}