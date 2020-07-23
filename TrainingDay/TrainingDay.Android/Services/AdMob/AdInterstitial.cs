using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TrainingDay.Droid.Services.AdMob;
using TrainingDay.Views.Controls;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(AdInterstitial))]
namespace TrainingDay.Droid.Services.AdMob
{
    class AdInterstitial : IAdInterstitial
    {
        InterstitialAd _ad;
        public void ShowAd(string adUnit)
        {
            var context = Application.Context;
            _ad = new InterstitialAd(context);
            _ad.AdUnitId = adUnit;

            var intlistener = new InterstitialAdListener(_ad);
            intlistener.OnAdLoaded();
            _ad.AdListener = intlistener;

            var requestbuilder = new AdRequest.Builder().AddTestDevice("7BAA90D16987E6F2CCC83017A97E7A73").AddTestDevice("DDC1D3A6C0ABF2EF62F5439DA8BA4C4D");
            _ad.LoadAd(requestbuilder.Build());
        }
    }


    public class InterstitialAdListener : AdListener
    {
        readonly InterstitialAd _ad;

        public InterstitialAdListener(InterstitialAd ad)
        {
            _ad = ad;
        }

        public override void OnAdLoaded()
        {
            base.OnAdLoaded();

            if (_ad.IsLoaded)
                _ad.Show();
        }
    }
}