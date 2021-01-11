using System;
using TrainingDay.Resources;
using TrainingDay.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreparedTrainingExercisesPage : ContentPage
    {
        public PreparedTrainingExercisesPage()
        {
            InitializeComponent();
            ItemsListView.DragDropController.UpdateSource = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var vm = BindingContext as TrainingExercisesPageViewModel;
            vm.StartSelectExercises();
            DisplayAlert(Resource.AdviceString, Resource.HelpDeleteUnusefulExercises, "OK");
            ItemsListView.SelectedItem = null;
        }
    }
}