using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
		public SettingsPage ()
		{
			InitializeComponent ();
            ThemeSwitch.IsToggled = ((App.Current) as App).IsLightTheme;
		    WeightNotifyCheckBox.IsToggled =  Settings.IsWeightNotify;
            FreqWeightNotify.Text = Settings.WeightNotifyFreq.ToString();

		    ScreenOnImplementedSwitch.IsToggled = Settings.IsDisplayOnImplement;
            TokenEditor.Text = Settings.Token;
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


	    private void WeightNotifyCheckBox_OnToggled(object sender, ToggledEventArgs e)
	    {
	        Settings.IsWeightNotify = WeightNotifyCheckBox.IsToggled;
        }

        private void FreqWeightNotify_OnUnfocused(object sender, FocusEventArgs e)
        {
            if (!e.IsFocused)
            {
                bool res = uint.TryParse(FreqWeightNotify.Text, out var newFreq);
                if (res)
                {
                    if (newFreq < 1)
                    {
                        FreqWeightNotify.Text = Settings.WeightNotifyFreq.ToString();
                    }
                    else
                    {
                        Settings.WeightNotifyFreq = (int)newFreq;
                    }
                }
                else
                {
                    FreqWeightNotify.Text = Settings.WeightNotifyFreq.ToString();
                }
            }
        }

        private void ScreenOnImplementedSwitch_OnToggled(object sender, ToggledEventArgs e)
        {
            Settings.IsDisplayOnImplement = ScreenOnImplementedSwitch.IsToggled;
        }

        private void ClearHistoryTrainings_Click(object sender, EventArgs e)
        {
            App.Database.DeleteAll<LastTraining>();
            App.Database.DeleteAll<LastTrainingExercise>();
        }
    }
}