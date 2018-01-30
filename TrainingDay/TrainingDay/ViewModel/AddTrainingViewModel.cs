using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TrainingDay.Annotations;
using TrainingDay.Model;
using TrainingDay.View;
using Xamarin.Forms;

namespace TrainingDay.ViewModel
{
    public class AddTrainingViewModel:INotifyPropertyChanged
    {
        public ICommand SelectExercisesCommand { get; set; }
        public ICommand AddTrainingItemCommand { get; set; }
        public TrainingItem TrainingItem { get; set; }
        public INavigation Navigation { get; set; }
        public ICommand ChoseExercisesCommand { protected set; get; }

        public AddTrainingViewModel()
        {
            TrainingItem = new TrainingItem();
            ExerciseItems = new ObservableCollection<Exercise>();
            AddTrainingItemCommand = new Command(AddTrainingItemMethod);
            SelectExercisesCommand = new Command(SelectExercisesMethod);
            ChoseExercisesCommand = new Command(ChoseExercises);
        }

        private async void SelectExercisesMethod()
        {
            await Navigation.PushAsync(new ExerciseListPage(new ExerciseListPageViewModel() { ListViewModel = this }));
        }

        private void ChoseExercises(object obj)
        {
            ExerciseItems.Clear();
            ExerciseListPageViewModel vm = obj as ExerciseListPageViewModel;
            if (obj != null)
            {
                var items = vm.GetSelectedItems();
                foreach (var exerciseItem in items)
                {
                    try
                    {
                        ExerciseItems.Add(exerciseItem);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            Navigation.PopAsync();
        }

        private void AddTrainingItemMethod()
        {
            App.Database.SaveTrainingItem(new Training()
            {
                Description = TrainingItem.Description,
                Title = TrainingItem.Title
            });

            int newId = App.Database.GetLastInsertId();
            foreach (var trainingItemExercise in ExerciseItems)
            {
                App.Database.SaveTrainingExerciseItem(new TrainingExerciseComm(){ExerciseId = trainingItemExercise.Id,TrainingId = newId });
            }

            DependencyService.Get<IMessage>().ShortAlert("Добавлено");
            App.Current.MainPage = new MainPage();
        }

        #region Properties

        public ObservableCollection<Exercise> ExerciseItems { get; set; }

        public string Title
        {
            get { return TrainingItem.Title; }
            set
            {
                TrainingItem.Title = value;
                OnPropertyChanged();
            } 
        }

        public string Description
        {
            get { return TrainingItem.Description; }
            set
            {
                TrainingItem.Description = value;
                OnPropertyChanged();
            }
        }
  
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
