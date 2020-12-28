using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microcharts;
using SkiaSharp;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.ViewModels;
using Xamarin.Essentials;
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
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm?.OnAppearing();

            //vm?.LoadWeightPlot();
            //vm?.LoadWaistPlot();
            //vm?.LoadHipsPlot();
        }

        private bool _isVisibleWeightHistory;
        private void ShowWeightHistory_Click(object sender, EventArgs e)
        {
            _isVisibleWeightHistory = !_isVisibleWeightHistory;
            WeightGrid.IsVisible = !_isVisibleWeightHistory;
            //WeightItemsList.IsVisible = _isVisibleWeightHistory;
            //ShowWeightHistoryMenu.Text = _isVisibleWeightHistory ? Resource.WeightString : Resource.HistoryString;
        }

        private async void ShowInfo_Click(object sender, EventArgs e)
        {
            var coef = vm.CurrentWaistValue / vm.CurrentHipsValue;
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(Resource.WaistHipMessage.Replace("_", "\n"), coef));

            var result = await DisplayAlert(Resource.HelperString, sb.ToString(), Resource.OkString,Resource.CancelString);
            if (result)
            {
                await Browser.OpenAsync(@"http://trainingday.tk/waist-hip?year=2020&Month=10", BrowserLaunchMode.SystemPreferred);
            }
        }
    }
}