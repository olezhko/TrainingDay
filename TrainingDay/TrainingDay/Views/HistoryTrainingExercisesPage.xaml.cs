using TrainingDay.ViewModels;
using TrainingDay.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryTrainingExercisesPage : ContentPage
    {
        public HistoryTrainingExercisesPage()
        {
            InitializeComponent();
        }

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    var vm = BindingContext as HistoryTrainingPageViewModel;
        //    vm.Navigation = Navigation;
        //    vm.LoadItems();
        //}

    }
}