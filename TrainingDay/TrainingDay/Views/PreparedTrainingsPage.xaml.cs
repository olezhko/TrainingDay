using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreparedTrainingsPage : ContentPage
    {
        public PreparedTrainingsPage()
        {
            InitializeComponent();
            viewModel = BindingContext as PreparedTrainingsPageViewModel;
        }

        private PreparedTrainingsPageViewModel viewModel;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.FillTrainings();
        }
    }
}