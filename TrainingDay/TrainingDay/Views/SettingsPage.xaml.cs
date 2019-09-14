using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XLabs;

namespace TrainingDay.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage ()
		{
			InitializeComponent ();
            ThemeSwitch.IsToggled = ((App.Current) as App).IsLightTheme;
		    WeightNotifyCheckBox.Checked =  Settings.IsWeightNotify;
        }


        private void Switch_OnToggled(object sender, ToggledEventArgs e)
        {
            ((App.Current) as App).IsLightTheme = ThemeSwitch.IsToggled; ChangesHideLabel.IsVisible = true;
            Device.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                ChangesHideLabel.IsVisible = false;
                return false;
            });
        }

	    private void WeightNotifyChanged(object sender, EventArgs<bool> e)
	    {
	        Settings.IsWeightNotify = WeightNotifyCheckBox.Checked;
	    }

	    private void FreqWeightNotify_OnTextChanged(object sender, TextChangedEventArgs e)
	    {
	        bool res = int.TryParse(FreqWeightNotify.Text, out var newFreq);
	        if (res)
	        {
	            if (newFreq < 1)
	            {
	                FreqWeightNotify.Text = Settings.WeightNotifyFreq.ToString();
                }
	            else
	            {
	                Settings.WeightNotifyFreq = newFreq;
	            }
            }
	        else
	        {
	            FreqWeightNotify.Text = Settings.WeightNotifyFreq.ToString();
	        }
	    }
	}
}