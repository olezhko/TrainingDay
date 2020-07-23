using System.ComponentModel;
using Android.Content;
using Android.Gms.Ads;
using Android.Widget;
using TrainingDay.Droid.Services;
using TrainingDay.Views.Controls;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AdMobView), typeof(AdMobViewRenderer))]
namespace TrainingDay.Droid.Services
{
    public class AdMobViewRenderer : ViewRenderer<AdMobView, AdView>
    {
        public AdMobViewRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && Control == null)
            {
                SetNativeControl(CreateAdView());
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(AdView.AdUnitId))
                Control.AdUnitId = Element.AdUnitId;
        }

        private AdView CreateAdView()
        {
            var adView = new AdView(Context)
            {
                AdSize = AdSize.Banner,
                AdUnitId = Element.AdUnitId
            };

            adView.LayoutParameters = new LinearLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
            //var req = new AdRequest.Builder().Build();
            var req = new AdRequest.Builder().AddTestDevice("F9F014E2F2BD39F477D3858D05F1672D").AddTestDevice("8D0331187890D7FB1B54CDF5BB913B08").Build();
            adView.LoadAd(req);
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                int heightPixels = AdSize.Banner.GetHeightInPixels(this.Context);
                adView.SetMinimumHeight(heightPixels);
            }

            return adView;
        }
    }
}