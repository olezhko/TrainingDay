using TrainingDay.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrainingAlarmListPage : ContentPage
    {
        private TrainingAlarmListPageViewModel vm;
        public TrainingAlarmListPage()
        {
            InitializeComponent();
            vm = BindingContext as TrainingAlarmListPageViewModel;
            vm.Navigation = Navigation;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            vm.LoadItems();
        }
    }
}