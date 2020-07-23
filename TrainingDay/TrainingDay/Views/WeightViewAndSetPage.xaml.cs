using System;
using System.Collections.Generic;
using OxyPlot;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.ViewModels;
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
            vm?.WeightPeriodChanged();
            PlotView.Controller = new PlotController();
            PlotView.Controller.UnbindAll();
            PlotView.Controller.BindTouchDown(PlotCommands.PointsOnlyTrackTouch);
        }

        private bool _isVisibleWeightHistory;
        private void ShowWeightHistory_Click(object sender, EventArgs e)
        {
            _isVisibleWeightHistory = !_isVisibleWeightHistory;
            WeightGrid.IsVisible = !_isVisibleWeightHistory;
            WeightItemsList.IsVisible = _isVisibleWeightHistory;
            ShowWeightHistoryMenu.Text = _isVisibleWeightHistory ? Resource.WeightString : Resource.HistoryString;
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();

        }
    }
}