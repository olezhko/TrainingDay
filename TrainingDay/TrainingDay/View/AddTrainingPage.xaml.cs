using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.Model;
using TrainingDay.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTrainingPage : ContentPage
    {
        AddTrainingViewModel viewModel = new AddTrainingViewModel();
        public AddTrainingPage()
        {
            InitializeComponent();
            viewModel.Navigation = this.Navigation;
            BindingContext = viewModel;
        }
    }
}