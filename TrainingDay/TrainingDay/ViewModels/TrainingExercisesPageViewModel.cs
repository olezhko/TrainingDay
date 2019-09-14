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
using Xamarin.Forms.Internals;

namespace TrainingDay.ViewModels
{
    class TrainingExercisesPageViewModel:BaseViewModel
    {
        public ObservableCollection<TrainingExerciseViewModel> Items { get; set; }
        public INavigation Navigation { get; set; }
        public TrainingViewModel Training { get; set; }
        public TrainingExercisesPageViewModel()
        {
            RefreshCommand = new Command(Refresh);
            SaveChangesCommand = new Command(SaveChanges);
            Items = new ObservableCollection<TrainingExerciseViewModel>();
        }

        public bool IsRefreshing { get; set; }
        public ICommand RefreshCommand { get; set; }
        private void Refresh()
        {
            IsRefreshing = true;
            OnPropertyChanged(nameof(IsRefreshing));

            Load(Training);
            IsRefreshing = false;
            OnPropertyChanged(nameof(IsRefreshing));
        }

        public void Load(TrainingViewModel trVm)
        {
            if (trVm == null)
            {
                return;
            }
            Training = new TrainingViewModel
            {
                Title = trVm.Title,
                Exercises = new ObservableCollection<TrainingExerciseViewModel>(trVm.Exercises),
                Id = trVm.Id
            };

            Items = Training.Exercises;
            OnPropertyChanged(nameof(Items));
        }

        public ICommand SaveChangesCommand { get; set; }
        private void SaveChanges()
        {
            // save training name
            var id = App.Database.SaveTrainingItem(new Training() { Id = Training.Id, Title = Training.Title });

            // clear all exer-tr communication with selected training id
            var trainingExerciseItems = App.Database.GetTrainingExerciseItems(); // get all tr-exercises comm
            foreach (var trainingExerciseItem in trainingExerciseItems)
            {
                if (trainingExerciseItem.TrainingId == Training.Id)
                {
                    App.Database.DeleteTrainingExerciseItem(trainingExerciseItem.Id);
                }
            }

            // save every exercise
            int order = 0;
            foreach (var exerciseSelectViewModel in Items)
            {
                var exId = App.Database.SaveExerciseItem(exerciseSelectViewModel.GetExercise());
                // save order numbers
                order++;
                App.Database.SaveTrainingExerciseItem(new TrainingExerciseComm()
                {
                    ExerciseId = exId,
                    TrainingId = id,
                    OrderNumber = order,
                    Weight = exerciseSelectViewModel.Weight,
                    CountOfApproches = exerciseSelectViewModel.CountOfApproches,
                    CountOfTimes = exerciseSelectViewModel.CountOfTimes,
                });
            }

            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
            App.Current.MainPage = new MainPage();
        }

        public ICommand AddExercisesCommand => new Command(AddExercises);
        private async void AddExercises()
        {
            var vm = new ExerciseListPageViewModel() { Navigation = Navigation };
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
                var selectedItems = obj.GetSelectedItems();
                selectedItems.ForEach(a=>a.IsSelected = false);
                foreach (var exerciseItem in selectedItems)
                {
                    Items.Add(exerciseItem);
                }
            }
            OnPropertyChanged(nameof(Items)); 
        }

        public ICommand MakeNotifyCommand => new Command(MakeNotify);
        private void MakeNotify()
        {
            MakeTrainingAlarmPageViewModel vm = new MakeTrainingAlarmPageViewModel() { Navigation = Navigation};
            vm.Alarm.Training = Training;
            MakeTrainingAlarmPage page = new MakeTrainingAlarmPage() { BindingContext = vm };
            Navigation.PushAsync(page, true);
        }


        public ICommand ItemTappedCommand => new Command<ItemTappedEventArgs>(ItemTapped);
        private async void ItemTapped(ItemTappedEventArgs e)
        {
            TrainingExerciseViewModel selected = e.Item as TrainingExerciseViewModel;
            TrainingExerciseItemPage page = new TrainingExerciseItemPage();
            page.LoadExercise(selected);
            await Navigation.PushAsync(page);
        }

        public ICommand DeleteExerciseCommand => new Command<TrainingExerciseViewModel>(DeleteExercise);
        private void DeleteExercise(TrainingExerciseViewModel sender)
        {
            QuestionPopup popup = new QuestionPopup(Resource.DeleteExercises, Resource.AreYouSerious);
            popup.PopupClosed += (o, closedArgs) =>
            {
                if (closedArgs.Button == "OK")
                {
                    Items.Remove(sender); // we must delete only from list; from base we delete in SaveChanges
                }
            };
            popup.Show();
        }


        public ICommand MakeTrainingCommand => new Command(MakeTraining);
        private void MakeTraining()
        {
            DependencyService.Get<IMessage>().ShowMessage(Resource.AdviceBeforeTrainingMessage, Resource.AdviceString);
            Training.Exercises.ForEach(item => item.IsNotFinished = true);
            Navigation.PushAsync(new TrainingImplementPage() { TrainingItem = Training, Title = Training.Title });
        }
    }
}
