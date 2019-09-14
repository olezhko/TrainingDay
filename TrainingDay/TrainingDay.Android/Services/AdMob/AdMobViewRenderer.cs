using System.ComponentModel;
using Android.Content;
using Android.Gms.Ads;
using Android.Widget;
using TrainingDay.Droid.Services;
using TrainingDay.Views.Controls;
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
            var req = new AdRequest.Builder().AddTestDevice("7BAA90D16987E6F2CCC83017A97E7A73").AddTestDevice("DDC1D3A6C0ABF2EF62F5439DA8BA4C4D").Build();
            adView.LoadAd(req);
            return adView;
        }
    }
}