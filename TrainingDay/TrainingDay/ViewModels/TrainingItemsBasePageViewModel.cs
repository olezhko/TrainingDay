using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using TrainingDay.Model;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.View;
using TrainingDay.Views;
using TrainingDay.Views.Controls;
using TrainingDay.Views.ModalPages;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    public class TrainingItemsBasePageViewModel:BaseViewModel
    {
        public ObservableCollection<Grouping<string, TrainingViewModel>> ItemsGrouped { get; set; }
        public States State { get; set; } = States.NoData;
        public INavigation Navigation { get; set; }
        public TrainingItemsBasePageViewModel()
        {
            //Items = new ObservableCollection<TrainingViewModel>();
            ItemsGrouped = new ObservableCollection<Grouping<string, TrainingViewModel>>();
            AddNewTrainingCommand = new Command(AddNewTraining);
            DeleteSelectedTrainingsCommand = new Command<ViewCell>(DeleteSelectedTraining);
            ItemSelectedCommand = new Command<SelectedItemChangedEventArgs>(TrainingSelected);
        }

        public void LoadItems()
        {
            State = States.Loading;
            OnPropertyChanged(nameof(State));

            //Items.Clear();
            ItemsGrouped.Clear();
            var trainingsItems = App.Database.GetTrainingItems(); // get list of trainings
            var trainingsGroups = App.Database.GetTrainingsGroups();
            Debug.WriteLine($"TrainingItemsBasePageViewModel Groups {trainingsGroups.Count()}");

            if (trainingsItems != null && trainingsItems.Any())
            {
                foreach (var training in trainingsItems)
                {
                    FillGroupedTraining(training, trainingsGroups);
                }
            }

            foreach (var group in ItemsGrouped)
            {
                group.Expanded = group.Expanded;
            }

            OnPropertyChanged(nameof(ItemsGrouped));

            State = ItemsGrouped.Any() ? States.Normal : States.NoData;
            OnPropertyChanged(nameof(State));
        }

        public ICommand AddNewTrainingCommand { get; set; }
        private void AddNewTraining()
        {
            PreparedTrainingsPageViewModel vm = new PreparedTrainingsPageViewModel(){Navigation = Navigation};
            Navigation.PushAsync(new PreparedTrainingsPage(){BindingContext = vm});
        }

        public ICommand DeleteSelectedTrainingsCommand { get; set; }
        private void DeleteSelectedTraining(ViewCell viewCell)
        {
            try
            {
                viewCell.ContextActions.Clear();
                var item = (TrainingViewModel)viewCell.BindingContext;


                QuestionPopup popup = new QuestionPopup(Resource.DeleteTraining, Resource.AreYouSerious + "\n" + item.Title);
                popup.PopupClosed += (o, closedArgs) =>
                {
                    if (closedArgs.Button == Resource.OkString)
                    {
                        App.Database.DeleteTrainingItem(item.Id);
                        item.DeleteTrainingsItemsFromBase();

                        DeleteTrainingAlarms(item);
                        //Items.Remove(item);
                        var group = ItemsGrouped.FirstOrDefault(gr => gr.Contains(item));
                        group.Remove(item);
                        if (group.Count == 0)
                        {
                            ItemsGrouped.Remove(group);
                            var groups = App.Database.GetTrainingsGroups();
                            var gr = groups.FirstOrDefault(a => a.Name == group.Key);
                            if (gr != null) App.Database.DeleteTrainingGroup(gr.Id);
                        }

                        DependencyService.Get<IMessage>().ShortAlert(Resource.DeletedString);
                        State = ItemsGrouped.Any() ? States.Normal : States.NoData;
                        OnPropertyChanged(nameof(State));
                    }
                };
                popup.Show(Resource.OkString, Resource.CancelString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
        
        public ICommand ItemSelectedCommand { get; set; }
        private async void TrainingSelected(SelectedItemChangedEventArgs parameter)
        {
            TrainingViewModel trVm = parameter.SelectedItem as TrainingViewModel;
            PrepareTrainingViewModel(trVm);

            TrainingExercisesPageViewModel vm = new TrainingExercisesPageViewModel()
            {
                Navigation = Navigation
            };
            vm.Load(trVm);
            await Navigation.PushAsync(new TrainingExercisesPage() {BindingContext = vm});
        }

        public static void PrepareTrainingViewModel(TrainingViewModel vm)
        {
            var trainingExerciseItems = App.Database.GetTrainingExerciseItems(); 
            var exerciseItems = App.Database.GetExerciseItems();
            var trainingExercises = trainingExerciseItems.Where(ex => ex.TrainingId == vm.Id);
            var unOrderedItems = trainingExercises.Where(a => a.OrderNumber < 0);

            trainingExercises = trainingExercises.OrderBy(a => a.OrderNumber).Where(a=>a.OrderNumber >= 0).ToList();
            int index = 0;
            foreach (var trainingExercise in trainingExercises)
            {
                var exercise = exerciseItems.First(ex => ex.Id == trainingExercise.ExerciseId);
                var trEx = new TrainingExerciseViewModel(exercise, trainingExercise)
                {
                    TrainingExerciseId = trainingExercise.Id,
                };
                index++;

                vm.AddExercise(trEx);
            }

            foreach (var trainingExercise in unOrderedItems)
            {
                if (trainingExercise.OrderNumber == -1)
                {
                    trainingExercise.OrderNumber = index;
                    App.Database.SaveTrainingExerciseItem(trainingExercise);
                }
                var exercise = exerciseItems.First(ex => ex.Id == trainingExercise.ExerciseId);
                var trEx = new TrainingExerciseViewModel(exercise, trainingExercise)
                {
                    TrainingExerciseId = trainingExercise.Id,
                };
                index++;
                vm.AddExercise(trEx);
            }
        }


        public ICommand AddToGroupCommand => new Command<ViewCell>(AddToGroup);
        private void AddToGroup(ViewCell viewCell)
        {
            viewCell.ContextActions.Clear();
            var item = (TrainingViewModel)viewCell.BindingContext;

            Navigation.PushAsync(new TrainingsSetGroupPage()
            {
                trainingMoveToGroup = item
            });
        }

        private void FillGroupedTraining(Training training, IEnumerable<TrainingUnion> unions)
        {
            var key = unions.FirstOrDefault(union =>
                (new TrainingUnionViewModel(union)).TrainingIDs.Contains(training.Id));

            if (key != null) // union exist
            {
                var group = ItemsGrouped.FirstOrDefault(item => item.Id == key.Id);
                if (group != null)
                {
                    var item = new TrainingViewModel(training);
                    item.GroupName = group.First().GroupName;
                    group.Add(item);
                }
                else
                {
                    ItemsGrouped.Add(new Grouping<string, TrainingViewModel>(key.Name, new List<TrainingViewModel>())
                    {
                        Expanded = key.IsExpanded,
                        Id = key.Id
                    });
                    var item = new TrainingViewModel(training);
                    item.GroupName = new TrainingUnionViewModel(key);
                    ItemsGrouped.Last().Add(item);
                }
            }
            else
            {
                var item = new TrainingViewModel(training);
                var group = ItemsGrouped.FirstOrDefault(gp => gp.Key == Resource.GroupingDefaultName);
                if (group == null)
                {
                    var gr = new Grouping<string, TrainingViewModel>(Resource.GroupingDefaultName, new List<TrainingViewModel>())
                    {
                        Expanded = Settings.IsExpandedMainGroup
                    };
                    gr.Add(item);
                    ItemsGrouped.Insert(0, gr);
                }
                else
                {
                    group.Add(item);
                }
            }
        }

        public ICommand DeleteFromGroupCommand => new Command<ViewCell>(DeleteFromGroup);
        private void DeleteFromGroup(ViewCell viewCell)
        {
            viewCell.ContextActions.Clear();
            var item = (TrainingViewModel)viewCell.BindingContext;

            if (item.GroupName == null || item.GroupName.Id == 0)
            {
                DependencyService.Get<IMessage>().ShowMessage(Resource.GroupingTrainingNotInGroup, Resource.Denied);
                return;
            }
            else
            {
                var union = App.Database.GetTrainingGroup(item.GroupName.Id);
                var viewModel = new TrainingUnionViewModel(union);
                viewModel.TrainingIDs.Remove(item.Id);

                if (viewModel.TrainingIDs.Count == 0)
                {
                    App.Database.DeleteTrainingGroup(viewModel.Model.Id);
                }
                else
                {
                    App.Database.SaveTrainingGroup(viewModel.Model);
                }
                item.GroupName = null;
                LoadItems();
                DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
            }
        }

        public Command<object> ToggleExpandGroupCommand => new Command<object>(ToggleExpandGroup);
        private void ToggleExpandGroup(object item)
        {
            var group = item as Grouping<string, TrainingViewModel>;
            group.Expanded = !group.Expanded;
            var groups = App.Database.GetTrainingsGroups();
            if (group.Key == Resource.GroupingDefaultName)
            {
                Settings.IsExpandedMainGroup = group.Expanded;
                return;
            }
            var gr = groups.First(a => a.Name == group.Key);
            gr.IsExpanded = group.Expanded;
            App.Database.SaveTrainingGroup(gr);
        }
    }

    public class Grouping<K, T> : ObservableCollection<T>
    {
        public int Id { get; set; }
        public K Key { get; private set; }
        private bool isVisible = true;
        public bool Expanded
        {
            get => isVisible;
            set
            {
                isVisible = value;
                if (value)
                {
                    foreach (var item in itemsMain)
                    {
                        this.Add(item);
                    }
                    itemsMain.Clear();
                }
                else
                {
                    foreach (var item in Items)
                    {
                        itemsMain.Add(item);
                    }
                    this.Clear();
                }

                OnPropertyChanged(new PropertyChangedEventArgs("Expanded"));
            }
        }

        private ObservableCollection<T> itemsMain = new ObservableCollection<T>();
        public Grouping(K key, IEnumerable<T> items)
        {
            Key = key;
            foreach (var item in items)
                this.Items.Add(item);
        }
    }

    public class TrainingViewModel:BaseViewModel
    {
        public int Id { get; set; }

        private string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            } }


        private TrainingUnionViewModel groupName = null;
        public TrainingUnionViewModel GroupName
        {
            get => groupName;
            set
            {
                groupName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SuperSetViewModel> ExercisesBySuperSet => PrepareSuperSets();

        public ObservableCollection<TrainingExerciseViewModel> Exercises { get; set; }

        public TrainingViewModel()
        {
            Exercises = new ObservableCollection<TrainingExerciseViewModel>();
        }

        private ObservableCollection<SuperSetViewModel> PrepareSuperSets()
        {
            ObservableCollection<SuperSetViewModel> res = new ObservableCollection<SuperSetViewModel>();
            foreach (var exercise in Exercises)
            {
                if (exercise.SuperSetId != 0)
                {
                    var first = res.FirstOrDefault(a => a.Id == exercise.SuperSetId);
                    if (first != null)
                    {
                        first.Add(exercise);
                    }
                    else
                    {
                        res.Add(new SuperSetViewModel() { TrainingId = exercise.TrainingId, Id = exercise.SuperSetId });
                        res.Last().Add(exercise);
                    }
                }
                else
                {
                    res.Add(new SuperSetViewModel() { TrainingId = exercise.TrainingId });
                    res.Last().Add(exercise);
                }
            }

            return res;
        }

        public TrainingViewModel(Training tr):this()
        {
            Title = tr.Title;
            Id = tr.Id;
        }

        public void DeleteTrainingsItemsFromBase()
        {
            App.Database.DeleteTrainingExerciseItemByTrainingId(Id);
            App.Database.DeleteSuperSetsByTrainingId(Id);
        }

        public void SaveToFile(string filename)
        {
            TrainingSerialize serialize = new TrainingSerialize();
            serialize.Title = Title;
            serialize.Id = Id;
            foreach (var trainingExerciseViewModel in Exercises)
            {
                serialize.Items.Add(new TrainingExerciseSerialize()
                {
                    TrainingExerciseId =  trainingExerciseViewModel.TrainingExerciseId,
                    SuperSetId = trainingExerciseViewModel.SuperSetId,
                    OrderNumber = trainingExerciseViewModel.OrderNumber,
                    TrainingId = Id,
                    Muscles = MusclesConverter.ConvertFromListToString(trainingExerciseViewModel.Muscles.ToList()),
                    ExerciseId = trainingExerciseViewModel.ExerciseId,
                    ExerciseImageUrl = trainingExerciseViewModel.ExerciseImageUrl,
                    ShortDescription = trainingExerciseViewModel.ShortDescription,
                    ExerciseItemName = trainingExerciseViewModel.ExerciseItemName,
                    IsNotFinished = trainingExerciseViewModel.IsNotFinished,
                    SuperSetNum = trainingExerciseViewModel.SuperSetNum,

                    TagsValue = ExerciseTagExtension.ConvertListToInt(trainingExerciseViewModel.Tags),
                    WeightAndRepsString = ExerciseTagExtension.ConvertJson(trainingExerciseViewModel.Tags, trainingExerciseViewModel),
                    CodeNum = trainingExerciseViewModel.GetExercise().CodeNum
                });
            }

            serialize.SaveToFile(filename);
        }

        public void AddExercise(TrainingExerciseViewModel exercise)
        {
            Exercises.Add(exercise);
            exercise.SuperSetNum = GetSuperSetNum(this, exercise);
            //if (exercise.SuperSetId != 0)
            //{
            //    var first = ExercisesBySuperSet.FirstOrDefault(a => a.Id == exercise.SuperSetId);
            //    if (first!=null)
            //    {
            //        first.Add(exercise);
            //    }
            //    else
            //    {
            //        ExercisesBySuperSet.Add(new SuperSetViewModel() { TrainingId = exercise.TrainingId,Id = exercise.SuperSetId});
            //        ExercisesBySuperSet.Last().Add(exercise);
            //    }
            //}
            //else
            //{
            //    ExercisesBySuperSet.Add(new SuperSetViewModel(){TrainingId = exercise.TrainingId});
            //    ExercisesBySuperSet.Last().Add(exercise);
            //}
        }

        public bool DeleteExercise(int id)
        {
            bool res = false;
            res = Exercises.Remove(Exercises.FirstOrDefault(item=>item.TrainingExerciseId == id));
            return res;
        }
        
        public bool DeleteExercise(TrainingExerciseViewModel exercise)
        {
            bool res = false;
            res = Exercises.Remove(exercise);
            //foreach (var superSet in ExercisesBySuperSet)
            //{
            //    if (exercise.SuperSetId != 0)
            //    {
            //        if (superSet.Id == exercise.SuperSetId)
            //        {
            //            if (superSet.Count == 1)
            //            {
            //                ExercisesBySuperSet.Remove(superSet);
            //            }
            //            else
            //            {
            //                int index = superSet.IndexOf(a => a.TrainingExerciseId == exercise.TrainingExerciseId);
            //                superSet.RemoveAt(index);
            //            }
            //            ReloadItems();
            //            res = true;
            //            break;
            //        }
            //    }
            //    else
            //    {
            //        if (superSet.First().TrainingExerciseId == exercise.TrainingExerciseId)
            //        {
            //            ExercisesBySuperSet.Remove(superSet);
            //            ReloadItems();
            //            res = true;
            //            break;
            //        }
            //    }
            //}

            return res;
        }

        public static int GetSuperSetNum(TrainingViewModel training, TrainingExerciseViewModel item)
        {
            int number = 1;
            List<int> superSetIds = new List<int>();
            foreach (var exercise in training.Exercises)
            {
                if (exercise.SuperSetId != 0)
                {
                    if (exercise.SuperSetId == item.SuperSetId)
                    {
                        return number;
                    }

                    if (!superSetIds.Contains(exercise.SuperSetId))
                    {
                        superSetIds.Add(exercise.SuperSetId);
                        number++;
                    }
                }
            }

            return number;
        }
    }

    public class SuperSetViewModel:ObservableCollection<TrainingExerciseViewModel>
    {
        public int Id { get; set; }
        public int TrainingId { get; set; }

        public bool IsSuperSet => SuperSetItems.Count > 1;
        public ObservableCollection<TrainingExerciseViewModel> SuperSetItems
        {
            get { return new ObservableCollection<TrainingExerciseViewModel>(this.Items); }
        }
        public SuperSet Model =>
            new SuperSet()
            {
                Count = Items.Count,
                Id = Id,
                TrainingId = TrainingId
            };
    }
}
