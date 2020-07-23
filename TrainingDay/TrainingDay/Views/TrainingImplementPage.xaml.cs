using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using TrainingDay.Model;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.ViewModels;
using TrainingDay.Views;
using TrainingDay.Views.Controls;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace TrainingDay.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrainingImplementPage : ContentPage
    {
        public ObservableCollection<SuperSetViewModel> Items { get; set; }
        public TrainingViewModel TrainingItem { get; set; }

        public TrainingImplementPage()
        {
            InitializeComponent();
            Items = new ObservableCollection<SuperSetViewModel>();
            if (Device.RuntimePlatform == Device.iOS)
                PromoView.AdUnitId = "iOS Key";
            else if (Device.RuntimePlatform == Device.Android)
                PromoView.AdUnitId = "ca-app-pub-8728883017081055/4843502807";

            BindingContext = this;
            _startTrainingDateTime = DateTime.Now;
            _enabledTimer = true;
            StepProgressBarControl.PropertyChanged += StepProgressBarControl_PropertyChanged;
            Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
            Settings.IsTrainingNotFinished = true;
        }

        private void StepProgressBarControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StepProgressBar.StepSelected))
            {
                FinishButton.IsVisible = Items[StepProgressBarControl.StepSelected].First().IsNotFinished;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Settings.IsDisplayOnImplement)
            {
                DeviceDisplay.KeepScreenOn = true;
            }

            if (StepProgressBarControl.ItemsSource == null)
            {
                Items = TrainingItem.ExercisesBySuperSet;
                StepProgressBarControl.ItemsSource = Items;
                OnPropertyChanged(nameof(Items));

                int index = 0;
                foreach (var item in Items)
                {
                    if (!item.SuperSetItems.First().IsNotFinished)
                    {
                        StepProgressBarControl.DeselectElement();
                    }

                    index++;
                    StepProgressBarControl.NextElement(index);
                }

                StepProgressBarControl.NextElement(FirstIndexIsNotFinished());
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;// cancel back button
        }

        #region Timer
        private bool _enabledTimer;
        public string CurrentTime { get; set; }
        private readonly DateTime _startTrainingDateTime;
        public TimeSpan StartTime { get; set; }

        private bool OnTimerTick()
        {
            CurrentTime = (DateTime.Now - _startTrainingDateTime + StartTime).ToString(@"hh\:mm\:ss");
            OnPropertyChanged(nameof(CurrentTime));

            //save to restore if not finish
            SaveNotFinishedTraining(Items, TrainingItem.Title, TrainingItem.Id);

            return _enabledTimer;
        }

        private void SaveNotFinishedTraining(ObservableCollection<SuperSetViewModel> items, string title, int id)
        {
            Settings.IsTrainingNotFinishedTime = CurrentTime;
            var fn = "NotFinished.trday";
            var filename = Path.Combine(FileSystem.CacheDirectory, fn);

            TrainingViewModel training = new TrainingViewModel();
            training.Id = id;
            training.Title = title;

            foreach (var item in Items)
            {
                foreach (var trainingExerciseViewModel in item.SuperSetItems)
                {
                    training.AddExercise(trainingExerciseViewModel);
                }
            }

            training.SaveToFile(filename);
        }

        #endregion

        #region Finish
        private void FinishButtonClicked(object sender, EventArgs e)
        {
            try
            {
                Items[StepProgressBarControl.StepSelected].ForEach(item => item.IsNotFinished = false);

                if (Items.All(a => a.All(item => !item.IsNotFinished)) && _enabledTimer)
                {
                    _enabledTimer = false;
                    DeviceDisplay.KeepScreenOn = false;
                    SaveLastTraining();
                    SaveChangedExercises();
                    //SaveRemoved();

                    // если добавить упражненияво время выполнения , потом по завершению не вернуться в главную страницу,
                    // то при добавлении упражнении в след раз во время выполенияв тренировку добавляется по две
                    Application.Current.MainPage = new NavigationPage(new MainPage());
                    DependencyService.Get<IMessage>().ShowMessage(Resource.AdviceAfterTrainingMessage, Resource.AdviceString);
                    DependencyService.Get<IMessage>().ShortAlert(Resource.TrainingFinishedString);

                    DependencyService.Get<IAdInterstitial>().ShowAd("ca-app-pub-8728883017081055/7837401616");
                    DependencyService.Get<IMessage>().CancelNotification(App.TrainingNotificationId);
                }
                else
                {
                    StepProgressBarControl.DeselectElement();
                    StepProgressBarControl.NextElement(FirstIndexIsNotFinished());
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
        }

        private int FirstIndexIsNotFinished()
        {
            int index = 0;
            foreach (var trainingExerciseViewModel in Items)
            {
                if (trainingExerciseViewModel.First().IsNotFinished)
                {
                    return index;
                }

                index++;
            }

            return index;
        }

        private void SaveLastTraining()
        {
            Settings.IsTrainingNotFinished = false;
            App.Database.SaveLastTrainingItem(new LastTraining()
            {
                ElapsedTime = DateTime.Now - _startTrainingDateTime + StartTime,
                Time = _startTrainingDateTime,
                Title = TrainingItem.Title,
                TrainingId = TrainingItem.Id,
            });

            var id = App.Database.GetLastInsertId();
            foreach (var superSet in TrainingItem.ExercisesBySuperSet)
            {
                foreach (var item in superSet)
                {
                    App.Database.SaveLastTrainingExerciseItem(new LastTrainingExercise()
                    {
                        LastTrainingId = id,
                        OrderNumber = item.OrderNumber,
                        ExerciseName = item.ExerciseItemName,
                        MusclesString = MusclesConverter.ConvertFromListToString(item.Muscles.ToList()),
                        Description = item.ShortDescription,
                        ExerciseImageUrl = item.ExerciseImageUrl,
                        SuperSetId = item.SuperSetId,
                        TagsValue = ExerciseTagExtension.ConvertListToInt(item.Tags),
                        WeightAndRepsString = ExerciseTagExtension.ConvertJson(item.Tags, item)
                    });
                }
            }
        }

        private void SaveChangedExercises()
        {
            int order = 0;
            foreach (var superSet in Items)
            {
                foreach (var trainingExerciseViewModel in superSet)
                {
                    var exId = App.Database.SaveExerciseItem(trainingExerciseViewModel.GetExercise());
                    // save order numbers
                    order++;
                    App.Database.SaveTrainingExerciseItem(new TrainingExerciseComm()
                    {
                        ExerciseId = exId,
                        TrainingId = TrainingItem.Id,
                        OrderNumber = order,
                        Id = trainingExerciseViewModel.TrainingExerciseId,
                        SuperSetId = trainingExerciseViewModel.SuperSetId,
                        WeightAndRepsString = ExerciseTagExtension.ConvertJson(trainingExerciseViewModel.Tags, trainingExerciseViewModel),
                    });
                }
            }
        }


        #endregion

        public ICommand AddExercisesCommand => new Command(AddExercisesRequest);
        private async void AddExercisesRequest()
        {
            var vm = new ExerciseListPageViewModel() { Navigation = Navigation };
            vm.ExercisesChousen += (sender, args) =>
            {
                AddExercises(vm);
            };
            await Navigation.PushAsync(new ExerciseListPage(vm));
        }

        private void AddExercises(ExerciseListPageViewModel obj)
        {
            if (obj != null)
            {
                var selectedItems = obj.GetSelectedItems();
                selectedItems.ForEach(a => a.IsSelected = false);
                foreach (var exerciseItem in selectedItems)
                {
                    var newSuperSet = new SuperSetViewModel(){TrainingId = TrainingItem.Id};
                    newSuperSet.Add(exerciseItem);
                    
                    Items.Add(newSuperSet);
                    OnPropertyChanged(nameof(Items));

                    //Items.Last().ForEach(a=>a.TrainingId = TrainingItem.Id);
                    exerciseItem.TrainingId = TrainingItem.Id;
                    exerciseItem.OrderNumber = Items.Count - 1;
                    var id = App.Database.SaveTrainingExerciseItem(exerciseItem.GetTrainingExerciseComm());
                    exerciseItem.TrainingExerciseId = id;
                }
            }
        }

        private void CancelTrainingClicked(object sender, EventArgs e)
        {
            QuestionPopup popup = new QuestionPopup("", Resource.CancelTrainingQuestion);
            popup.PopupClosed += (o, closedArgs) =>
            {
                if (closedArgs.Button == Resource.OkString)
                {
                    DeviceDisplay.KeepScreenOn = false;
                    Settings.IsTrainingNotFinished = false;
                    Application.Current.MainPage = new NavigationPage(new MainPage());
                }
            };
            popup.Show(Resource.OkString, Resource.CancelString);
        }



        public ICommand ViewFullScreenTimeCommand => new Command(ViewFullScreenTime);
        public bool IsViewFullScreenTime { get; set; }
        private void ViewFullScreenTime()
        {
            IsViewFullScreenTime = !IsViewFullScreenTime;
            OnPropertyChanged(nameof(IsViewFullScreenTime));
            DeviceDisplay.KeepScreenOn = IsViewFullScreenTime;
            if (IsViewFullScreenTime)
            {
                DependencyService.Get<IDeviceConfig>().SetBrightness(0.3f);
            }
            else
            {
                DependencyService.Get<IDeviceConfig>().SetBrightness(-1.0f);
            }
        }

        //List<SuperSetViewModel> removedList = new List<SuperSetViewModel>();
        //private void RemoveExerciseClicked(object sender, EventArgs e)
        //{
        //    removedList.Add(Items[StepProgressBarControl.StepSelected]);

        //    Items.RemoveAt(StepProgressBarControl.StepSelected);
        //    OnPropertyChanged(nameof(Items));
        //    DependencyService.Get<IMessage>().ShortAlert(Resource.DeletedString);
        //}

        //private void SaveRemoved()
        //{
        //    foreach (var superSet in removedList)
        //    {
        //        if (superSet.Count > 1)
        //        {
        //            App.Database.DeleteSuperSetItem(superSet.Id);
        //        }
        //        foreach (var trainingExerciseViewModel in superSet)
        //        {
        //            App.Database.DeleteTrainingExerciseItem(trainingExerciseViewModel.TrainingExerciseId);
        //        }
        //    }
        //}
    }
}