using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using TrainingDay.iOS.Services;
using UIKit;
using UserNotifications;

namespace TrainingDay.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
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
            Xamarin.Forms.Forms.SetFlags(new string[]
                {"CarouselView_Experimental", "IndicatorView_Experimental", "Expander_Experimental","Shapes_Experimental"});
            global::Xamarin.Forms.Forms.Init();

            UNUserNotificationCenter.Current.Delegate = new iOSNotificationReceiver();
            LoadApplication(new App(false));
            App.ScreenWidth = UIScreen.MainScreen.Bounds.Width;
            App.ScreenHeight = UIScreen.MainScreen.Bounds.Height;
            OxyPlot.Xamarin.Forms.Platform.iOS.PlotViewRenderer.Init();

            return base.FinishedLaunching(app, options);
        }
    }
}
