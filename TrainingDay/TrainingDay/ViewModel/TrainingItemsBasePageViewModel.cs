using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TrainingDay.Code;
using TrainingDay.Model;
using TrainingDay.View;
using Xamarin.Forms;

namespace TrainingDay.ViewModel
{
    // ViewModel for view all Trainings
    class TrainingItemsBasePageViewModel:BaseViewModel
    {
        public TrainingItemsBasePageViewModel()
        {
            Items = new ObservableCollection<TrainingViewModel>();
            DeleteSelectedTrainingsCommand = new Command(DeleteSelectedTrainings);
            ShowTrainingExercieseCommand = new Command(ShowTrainingExercieses);
            TrainingItemSelectedCommand = new Command(EditSelectedTraining);
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
                        Items.Add(new TrainingViewModel() { Id = training.Id,Description = training.Description,Title = training.Title});
                    }

                    var allExercises = trainingExerciseItems.Where(ex => ex.TrainingId == training.Id).ToList();
                    var trainingViewModel = Items.First(tr => tr.Id == training.Id);
                    foreach (var allExercise in allExercises)
                    {
                        trainingViewModel.Exercises.Add(exerciseItems.First(ex=>ex.Id == allExercise.ExerciseId));
                    }
                }
            }
            OnPropertyChanged(nameof(Items));
        }

        private void ShowTrainingExercieses(object parameter)
        {
            if (vm != null)
                vm.TrainingExercisesChanges -= VmOnTrainingExercisesChanges;

            TrainingViewModel trVM = parameter as TrainingViewModel;
            vm = new TrainingExercisesPageViewModel()
            {
                Navigation = Navigation, TrainingID = trVM.Id
            };
            vm.TrainingExercisesChanges += VmOnTrainingExercisesChanges;
            Navigation.PushAsync(new TrainingExercisesPage() {BindingContext = vm});
        }

        private void VmOnTrainingExercisesChanges(object sender, EventArgs eventArgs)
        {
            TrainingExercisesPageViewModel vmSender = sender as TrainingExercisesPageViewModel;
            int id = vmSender.TrainingID;
            var collExercise = vmSender.Items;
            App.Database.DeleteTrainingItem(id);
            foreach (var exerciseSelectViewModel in collExercise)
            {
                App.Database.SaveTrainingExerciseItem(new TrainingExerciseComm()
                {
                    TrainingId = id,
                    ExerciseId = exerciseSelectViewModel.Id
                });
            }
            DependencyService.Get<IMessage>().ShortAlert("Сохранено");
        }

        private void DeleteSelectedTrainings()
        {
            bool isDeleted = false;
            foreach (var trainingViewModel in Items)
            {
                if (trainingViewModel.IsSelected)
                {
                    App.Database.DeleteTrainingItem(trainingViewModel.Id);
                    App.Database.DeleteTrainingExerciseItemByTraningId(trainingViewModel.Id);
                    isDeleted = true;
                }
            }
            if (isDeleted)
            {
                DependencyService.Get<IMessage>().ShortAlert("Удалено");
                LoadItems();
            }
        }

        private void EditSelectedTraining()
        {
            AddTrainingViewModel viewModel = new AddTrainingViewModel();
            viewModel.TrainingItem = new TrainingItem()
            {
                Description = SelectedTraining.Description,
                Title = SelectedTraining.Title,
                Exercises = new ObservableCollection<Exercise>(SelectedTraining.Exercises)
            };
            viewModel.Navigation = Navigation;
            Navigation.PushAsync(new AddTrainingPage() {BindingContext = viewModel});
        }




        private Command _itemSelectedCommand;
        public ICommand ItemSelectedCommand
        {
            get
            {
                if (_itemSelectedCommand == null)
                {
                    _itemSelectedCommand = new Command(ShowTrainingExercieses);
                }

                return _itemSelectedCommand;
            }
        }

        public INavigation Navigation;
        public ICommand DeleteSelectedTrainingsCommand { get; set; }
        public ICommand ShowTrainingExercieseCommand { get; set; }
        public ICommand TrainingItemSelectedCommand { get; set; }

        public TrainingViewModel SelectedTraining { get; set; }
        public ObservableCollection<TrainingViewModel> Items { get; set; }
        private TrainingExercisesPageViewModel vm;
    }

    public class TrainingViewModel
    {
        public bool IsSelected { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public ObservableCollection<Exercise> Exercises { get; set; }

        public TrainingViewModel()
        {
            Exercises = new ObservableCollection<Exercise>();
        }
    }

}
