using TrainingDay.ViewModels;
using TrainingDay.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryTrainingPage : ContentPage
    {
        public HistoryTrainingPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var vm = BindingContext as HistoryTrainingPageViewModel;
            vm.Navigation = Navigation;
            vm.LoadItems();
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = BindingContext as HistoryTrainingPageViewModel;
            if (vm.State == States.View2) // details view
            {
                vm.ChangeState(vm.LastTrainings.Count == 0 ? States.NoData : States.View1); // nodataview or lasttrainingsview
                return true;//  мы хотим остаться на странице
            }

            return base.OnBackButtonPressed();
        }
    }
}