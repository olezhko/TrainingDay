using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using Google.MobileAds;
using TrainingDay.iOS.Services;
using TrainingDay.Views.Controls;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AdInterstitial_IOs))]
namespace TrainingDay.iOS.Services
{
    public class AdInterstitial_IOs : IAdInterstitial
    {
        Interstitial interstitial;

        //public AdInterstitial_iOS()
        //{
        //    interstitial.ScreenDismissed += (s, e) => LoadAd("");
        //}

        void LoadAd(string url)
        {
            interstitial = new Interstitial(url);
            var request = Request.GetDefaultRequest();
            //request.TestDevices = new string[] { "Your Test Device ID", "GADSimulator" };
            interstitial.LoadRequest(request);
        }

        UIViewController GetVisibleViewController()
        {
            var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            if (rootController.PresentedViewController == null)
                return rootController;

            if (rootController.PresentedViewController is UINavigationController)
            {
                return ((UINavigationController)rootController.PresentedViewController).VisibleViewController;
            }

            if (rootController.PresentedViewController is UITabBarController)
            {
                return ((UITabBarController)rootController.PresentedViewController).SelectedViewController;
            }

            return rootController.PresentedViewController;
        }

        public void ShowAd(string url)
        {
            LoadAd(url);
            if (interstitial.IsReady)
            {
                var viewController = GetVisibleViewController();
                interstitial.PresentFromRootViewController(viewController);
            }
        }
    }
}