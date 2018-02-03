using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using TrainingDay.Helpers;
using TrainingDay.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.View
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
            if (vm!=null)
            {
                vm.WeightPeriodChanged();
            }
        }
    }
}