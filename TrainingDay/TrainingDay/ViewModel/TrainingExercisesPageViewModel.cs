using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TrainingDay.Code;
using TrainingDay.Helpers;
using TrainingDay.Model;
using TrainingDay.View;
using Xamarin.Forms;

namespace TrainingDay.ViewModel
{
    class TrainingExercisesPageViewModel:BaseViewModel
    {
        public TrainingExercisesPageViewModel()
        {
            CancelChangesCommand = new Command(CancelChanges);
            SaveChangesCommand = new Command(SaveChanges);
            DeleteSelectedExercisesCommand = new Command(DeleteSelectedExercises);
            ExerciseSelectedCommand = new Command(ExerciseSelected);
        }

        private void ExerciseSelected()
        {
            ExerciseItemPage exercise = new ExerciseItemPage();
            exercise.LoadExercise(SelectedItem.Id);
            Navigation.PushAsync(exercise);
        }

        public void LoadItems()
        {
            if (Items == null)
            {
                Items = new ObservableCollection<ExerciseSelectViewModel>();
            }
            Items.Clear();
            var items = App.Database.GetTrainingExerciseItemByTraningId(TrainingID);
            foreach (var exercise in items)
            {
                Items.Add(new ExerciseSelectViewModel()
                {
                    Id = exercise.Id,
                    ExerciseItemName = exercise.ExerciseItemName,
                    ExerciseCountOfApproches = exercise.CountOfApproches,
                    ExerciseCountOfTimes = exercise.CountOfTimes,
                    ExerciseShortDescription = exercise.ShortDescription,
                });
            }
            OnPropertyChanged(nameof(Items));
        }

        private void DeleteSelectedExercises()
        {
            bool IsDeleted = false;
            foreach (var item in Items.Where(x => x.IsSelected).ToList())
            {
                IsDeleted = true;
                Items.Remove(item);
            }
            if (IsDeleted)
                DependencyService.Get<IMessage>().ShortAlert(Resource.DeletedString);
        }

        private void SaveChanges()
        {
            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
            TrainingExercisesChanges?.Invoke(this,null);
            Navigation.PopAsync();
        }

        private void CancelChanges()
        {
            Navigation.PopAsync();
        }



        public int TrainingID { get; set; }
        public INavigation Navigation;
        public ICommand DeleteSelectedExercisesCommand { get; set; }
        public ICommand SaveChangesCommand { get; set; }
        public ICommand CancelChangesCommand { get; set; }
        public ICommand ExerciseSelectedCommand { get; set; }
        public event EventHandler TrainingExercisesChanges;

        public ObservableCollection<ExerciseSelectViewModel> Items { get; set; }

        public ExerciseSelectViewModel SelectedItem { get; set; }
    }
}
