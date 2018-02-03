using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TrainingDay.Annotations;
using TrainingDay.Code;
using TrainingDay.Helpers;
using TrainingDay.Model;
using Xamarin.Forms;
using XLabs;

namespace TrainingDay.ViewModel
{
    public class ExerciseListPageViewModel : BaseViewModel
    {
        public ObservableCollection<ExerciseSelectViewModel> Items { get; set; }
        public ICommand DeleteExerciseCommand { protected set; get; }
        private AddTrainingViewModel lvm;
        public AddTrainingViewModel ListViewModel
        {
            get { return lvm; }
            set
            {
                if (lvm != value)
                {
                    lvm = value;
                    OnPropertyChanged();
                }
            }
        }

        public INavigation Navigation { get; set; }

        public ExerciseListPageViewModel()
        {
            DeleteExerciseCommand = new Command(DeleteExercise);
        }

        private void DeleteExercise()
        {
            bool isDeleted = false;
            foreach (var exerciseSelectViewModel in Items)
            {
                if (exerciseSelectViewModel.IsSelected)
                {
                    App.Database.DeleteExerciseItem(exerciseSelectViewModel.Id);
                    isDeleted = true;
                }
            }
            if (isDeleted)
            {
                DependencyService.Get<IMessage>().ShortAlert(Resource.DeletedString);
                LoadItems();
            }
        }

        public void LoadItems()
        {
            if (Items == null)
            {
                Items = new ObservableCollection<ExerciseSelectViewModel>();
            }
            Items.Clear();
            foreach (var exercise in App.Database.GetExerciseItems())
            {
                var newItem = new ExerciseSelectViewModel()
                {
                    Id = exercise.Id,
                    IsSelected = ListViewModel.ExerciseItems.Any(a => a.Id == exercise.Id),
                    ExerciseItemName = exercise.ExerciseItemName,
                    ExerciseCountOfApproches = exercise.CountOfApproches,
                    ExerciseCountOfTimes = exercise.CountOfTimes,
                    ExerciseShortDescription = exercise.ShortDescription,
                };
                Items.Add(newItem);

            }
            OnPropertyChanged(nameof(Items));
        }

        public List<Exercise> GetSelectedItems()
        {
            var items = Items.Where(a => a.IsSelected).Select(x => new Exercise()
            {
                ExerciseItemName = x.ExerciseItemName,
                Id = x.Id,
                CountOfApproches = x.ExerciseCountOfApproches,
                CountOfTimes = x.ExerciseCountOfTimes,
                ShortDescription = x.ExerciseShortDescription
            }).ToList();

            return items;
        }
    }

    public class ExerciseSelectViewModel:BaseViewModel
    {
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }
        public int Id { get; set; }
        public string ExerciseItemName { get; set; }
        public int ExerciseCountOfApproches { get; set; }
        public int ExerciseCountOfTimes { get; set; }
        public string ExerciseShortDescription { get; set; }

        public ExerciseSelectViewModel(Exercise baseExercise)
        {
            ExerciseItemName = baseExercise.ExerciseItemName;
            ExerciseCountOfApproches = baseExercise.CountOfApproches;
            ExerciseCountOfTimes = baseExercise.CountOfTimes;
            ExerciseShortDescription = baseExercise.ShortDescription;
        }

        public ExerciseSelectViewModel()
        {
        }
    }
}
