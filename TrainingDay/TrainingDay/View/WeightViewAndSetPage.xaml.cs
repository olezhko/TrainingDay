using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
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