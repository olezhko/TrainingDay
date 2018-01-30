using System.Globalization;
using TrainingDay.Model;
using Xamarin.Forms;
[assembly: Dependency(typeof(TrainingDay.Helpers.Localize))]
namespace TrainingDay.Helpers
{
    public class Localize : ILocalize
    {
        public System.Globalization.CultureInfo GetCurrentCultureInfo()
        {
            return CultureInfo.CurrentUICulture;
        }
    }
}
