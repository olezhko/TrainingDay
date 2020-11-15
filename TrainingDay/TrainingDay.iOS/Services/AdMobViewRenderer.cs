using System.ComponentModel;
using System.Runtime.Remoting.Contexts;
using CoreGraphics;
using Google.MobileAds;
using TrainingDay.iOS.Services;
using TrainingDay.Views.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobViewRenderer))]
namespace TrainingDay.iOS.Services
{
    public class AdMobViewRenderer : ViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                BannerView bannerView = new BannerView(AdSizeCons.Banner, new CGPoint(0, 0));
                bannerView.AdUnitId = (Element as AdMobView).AdUnitId;
                foreach (UIWindow uiWindow in UIApplication.SharedApplication.Windows)
                {
                    if (uiWindow.RootViewController != null)
                    {
                        bannerView.RootViewController = uiWindow.RootViewController;
                    }
                }
                var request = Request.GetDefaultRequest();
                bannerView.LoadRequest(request);
                SetNativeControl(bannerView);

            }

        }
    }
}