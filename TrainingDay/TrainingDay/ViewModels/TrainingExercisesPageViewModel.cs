using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using TrainingDay.Model;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.View;
using TrainingDay.Views;
using TrainingDay.Views.Controls;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TrainingDay.ViewModels
{
    class TrainingExercisesPageViewModel:BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public TrainingViewModel Training { get; set; }

        public TrainingExercisesPageViewModel()
        {
            SaveChangesCommand = new Command(SaveChanges);
            Training = new TrainingViewModel();
        }

        public void Load(TrainingViewModel trVm)
        {
            if (trVm == null)
            {
                return;
            }

            Training.Title = trVm.Title;
            OnPropertyChanged(nameof(Training.Title));

            Training.Id = trVm.Id;
            foreach (var item in trVm.Exercises)
            {
                Training.AddExercise(item);
            }
            OnPropertyChanged(nameof(Training.Exercises));
        }


        public ICommand AddExercisesCommand => new Command(AddExercises);
        private async void AddExercises()
        {
            var vm = new ExerciseListPageViewModel() { Navigation = Navigation };
            vm.ExercisesChousen += (sender, args) =>
            {
                AddSelectedExercises(vm);
            };
            await Navigation.PushAsync(new ExerciseListPage(vm));
        }
      
        private void AddSelectedExercises(ExerciseListPageViewModel obj)
        {
            if (obj != null)
            {
                var selectedItems = obj.GetSelectedItems();
                selectedItems.ForEach(a=>a.IsSelected = false);
                foreach (var exerciseItem in selectedItems)
                {
                    Training.AddExercise(exerciseItem);
                }
            }
        }

        public ICommand MakeNotifyCommand => new Command(MakeNotify);
        private async void MakeNotify()
        {
            MakeTrainingAlarmPageViewModel vm = new MakeTrainingAlarmPageViewModel() { Navigation = Navigation};
            vm.Alarm.TrainingId = Training.Id;
            MakeTrainingAlarmPage page = new MakeTrainingAlarmPage() { BindingContext = vm };
            await Navigation.PushAsync(page, true);
        }

        public ICommand ItemTappedCommand => new Command<Syncfusion.ListView.XForms.ItemTappedEventArgs>(TrainingExerciseTapped);
        private async void TrainingExerciseTapped(Syncfusion.ListView.XForms.ItemTappedEventArgs e)
        {
            TrainingExerciseViewModel viewModel = e.ItemData as TrainingExerciseViewModel;

            if (CurrentAction != ExerciseCheckBoxAction.None) // when we in action, tapped equals changing selected
            {
                if (CurrentAction == ExerciseCheckBoxAction.SuperSet)
                {
                    if ( viewModel.SuperSetId == 0 ) //!IsExerciseInSuperSet
                    {
                        viewModel.IsSelected = !viewModel.IsSelected;
                    }
                }
                else
                {
                    viewModel.IsSelected = !viewModel.IsSelected;
                }

                return;
            }

            TrainingExerciseItemPage page = new TrainingExerciseItemPage();
            viewModel.IsNotFinished = false;// --> for Time start button, to hide button
            page.LoadExercise(viewModel);
            await Navigation.PushAsync(page);
        }



        public ICommand DeleteExerciseCommand => new Command<TrainingExerciseViewModel>(DeleteExercise);
        private void DeleteExercise(TrainingExerciseViewModel sender)// "cross button pressed"
        {
            if (CurrentAction == ExerciseCheckBoxAction.SuperSet)
            {
                if (sender.SuperSetId != 0)
                {
                    QuestionPopup popup = new QuestionPopup("", Resource.DeleteExerciseFromSuperSetQuestion);
                    popup.PopupClosed += (o, closedArgs) =>
                    {
                        if (closedArgs.Button == Resource.OkString)
                        {
                            var id = sender.SuperSetId;
                            sender.SuperSetId = 0;
                            CheckSuperSetExist(id);
                        }
                    };
                    popup.Show(Resource.OkString,Resource.CancelString);
                }
            }
            else
            {
                QuestionPopup popup = new QuestionPopup(Resource.DeleteExercises, Resource.AreYouSerious + "\n" + sender.ExerciseItemName);
                popup.PopupClosed += (o, closedArgs) =>
                {
                    if (closedArgs.Button == Resource.OkString)
                    {
                        Training.DeleteExercise(sender);

                        if(sender.SuperSetId != 0)
                            CheckSuperSetExist(sender.SuperSetId);
                    }
                };
                popup.Show(Resource.OkString, Resource.CancelString);
            }
        }

        private void CheckSuperSetExist(int supersetId)
        {
            var list = new List<TrainingExerciseViewModel>();
            foreach (var item in Training.Exercises)
            {
                if (item.SuperSetId == supersetId)
                {
                    list.Add(item);
                }
            }

            if (list.Count == 1)
            {
                var item = list.First();
                item.SuperSetId = 0;
                App.Database.SaveTrainingExerciseItem(item.GetTrainingExerciseComm());
                App.Database.DeleteSuperSetItem(supersetId);
            }
        }

        public ICommand MakeTrainingCommand => new Command(MakeTraining);
        private async void MakeTraining()
        {
            DependencyService.Get<IMessage>().ShowMessage(Resource.AdviceBeforeTrainingMessage, Resource.AdviceString);
            foreach (var item in Training.Exercises)
            {
                item.IsNotFinished = true;
            }
            await Navigation.PushAsync(new TrainingImplementPage() { TrainingItem = Training, Title = Training.Title });

            //Application.Current.MainPage = new NavigationPage(new TrainingImplementPage() { TrainingItem = Training, Title = Training.Title });
        }


        // save name
        // save super-sets
        // save exercises
        public ICommand SaveChangesCommand { get; set; }
        private void SaveChanges()
        {
            // save training name
            var id = App.Database.SaveTrainingItem(new Training() { Id = Training.Id, Title = Training.Title });

            ClearTrExUnused(); // clear all trainingExercises communication with selected training id

            // save every exercise
            int order = 0;
            foreach (var item in Training.Exercises)
            {
                if (!item.IsSelected && CurrentAction== ExerciseCheckBoxAction.Select)
                {
                    continue;//ignore
                }
                var exId = App.Database.SaveExerciseItem(item.GetExercise());
                // save order numbers
                order++;
                App.Database.SaveTrainingExerciseItem(new TrainingExerciseComm()
                {
                    ExerciseId = exId,
                    TrainingId = id,
                    OrderNumber = order,
                    Id = item.TrainingExerciseId,
                    SuperSetId = item.SuperSetId,
                    WeightAndRepsString = ExerciseTagExtension.ConvertJson(item.Tags, item)
                });
            }

            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
            //Application.Current.MainPage = new NavigationPage(new MainPage());
            //Navigation.PopAsync();
        }

        private void ClearTrExUnused()
        {
            var trExercises = Training.Exercises;
            var trainingExerciseItems = App.Database.GetTrainingExerciseItems(); // get all tr-exercises comm

            // delete all exercises, that user delete by "cross" button
            foreach (var trainingExerciseItem in trainingExerciseItems)
            {
                if (trainingExerciseItem.TrainingId == Training.Id && trExercises.All(model => trainingExerciseItem.Id != model.TrainingExerciseId))
                {
                    App.Database.DeleteTrainingExerciseItem(trainingExerciseItem.Id);
                }
            }

            var superSets = App.Database.GetSuperSetItems();
            foreach (var superSet in superSets)
            {
                if (superSet.TrainingId == Training.Id)
                {
                    bool res = false;
                    foreach (var trainingExerciseViewModel in trExercises)
                    {
                        if (trainingExerciseViewModel.SuperSetId == superSet.Id) //if training have exercises with this superset id
                        {
                            res = true;
                        }
                    }

                    if (!res)
                    {
                        App.Database.DeleteSuperSetItem(superSet.Id);
                    }
                }
            }
        }

        private void SaveNewTraining()
        {
            // save training name
            var id = App.Database.SaveTrainingItem(new Training() { Id = Training.Id, Title = Training.Title });

            // save every exercise
            int order = 0;
            foreach (var item in Training.Exercises)
            {
                if (!item.IsSelected && CurrentAction == ExerciseCheckBoxAction.Select)
                {
                    continue;//ignore
                }
                var exId = App.Database.SaveExerciseItem(item.GetExercise());
                // save order numbers
                order++;
                App.Database.SaveTrainingExerciseItem(new TrainingExerciseComm()
                {
                    ExerciseId = exId,
                    TrainingId = id,
                    OrderNumber = order,
                    Id = item.TrainingExerciseId,
                    SuperSetId = item.SuperSetId,
                    WeightAndRepsString = ExerciseTagExtension.ConvertJson(item.Tags, item)
                });
            }

            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
        }

        #region Exercises Actions : SuperSet, Move, Copy
        public bool IsExercisesCheckBoxVisible { get; set; }
        public ExerciseCheckBoxAction CurrentAction { get; set; }
        public ICommand SetSuperSetCommand => new Command(InitSuperSetMode);
        private void InitSuperSetMode()
        {
            CurrentAction = ExerciseCheckBoxAction.SuperSet;
            OnPropertyChanged(nameof(CurrentAction));
            PrepareAction(Resource.SuperSetString);
        }

        public ICommand ExercisesCheckedChangedCommand => new Command<TrainingExerciseViewModel>(CollectCheckedExercises);
        ObservableCollection<TrainingExerciseViewModel> selectedItems = new ObservableCollection<TrainingExerciseViewModel>();
        private void CollectCheckedExercises(TrainingExerciseViewModel item)
        {
            if (IsMoveOrCopyAction && CurrentAction != ExerciseCheckBoxAction.SuperSet)// when TrainingSelected->DeleteExercise, this event raised
            {
                return;
            }
            if (item.IsSelected)
                selectedItems.Add(item.Clone());
            else
                selectedItems.Remove(item.Clone());
        }

        private void CreateSuperSet()
        {
            //Training.CreateSuperSetByItems(selectedItems);
           
            var id = App.Database.SaveSuperSetItem(new SuperSet()
            {
                Count = selectedItems.Count,
                TrainingId = this.Training.Id
            });

            var superSetNum = -1;
            foreach (var trainingExerciseViewModel in selectedItems)
            {
                trainingExerciseViewModel.IsSelected = false;
                trainingExerciseViewModel.SuperSetId = id;
                trainingExerciseViewModel.SuperSetNum = superSetNum == -1?TrainingViewModel.GetSuperSetNum(Training, trainingExerciseViewModel): superSetNum;
                superSetNum = trainingExerciseViewModel.SuperSetNum;
                App.Database.SaveTrainingExerciseItem(trainingExerciseViewModel.GetTrainingExerciseComm());
            }


            foreach (var trainingExerciseViewModel in Training.Exercises)
            {
                if (trainingExerciseViewModel.IsSelected)
                {
                    trainingExerciseViewModel.SuperSetNum = superSetNum;
                    trainingExerciseViewModel.SuperSetId = id;
                    trainingExerciseViewModel.IsSelected = false;
                }
            }
        }

        public string ExerciseActionString { get; set; }
        public ICommand StartMoveExerciseCommand => new Command(StartMoveExercises);
        private void StartMoveExercises()
        {
            CurrentAction = ExerciseCheckBoxAction.Move;
            OnPropertyChanged(nameof(CurrentAction));
            PrepareAction(Resource.MoveString);
        }

        public ICommand StartCopyExerciseCommand => new Command(StartCopyExercise);
        private void StartCopyExercise()
        {
            CurrentAction = ExerciseCheckBoxAction.Copy;
            OnPropertyChanged(nameof(CurrentAction));
            PrepareAction(Resource.CopyString);
        }

        private void PrepareAction(string action)
        {
            //foreach (var superSetViewModel in Training.ExercisesBySuperSet)
            //{
            //    superSetViewModel.IsSelected = false;
            //}

            IsExercisesCheckBoxVisible = true;
            OnPropertyChanged(nameof(IsExercisesCheckBoxVisible));

            ExerciseActionString = action;
            OnPropertyChanged(nameof(ExerciseActionString));
        }

        public ICommand CancelActionCommand => new Command(() => StopAction(false));
        private void StopAction(bool result = false)
        {
            if (result)
            {
                switch (CurrentAction)
                {
                    case ExerciseCheckBoxAction.SuperSet:
                        DependencyService.Get<IMessage>().ShortAlert(Resource.SuperSetCreatedMessage);
                        break;
                    case ExerciseCheckBoxAction.Move:
                        DependencyService.Get<IMessage>().ShortAlert(Resource.MoveExercisesFinishedMessage);
                        break;
                    case ExerciseCheckBoxAction.Copy:
                        DependencyService.Get<IMessage>().ShortAlert(Resource.CopyExercisesFinishedMessage);
                        break;
                    case ExerciseCheckBoxAction.Select:
                        //DependencyService.Get<IMessage>().ShortAlert("Select Items");
                        break;
                }
            }
            else
            {
                Training.Exercises.ForEach(item=>item.IsSelected = false);
                if (CurrentAction == ExerciseCheckBoxAction.Select)
                {
                    Navigation.PopAsync();
                    Navigation.PopAsync();
                    return;
                }
            }

            selectedItems.Clear();
            CurrentAction = ExerciseCheckBoxAction.None;
            OnPropertyChanged(nameof(CurrentAction));

            IsExercisesCheckBoxVisible = false;
            OnPropertyChanged(nameof(IsExercisesCheckBoxVisible));
        }

        public ICommand StartActionCommand => new Command(StartAction);
        public bool IsMoveOrCopyAction { get; set; } // need for show or hide listview with training to copy or move
        private void StartAction()
        {
            if (CurrentAction == ExerciseCheckBoxAction.Select)
            {
                SaveNewTraining();
                StopAction(true);
                Navigation.PopAsync();
                Navigation.PopAsync();
            }
            else
                if (CurrentAction != ExerciseCheckBoxAction.SuperSet)
                {
                    ReFillTrainingToCopyOrMove();
                    IsMoveOrCopyAction = true;
                    OnPropertyChanged(nameof(IsMoveOrCopyAction));
                }
                else
                {
                    if (selectedItems.Count == 1)
                    {
                        CreateSuperSet();
                        StopAction(true);
                    }
                }
        }

        private void ReFillTrainingToCopyOrMove()
        {
            var trainingsItems = App.Database.GetTrainingItems(); // get list of trainings
            TrainingItems.Clear();
            if (trainingsItems != null && trainingsItems.Any())
            {
                foreach (var training in trainingsItems)
                {
                    if (training.Id != Training.Id)
                    {
                        TrainingItems.Add(new TrainingViewModel(training)
                        {
                            Title = training.Title
                        });
                    }
                }
            }
            OnPropertyChanged(nameof(TrainingItems));
        }

        public ICommand TrainingSelectedCommand => new Command<ItemTappedEventArgs>(TrainingSelected);
        private void TrainingSelected(ItemTappedEventArgs parameter)
        {
            TrainingViewModel trVm = parameter.Item as TrainingViewModel;
            int id = trVm.Id;
            if (id!=0)
            {
                while (selectedItems.Count!=0)
                {
                    var model = selectedItems[0];
                    var ex1 = model.GetTrainingExerciseComm();
                    if (CurrentAction == ExerciseCheckBoxAction.Move)
                    {
                        App.Database.DeleteTrainingExerciseItem(ex1.Id);
                        Training.DeleteExercise(model.TrainingExerciseId);
                        if (model.SuperSetId != 0)
                            CheckSuperSetExist(model.SuperSetId);
                    }

                    ex1.SuperSetId = 0;
                    ex1.TrainingId = id;
                    ex1.Id = 0;
                    ex1.OrderNumber = -1;
                    App.Database.SaveTrainingExerciseItem(ex1);
                    selectedItems.Remove(model);
                }
            }

            IsMoveOrCopyAction = false;
            OnPropertyChanged(nameof(IsMoveOrCopyAction));

            StopAction(true);
        }


        public ICommand CreateNewAndPasteCommand => new Command(CreateNewAndPaste);
        private void CreateNewAndPaste()
        {
            var id = App.Database.SaveTrainingItem(new Training()
            {
                Title = Resource.TrainingString
            });

            if (id != 0)
            {
                while (selectedItems.Count != 0)
                {
                    var model = selectedItems[0];
                    var ex1 = model.GetTrainingExerciseComm();
                    if (CurrentAction == ExerciseCheckBoxAction.Move)
                    {
                        App.Database.DeleteTrainingExerciseItem(ex1.Id);
                        Training.DeleteExercise(model.TrainingExerciseId);
                        if (model.SuperSetId != 0)
                            CheckSuperSetExist(model.SuperSetId);
                    }

                    ex1.SuperSetId = 0;
                    ex1.TrainingId = id;
                    ex1.Id = 0;
                    ex1.OrderNumber = -1;
                    App.Database.SaveTrainingExerciseItem(ex1);
                    selectedItems.Remove(model);
                }
            }

            IsMoveOrCopyAction = false;
            OnPropertyChanged(nameof(IsMoveOrCopyAction));

            StopAction(true);
        }

        public ObservableCollection<TrainingViewModel> TrainingItems { get; set; } = new ObservableCollection<TrainingViewModel>();
        #endregion

        #region Share
        public ICommand ShareTrainingCommand => new Command(ShareTraining);
        private async void ShareTraining()
        {
            var fn = $"{Resource.TrainingString}_{Training.Title}.trday";
            var filename = Path.Combine(FileSystem.CacheDirectory, fn);

            Training.SaveToFile(filename);
            await Share.RequestAsync(new ShareFileRequest()
            {
                Title = Resource.ShareTrainingString,
                File = new ShareFile(filename, "application/trday")
            });
            DependencyService.Get<IMessage>().ShortAlert(Resource.SharedString);
        }
        #endregion

        // after prepared
        public void StartSelectExercises()
        {
            CurrentAction = ExerciseCheckBoxAction.Select;
            OnPropertyChanged(nameof(CurrentAction));
            PrepareAction(Resource.ChouseExerciseString);
        }
    }

    public enum ExerciseCheckBoxAction
    {
        None,
        SuperSet,
        Move,
        Copy,
        Select
    }
}
