using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TrainingDay.Old;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.Views.Controls;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    public class ExerciseListPageViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public States State { get; set; } = States.NoData;
        public ObservableCollection<TrainingExerciseViewModel> Items { get; set; } // элементы, отображенные на экране в списке
        ObservableCollection<TrainingExerciseViewModel> BaseItems { get; set; } // все упражнения из базы

        public ExerciseListPageViewModel()
        {
            ChoseExercisesCommand = new Command(ChoseExercises);
            DeleteExerciseCommand = new Command(DeleteExerciseFromBase);
            ViewFilterWindowCommand = new Command(ViewFilterWindow);
            SearchByNameCommand = new Command(UpdateItems);
            Items = new ObservableCollection<TrainingExerciseViewModel>();
            BaseItems = new ObservableCollection<TrainingExerciseViewModel>();
            PropertyChanged += ExerciseListPageViewModel_PropertyChanged;
        }

        private void ExerciseListPageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Items))
            {
                State = Items.Any() ? States.Normal : States.NoData;
                OnPropertyChanged(nameof(State));
            }
        }

        /// <summary>
        /// Если элементы были выбраны путем поиска, а потом сбросили поиск, то оставить их выбранными
        /// </summary>
        /// <returns></returns>
        public List<TrainingExerciseViewModel> GetSelectedItems()
        {
            var items = BaseItems.Where(a => a.IsSelected).Select(x => x).ToList();
            return items;
        }

        public ICommand DeleteExerciseCommand { protected set; get; }
        private void DeleteExerciseFromBase()
        {
            QuestionPopup popup = new QuestionPopup(Resource.DeleteExercises, Resource.AreYouSerious);
            popup.PopupClosed += (o, closedArgs) =>
            {
                if (closedArgs.Button == "OK")
                {
                    bool isDeleted = false;
                    foreach (var exerciseSelectViewModel in Items)
                    {
                        if (exerciseSelectViewModel.IsSelected)
                        {
                            App.Database.DeleteExerciseItem(exerciseSelectViewModel.ExerciseId);
                            isDeleted = true;
                        }
                    }

                    if (isDeleted)
                    {
                        DependencyService.Get<IMessage>().ShortAlert(Resource.DeletedString);
                        UpdateItems();
                    }
                }
            };
            popup.Show();
        }

        public ICommand ViewFilterWindowCommand { protected set; get; }
        private void ViewFilterWindow()
        {
            Navigation.PushAsync(new FilterPage(this));
        }

        public event EventHandler ExercisesChousen;
        public ICommand ChoseExercisesCommand { protected set; get; }
        private void ChoseExercises()
        {
            Navigation.PopAsync();
            ExercisesChousen?.Invoke(this, null);
        }

        public void UpdateItems()
        {
            State = States.Loading;
            OnPropertyChanged(nameof(State));

            LoadItemsAsync();

            State = Items.Any() ? States.Normal : States.NoData;
            OnPropertyChanged(nameof(State));
        }

        private async void LoadItemsAsync()
        {
            var res = await Task.Run(() => LoadItems(NameFilter, MuscleFilter));
            Items = res;
            BaseItems = new ObservableCollection<TrainingExerciseViewModel>(res);
            OnPropertyChanged(nameof(Items));
        }

        public string NameFilter { get; set; }
        public List<MusclesEnum> MuscleFilter { get; set; } = new List<MusclesEnum>();
        public ICommand SearchByNameCommand { protected set; get; }
        public ObservableCollection<TrainingExerciseViewModel> LoadItems(string name, List<MusclesEnum> muscle)
        {
            var newItems = new ObservableCollection<TrainingExerciseViewModel>();
            var selItems = GetSelectedItems();
            var baseItems = App.Database.GetExerciseItems();
            foreach (var exercise in baseItems)
            {
                var newItem = new TrainingExerciseViewModel(exercise, new TrainingExerciseComm() { Weight = 5, CountOfApproches = 5, CountOfTimes = 15 });
                if (selItems.Any(ex => ex.ExerciseId == exercise.Id)) // если было до этого выбрано
                {
                    newItem.IsSelected = true;
                }

                bool byname = false, byFilter = false;

                if (String.IsNullOrEmpty(name) && muscle.Count == 0)
                {
                    newItems.Add(newItem);
                }
                else
                {
                    if (!String.IsNullOrEmpty(name))
                    {
                        if (newItem.ExerciseItemName.Contains(name, StringComparison.OrdinalIgnoreCase))
                        {
                            byname = true;
                        }
                    }

                    if (muscle.Count != 0)
                    {
                        var newData = newItem.Muscles.Select(i => (MusclesEnum)i.Id).Intersect(muscle);
                        if (newData.Count() != 0)
                        {
                            byFilter = true;
                        }
                    }

                    if (!String.IsNullOrEmpty(name) && muscle.Count == 0 && byname)
                    {
                        newItems.Add(newItem);
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(name) && muscle.Count != 0 && byFilter)
                        {
                            newItems.Add(newItem);
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(name) && muscle.Count != 0 && byFilter && byname)
                            {
                                newItems.Add(newItem);
                            }
                        }
                    }
                }
            }
            return newItems;
        }
    }
}