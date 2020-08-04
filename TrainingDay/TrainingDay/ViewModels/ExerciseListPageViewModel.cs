using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Crashes;
using TrainingDay.Model;
using TrainingDay.Old;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

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
            Items = new ObservableCollection<TrainingExerciseViewModel>();
            BaseItems = new ObservableCollection<TrainingExerciseViewModel>();
            PropertyChanged += ExerciseListPageViewModel_PropertyChanged;
            UpdateItems();
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
            var resultItems = new List<TrainingExerciseViewModel>();
            resultItems.AddRange(BaseItems.Where(a => a.IsSelected).Select(x => x).ToList());
            foreach (var selectedIndex in selectedIndexes)
            {
                if (resultItems.All(item => item.ExerciseId != selectedIndex))
                {
                    resultItems.Add(new TrainingExerciseViewModel(App.Database.GetExerciseItem(selectedIndex),new TrainingExerciseComm()));
                }
            }

            return resultItems;
        }

        public ICommand DeleteExerciseCommand { protected set; get; }
        private void DeleteExerciseFromBase()
        {
            var deleteItems = GetSelectedItems();
            var names = string.Join("\n", deleteItems.Select(item => item.ExerciseItemName));
            var idItemsToRemove = new List<int>();
            QuestionPopup popup = new QuestionPopup(Resource.DeleteExercises, Resource.AreYouSerious + "\n" + names);
            popup.PopupClosed += (o, closedArgs) =>
            {
                if (closedArgs.Button == Resource.OkString)
                {
                    bool isDeleted = false;
                    foreach (var exerciseSelectViewModel in deleteItems)
                    {
                        if (exerciseSelectViewModel.IsSelected)
                        {
                            idItemsToRemove.Add(exerciseSelectViewModel.ExerciseId);
                            App.Database.DeleteExerciseItem(exerciseSelectViewModel.ExerciseId);
                            isDeleted = true;
                        }
                    }

                    if (isDeleted)
                    {
                        DependencyService.Get<IMessage>().ShortAlert(Resource.DeletedString);
                        foreach (var idItem in idItemsToRemove)
                        {
                            var itemToFind = Items.First(item => item.ExerciseId == idItem);
                            Items.Remove(itemToFind);
                        }
                    }
                }
            };
            popup.Show(Resource.OkString, Resource.CancelString);
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

        List<int> selectedIndexes = new List<int>();
        private void FillSelectedIndexes()
        {
            foreach (var trainingExerciseViewModel in Items)
            {
                if (trainingExerciseViewModel.IsSelected && !selectedIndexes.Contains(trainingExerciseViewModel.ExerciseId))
                {
                    selectedIndexes.Add(trainingExerciseViewModel.ExerciseId);
                }
                if (!trainingExerciseViewModel.IsSelected && selectedIndexes.Contains(trainingExerciseViewModel.ExerciseId))
                {
                    selectedIndexes.Remove(trainingExerciseViewModel.ExerciseId);
                }
            }
        }

        public string NameFilter { get; set; }
        public List<MusclesEnum> MuscleFilter { get; set; } = new List<MusclesEnum>();
        public ObservableCollection<TrainingExerciseViewModel> LoadItems(string name, List<MusclesEnum> muscle)
        {
            var newItems = new ObservableCollection<TrainingExerciseViewModel>();
            var baseItems = App.Database.GetExerciseItems();
            try
            {
                //var selItems = GetSelectedItems();
                FillSelectedIndexes();

                foreach (var exercise in baseItems)
                {
                    var newItem = new TrainingExerciseViewModel(exercise, new TrainingExerciseComm());
                    //if (selItems.Any(ex => ex.ExerciseId == exercise.Id)) // если было до этого выбрано
                    //{
                    //    newItem.IsSelected = true;
                    //}
                    newItem.IsSelected = selectedIndexes.Contains(newItem.ExerciseId);
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                Crashes.TrackError(e);
                try
                {
                    baseItems.ForEach(exercise => newItems.Add(new TrainingExerciseViewModel(exercise, new TrainingExerciseComm())));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    Crashes.TrackError(e);
                }
                return newItems;
            }
        }
    }
}