using System;
using TrainingDay.Resources;
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
            ItemsListView.DragDropController.UpdateSource = true;
        }

        private bool _isShown = false;
        public bool IsShowDeleteUnusefulExercisesHelp = false;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (IsShowDeleteUnusefulExercisesHelp && !_isShown)
            {
                _isShown = true;
                var vm = BindingContext as TrainingExercisesPageViewModel;
                
                vm.StartSelectExercises();
                
                DisplayAlert(Resource.AdviceString, Resource.HelpDeleteUnusefulExercises, "OK");
            }
            ItemsListView.SelectedItem = null;
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