using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Foundation;
using TrainingDay.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(TrainingDay.iOS.Services.Localize))]
namespace TrainingDay.iOS.Services
{
    public class Localize : ILocalize
    {
        public System.Globalization.CultureInfo GetCurrentCultureInfo()
        {
            var netLanguage = "en";
            var prefLanguage = "en-US";
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];
                netLanguage = pref.Replace("_", "-"); // заменяет pt_BR на pt-BR
            }
            
            System.Globalization.CultureInfo ci = null;
            try
            {
                ci = new System.Globalization.CultureInfo(netLanguage);
            }
            catch(Exception ex)
            {
                ci = new System.Globalization.CultureInfo(prefLanguage);
            }

            return CultureInfo.CurrentCulture;
        }

        public string GetCurrentLanguage()
        {
            var netLanguage = "en";
            var prefLanguage = "en-US";
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];
                netLanguage = pref.Replace("_", "-"); // заменяет pt_BR на pt-BR
            }

            return CultureInfo.CurrentCulture.DisplayName;
        }
    }
}