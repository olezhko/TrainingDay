using TrainingDay.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrainingExercisesPage : ContentPage
    {
        public TrainingExercisesPage()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            var vm = BindingContext as TrainingExercisesPageViewModel;
            if (vm.Training == null || vm.Training.Id == 0)
            {
                ToolbarItems.Remove(AddAlarmToolbarItem);
                MakeTrainingButton.IsVisible = false;
            }
            else
                MakeTrainingButton.IsVisible = true;
        }
    }
}