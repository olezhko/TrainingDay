using TrainingDay.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTrainingPage : ContentPage
    {
        AddTrainingViewModel viewModel = new AddTrainingViewModel();
        public AddTrainingPage()
        {
            InitializeComponent();
            ItemsListView.DragDropController.UpdateSource = true;
            viewModel.Navigation = this.Navigation;
            BindingContext = viewModel;
        }
    }
}