using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Newtonsoft.Json;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.View;
using TrainingDay.Views;
using TrainingDay.Views.Controls;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    public class AddTrainingViewModel:BaseViewModel
    {
        public ObservableCollection<TrainingExerciseViewModel> ExerciseItems { get; set; }
        public INavigation Navigation { get; set; }
        public string Title { get; set; }
        public AddTrainingViewModel()
        {
            ExerciseItems = new ObservableCollection<TrainingExerciseViewModel>();
            AddTrainingItemCommand = new Command(AddTrainingItemMethod);
            SelectExercisesCommand = new Command(SelectExercisesMethod);
        }

        public ICommand SelectExercisesCommand { get; set; }
        private async void SelectExercisesMethod()
        {
            var vm = new ExerciseListPageViewModel() {Navigation = Navigation};
            vm.ExercisesChousen += (sender, args) =>
            {
                ChoseExercises(vm);
            };
            await Navigation.PushAsync(new ExerciseListPage(vm));
        }

        private void ChoseExercises(ExerciseListPageViewModel obj)
        {
            if (obj != null)
            {
                var items = obj.GetSelectedItems();
                foreach (var exerciseItem in items)
                {
                    ExerciseItems.Add(exerciseItem);
                }
            }
            OnPropertyChanged(nameof(ExerciseItems));
        }



        public ICommand DeleteExerciseCommand => new Command<TrainingExerciseViewModel>(DeleteExercise);
        private void DeleteExercise(object o)
        {
            var sender = o as TrainingExerciseViewModel;
            ExerciseItems.Remove(sender);
        }

        public ICommand AddTrainingItemCommand { get; set; }
        private void AddTrainingItemMethod()
        {
            if (string.IsNullOrEmpty(Title) || ExerciseItems.Count == 0)
            {
                DependencyService.Get<IMessage>().ShortAlert(Resource.EmptyFields);
                return;
            }
            App.Database.SaveTrainingItem(new Training()
            {
                Title = Title
            });

            int newId = App.Database.GetLastInsertId();
            int orderNumber = 0;
            foreach (var trainingItemExercise in ExerciseItems)
            {
                App.Database.SaveTrainingExerciseItem(new TrainingExerciseComm()
                {
                    ExerciseId = trainingItemExercise.ExerciseId, TrainingId = newId, OrderNumber = orderNumber,
                    WeightAndRepsString = JsonConvert.SerializeObject(trainingItemExercise.WeightAndRepsItems)
                });
                orderNumber++;
            }

            DependencyService.Get<IMessage>().ShortAlert(Resource.AddedString);
            Navigation.PopAsync(false);
            Navigation.PopAsync(false);
        }
    }
}
