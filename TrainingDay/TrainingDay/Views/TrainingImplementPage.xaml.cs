using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
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
                SkipButton.IsVisible = Items[StepProgressBarControl.StepSelected].First().IsNotFinished;
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

            UpdateTime();

            //save to restore if not finish
            SaveNotFinishedTraining(TrainingItem.Title, TrainingItem.Id);
            UpdateNotifyTimer();
            return _enabledTimer;
        }

        private void SaveNotFinishedTraining(string title, int id)
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

        private void UpdateNotifyTimer()
        {
            if (_enabledTimer)
            {
                var name = string.Join(" - ", Items[StepProgressBarControl.StepSelected].Select(a => a.ExerciseItemName));
                if (Device.RuntimePlatform == Device.Android)
                    DependencyService.Get<IMessage>().ShowNotification(App.TrainingImplementTimeId, name,
                        CurrentTime, true, true);
            }
        }

        private void UpdateTime()
        {
            var exercises = Items[StepProgressBarControl.StepSelected];
            foreach (var item in exercises)
            {
                if (item.Tags.Contains(ExerciseTags.ExerciseByTime) && item.IsTimeCalculating && item.IsNotFinished)
                {
                    item.Time = DateTime.Now - item.StartCalculateDateTime;
                }
            }
        }
        #endregion

        #region Finish
        private async void FinishButtonClicked(object sender, EventArgs e)
        {
            try
            {
                Items[StepProgressBarControl.StepSelected].ForEach(item => item.IsNotFinished = false);

                Items[StepProgressBarControl.StepSelected].ForEach(item => item.IsSkipped = false);

                
                if (Items.All(a => a.All(item => !item.IsNotFinished || item.IsSkipped)) && _enabledTimer)
                {
                    _enabledTimer = false;
                    DependencyService.Get<IMessage>().CancelNotification(App.TrainingImplementTimeId);
                    DeviceDisplay.KeepScreenOn = false;
                    Settings.IsTrainingNotFinished = false;
                    SaveLastTraining();
                    SaveChangedExercises();

                    DependencyService.Get<IMessage>().ShowMessage(Resource.AdviceAfterTrainingMessage, Resource.AdviceString);
                    DependencyService.Get<IMessage>().ShortAlert(Resource.TrainingFinishedString);

                    DependencyService.Get<IAdInterstitial>().ShowAd(Device.RuntimePlatform == Device.Android
                        ? "ca-app-pub-8728883017081055/7837401616"
                        : "ca-app-pub-8728883017081055/1550276858");

                    DependencyService.Get<IMessage>().CancelNotification(App.TrainingNotificationId);

                    await SiteService.SendFinishedWorkout(Settings.Token);
                    await Navigation.PopAsync();
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
                await Navigation.PopAsync();
            }
        }

        private int FirstIndexIsNotFinished()
        {
            int index = 0;
            foreach (var trainingExerciseViewModel in Items)
            {
                if (trainingExerciseViewModel.First().IsNotFinished && !trainingExerciseViewModel.First().IsSkipped)
                {
                    return index;
                }

                index++;
            }

            return index;
        }

        private void SaveLastTraining()
        {
            App.Database.SaveLastTrainingItem(new LastTraining()
            {
                ElapsedTime = DateTime.Now - _startTrainingDateTime + StartTime,
                Time = _startTrainingDateTime,
                Title = TrainingItem.Title,
                TrainingId = TrainingItem.Id,
            });

            var id = App.Database.GetLastInsertId();
            foreach (var superSet in Items)
            {
                foreach (var item in superSet)
                {
                    if (item.IsSkipped)
                    {
                        continue;
                    }
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
                if (closedArgs.Button == Resource.YesString)
                {
                    _enabledTimer = false;
                    DependencyService.Get<IMessage>().CancelNotification(App.TrainingImplementTimeId);
                    DeviceDisplay.KeepScreenOn = false;
                    Settings.IsTrainingNotFinished = false;

                    Navigation.PopAsync();
                }
            };
            popup.Show(Resource.YesString, Resource.NoString);
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

        private void SkipButtonClicked(object sender, EventArgs e)
        {
            Items[StepProgressBarControl.StepSelected].ForEach(item => item.IsSkipped = !item.IsSkipped);// reverse skipped in exercise

            if (Items[StepProgressBarControl.StepSelected].First().IsSkipped) // if ex or superset not skipped
            {
                StepProgressBarControl.SkipElement(); // make it skipped in step
                StepProgressBarControl.NextElement(FirstIndexIsNotFinished());
            }
            else
            {
                StepProgressBarControl.DeSkipElement();
            }
        }
    }
}