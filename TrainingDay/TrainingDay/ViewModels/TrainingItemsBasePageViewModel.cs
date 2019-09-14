using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.View;
using TrainingDay.Views;
using TrainingDay.Views.Controls;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    // ViewModel for view all Trainings
    class TrainingItemsBasePageViewModel:BaseViewModel
    {
        public ObservableCollection<TrainingViewModel> Items { get; set; }
        public States State { get; set; } = States.NoData;
        public INavigation Navigation { get; set; }
        public TrainingItemsBasePageViewModel()
        {
            Items = new ObservableCollection<TrainingViewModel>();
            AddNewTrainingCommand = new Command(AddNewTraining);
            DeleteSelectedTrainingsCommand = new Command<ViewCell>(DeleteSelectedTrainings);
            ItemSelectedCommand = new Command<SelectedItemChangedEventArgs>(ShowTrainingExercieses);
        }

        public void LoadItems()
        {
            State = States.Loading;
            OnPropertyChanged(nameof(State));

            Items.Clear();
            var trainingsItems = App.Database.GetTrainingItems(); // get list of trainings

            if (trainingsItems != null && trainingsItems.Any())
            {
                foreach (var training in trainingsItems)
                {
                    if (!Items.Any(a => a.Id == training.Id))
                    {
                        Items.Add(new TrainingViewModel(training) { });
                    }
                }
            }

            OnPropertyChanged(nameof(Items));

            State = Items.Any() ? States.Normal : States.NoData;
            OnPropertyChanged(nameof(State));
        }

        public ICommand AddNewTrainingCommand { get; set; }
        private void AddNewTraining()
        {
            PreparedTrainingsPageViewModel vm = new PreparedTrainingsPageViewModel(){Navigation = Navigation};
            Navigation.PushAsync(new PreparedTrainingsPage(){BindingContext = vm});
        }

        public ICommand DeleteSelectedTrainingsCommand { get; set; }
        private void DeleteSelectedTrainings(ViewCell viewCell)
        {
            State = States.Loading;
            OnPropertyChanged(nameof(State));

            viewCell.ContextActions.Clear();
            var item = (TrainingViewModel)viewCell.BindingContext;
            App.Database.DeleteTrainingItem(item.Id);
            App.Database.DeleteTrainingExerciseItemByTraningId(item.Id);
            DeleteTrainingAlarms(item);
            DependencyService.Get<IMessage>().ShortAlert(Resource.DeletedString);
            LoadItems();
        }

        private void DeleteTrainingAlarms(TrainingViewModel item)
        {
            var alarms = App.Database.GetAlarmItems();
            foreach (var alarm in alarms)
            {
                if (alarm.TrainingId == item.Id)
                {
                    App.Database.DeleteAlarmItem(alarm.Id);
                }
            }
        }

        private void PrepareTrainingViewModel(TrainingViewModel vm)
        {
            var trainingExerciseItems = App.Database.GetTrainingExerciseItems();
            var exerciseItems = App.Database.GetExerciseItems();
            var allExercises = trainingExerciseItems.Where(ex => ex.TrainingId == vm.Id).ToList();
            foreach (var allExercise in allExercises)
            {
                var exer = exerciseItems.First(ex => ex.Id == allExercise.ExerciseId);
                vm.Exercises.Add(
                    new TrainingExerciseViewModel(exer, allExercise)
                    {
                        TrainingExerciseId = allExercise.Id
                    });
            }
        }


        public ICommand ItemSelectedCommand { get; set; }
        private void ShowTrainingExercieses(SelectedItemChangedEventArgs parameter)
        {
            TrainingViewModel trVm = parameter.SelectedItem as TrainingViewModel;
            PrepareTrainingViewModel(trVm);

            TrainingExercisesPageViewModel vm = new TrainingExercisesPageViewModel()
            {
                Navigation = Navigation
            };
            vm.Load(trVm);
            Navigation.PushAsync(new TrainingExercisesPage() {BindingContext = vm});
        }
    }

    public class TrainingViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public ObservableCollection<TrainingExerciseViewModel> Exercises { get; set; }

        public TrainingViewModel()
        {
            Exercises = new ObservableCollection<TrainingExerciseViewModel>();
        }

        public TrainingViewModel(Training tr)
        {
            Title = tr.Title;
            Id = tr.Id;
            Exercises = new ObservableCollection<TrainingExerciseViewModel>();
        }

        public byte[] GetBytes()
        {
            return new byte[1];
        }

        public void LoadFromBytes(byte[] data)
        {

        }
    }

}
