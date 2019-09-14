using TrainingDay.Services;
using TrainingDay.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrainingItemsBasePage : ContentPage
    {
        public TrainingItemsBasePage()
        {
            InitializeComponent();
            BindingContext = new TrainingItemsBasePageViewModel(){Navigation = this.Navigation };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var vm = BindingContext as TrainingItemsBasePageViewModel;
            vm.LoadItems();
        }
    }
}