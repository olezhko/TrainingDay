using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.Code;

namespace TrainingDay.ViewModel
{
    public class MakeTrainingPageViewModel:BaseViewModel
    {
        public bool IsVisibleNoTrainingsNeedAddNewLabel { get; set; }
        public ObservableCollection<TrainingViewModel> Items { get; set; }
        public MakeTrainingPageViewModel()
        {
            Items = new ObservableCollection<TrainingViewModel>();
        }

        public void LoadItems()
        {
            Items.Clear();
            var trainingExerciseItems = App.Database.GetTrainingExerciseItems();
            var exerciseItems = App.Database.GetExerciseItems();
            var trainingsItems = App.Database.GetTrainingItems();

            if (trainingsItems != null && trainingsItems.Any())
            {
                foreach (var training in trainingsItems)
                {
                    if (!Items.Any(a => a.Id == training.Id))
                    {
                        Items.Add(new TrainingViewModel() { Id = training.Id, Description = training.Description, Title = training.Title });
                    }

                    var allExercises = trainingExerciseItems.Where(ex => ex.TrainingId == training.Id).ToList();
                    var trainingViewModel = Items.First(tr => tr.Id == training.Id);
                    foreach (var allExercise in allExercises)
                    {
                        trainingViewModel.Exercises.Add(exerciseItems.First(ex => ex.Id == allExercise.ExerciseId));
                    }
                }
            }
            IsVisibleNoTrainingsNeedAddNewLabel = Items.Any();
            OnPropertyChanged(nameof(IsVisibleNoTrainingsNeedAddNewLabel));
            DrawItems();
        }

        private void DrawItems()
        {
            foreach (var trainingViewModel in Items)
            {
                
            }
        }
    }
}
