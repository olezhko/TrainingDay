using System;
using System.Globalization;
using TrainingDay.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(TrainingDay.Droid.Services.Localize))]

namespace TrainingDay.Droid.Services
{
    public class Localize : ILocalize
    {
        public System.Globalization.CultureInfo GetCurrentCultureInfo()
        {
            var androidLocale = Java.Util.Locale.Default;
            var netLanguage = androidLocale.ToString().Replace("_", "-");
            CultureInfo cu;
            try
            {
                cu = new System.Globalization.CultureInfo(netLanguage);
            }
            catch (CultureNotFoundException ex)
            {
                netLanguage = netLanguage.Substring(0, 2) + "-" + netLanguage.Substring(0, 2).ToUpper();
                cu = new System.Globalization.CultureInfo(netLanguage);
            }

            return cu;
        }

        public string GetCurrentLanguage()
        {
            var androidLocale = Java.Util.Locale.Default;
            return androidLocale.Language;
        }
    }
}