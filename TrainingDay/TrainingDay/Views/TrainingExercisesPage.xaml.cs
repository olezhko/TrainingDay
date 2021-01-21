using System;
using System.Linq;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ItemsListView.SelectedItem = null;
        }

        private void Load(int id)
        {
            TrainingViewModel trVm = new TrainingViewModel(App.Database.GetTrainingItem(id));
            PrepareTrainingViewModel(trVm);
            TrainingExercisesPageViewModel vm = new TrainingExercisesPageViewModel()
            {
                Navigation = Navigation
            };
            vm.Load(trVm);
        }

        public static void PrepareTrainingViewModel(TrainingViewModel vm)
        {
            var trainingExerciseItems = App.Database.GetTrainingExerciseItems();
            var exerciseItems = App.Database.GetExerciseItems();
            var trainingExercises = trainingExerciseItems.Where(ex => ex.TrainingId == vm.Id);
            var unOrderedItems = trainingExercises.Where(a => a.OrderNumber < 0);

            trainingExercises = trainingExercises.OrderBy(a => a.OrderNumber).Where(a => a.OrderNumber >= 0).ToList();
            int index = 0;
            foreach (var trainingExercise in trainingExercises)
            {
                var exercise = exerciseItems.First(ex => ex.Id == trainingExercise.ExerciseId);
                var trEx = new TrainingExerciseViewModel(exercise, trainingExercise)
                {
                    TrainingExerciseId = trainingExercise.Id,
                };
                index++;

                vm.AddExercise(trEx);
            }

            foreach (var trainingExercise in unOrderedItems)
            {
                if (trainingExercise.OrderNumber == -1)
                {
                    trainingExercise.OrderNumber = index;
                    App.Database.SaveTrainingExerciseItem(trainingExercise);
                }
                var exercise = exerciseItems.First(ex => ex.Id == trainingExercise.ExerciseId);
                var trEx = new TrainingExerciseViewModel(exercise, trainingExercise)
                {
                    TrainingExerciseId = trainingExercise.Id,
                };
                index++;
                vm.AddExercise(trEx);
            }
        }
    }
}