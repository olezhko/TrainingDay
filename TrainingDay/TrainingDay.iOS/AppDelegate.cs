using Foundation;
using Google.MobileAds;
using Syncfusion.ListView.XForms.iOS;
using Syncfusion.XForms.iOS.EffectsView;
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
            MobileAds.SharedInstance.Start(completionHandler:null);
            Xamarin.Forms.Forms.SetFlags(new string[]
                {"CarouselView_Experimental", "IndicatorView_Experimental", "Expander_Experimental","Shapes_Experimental"});
            global::Xamarin.Forms.Forms.Init();
            OxyPlot.Xamarin.Forms.Platform.iOS.PlotViewRenderer.Init();
            SfListViewRenderer.Init();
            SfEffectsViewRenderer.Init();  //Initialize only when effects view is added to Listview.
            UNUserNotificationCenter.Current.Delegate = new iOSNotificationReceiver();
            LoadApplication(new App(false));
            App.ScreenWidth = UIScreen.MainScreen.Bounds.Width;
            App.ScreenHeight = UIScreen.MainScreen.Bounds.Height;
            return base.FinishedLaunching(app, options);
        }
    }
}