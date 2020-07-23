using System;
using System.Linq;
using TrainingDay.Resources;
using TrainingDay.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MakeTrainingAlarmPage : ContentPage
	{
		public MakeTrainingAlarmPage ()
		{
			InitializeComponent ();
        }

	    protected override void OnBindingContextChanged()
	    {
	        base.OnBindingContextChanged();
	        var vm = BindingContext as MakeTrainingAlarmPageViewModel;
            // need to raise values

            try
            {
                if (vm.TrainingItems != null && vm.Alarm.TrainingId != 0)
                    TrainingsPicker.SelectedIndex = vm.TrainingItems.IndexOf(vm.TrainingItems.First(a => a.Id == vm.Alarm.TrainingId));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

	        DaysOfWeekSelection.Days = vm.Alarm.Days;
	    }
	}
}