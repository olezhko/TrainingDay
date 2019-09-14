using System.Globalization;
using Plugin.LocalNotifications;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.ViewModels;
using TrainingDay.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeightViewAndSetPage : ContentPage
    {
        private WeightViewAndSetPageViewModel vm;
        public WeightViewAndSetPage()
        {
            InitializeComponent();
            PeriodPicker.Items.Add(Resource.WeekString);
            PeriodPicker.Items.Add(Resource.TwoWeeksString);
            PeriodPicker.Items.Add(Resource.OneMounthString);
            PeriodPicker.Items.Add(Resource.TwoMounthString);
            PeriodPicker.Items.Add(Resource.ThreeMounthString);
            PeriodPicker.Items.Add(Resource.HalfYearString);
            PeriodPicker.Items.Add(Resource.YearString);
            vm = new WeightViewAndSetPageViewModel();
            BindingContext  = vm;
            CrossLocalNotifications.Current.Cancel(App.WeightNotificationId);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm?.WeightPeriodChanged();
        }
    }
}