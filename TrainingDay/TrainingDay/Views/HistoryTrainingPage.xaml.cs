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
            if (vm.State == States.View2)
            {
                vm.ChangeState(States.View1);// хотим открыть список выполненных упражнений
                return true;//  мы хотим остаться на странице
            }

            return base.OnBackButtonPressed();
        }
    }
}