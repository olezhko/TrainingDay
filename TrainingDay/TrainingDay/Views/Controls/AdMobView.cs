using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TrainingDay.Views.Controls
{
    public class AdMobView : Xamarin.Forms.View
    {
        public static readonly BindableProperty AdUnitIdProperty = BindableProperty.Create(
            nameof(AdUnitId),
            typeof(string),
            typeof(AdMobView),
            string.Empty);

        public string AdUnitId
        {
            get => (string)GetValue(AdUnitIdProperty);
            set => SetValue(AdUnitIdProperty, value);
        }

        public AdMobView()
        {
#if DEBUG
            IsVisible = false;
#endif
        }
    }

    public interface IAdInterstitial
    {
        void ShowAd(string url);
    }
}
