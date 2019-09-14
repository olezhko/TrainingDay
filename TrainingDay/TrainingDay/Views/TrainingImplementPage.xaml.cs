using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.ViewModels;
using TrainingDay.Views;
using TrainingDay.Views.Controls;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrainingImplementPage : ContentPage
    {
        private DateTime startTrainingDateTime;
        public TrainingImplementPage()
        {
            InitializeComponent();
            Items = new ObservableCollection<TrainingExerciseViewModel>();
            if (Device.RuntimePlatform == Device.iOS)
                PromoView.AdUnitId = "iOS Key";
            else if (Device.RuntimePlatform == Device.Android)
                PromoView.AdUnitId = "ca-app-pub-8728883017081055/4843502807";

            this.BindingContext = this;
            startTrainingDateTime = DateTime.Now;
            EnabledTimer = true;
            StepProgressBarControl.PropertyChanged += StepProgressBarControl_PropertyChanged;
            Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
        }

        private void StepProgressBarControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StepProgressBar.StepSelected))
            {
                FinishButton.IsVisible = Items[StepProgressBarControl.StepSelected].IsNotFinished;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DeviceDisplay.KeepScreenOn = true;
            StepProgressBarControl.ItemsSource = TrainingItem.Exercises;
            Items = TrainingItem.Exercises;
            OnPropertyChanged(nameof(Items));
        }

        private bool OnTimerTick()
        {
            CurrentTime = (DateTime.Now - startTrainingDateTime).ToString(@"hh\:mm\:ss");
            OnPropertyChanged(nameof(CurrentTime));
            return EnabledTimer;
        }

        public string CurrentTime { get; set; }
        public ObservableCollection<TrainingExerciseViewModel> Items { get; set; }
        public TrainingViewModel TrainingItem { get; set; }

        private bool EnabledTimer = false;
        private void FinishButtonClicked(object sender, EventArgs e)
        {
            try
            {
                Items[StepProgressBarControl.StepSelected].IsNotFinished = false;

                if (Items.All(a => !a.IsNotFinished) && EnabledTimer)
                {
                    EnabledTimer = false;
                    DeviceDisplay.KeepScreenOn = false;
                    FinishIfLast();
                    Navigation.PopAsync();
                }
                else
                {
                    StepProgressBarControl.DeselectElement();
                    StepProgressBarControl.NextElement(FirstIndexIsNotFinished());
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                Navigation.PopAsync();
            }

        }

        private int FirstIndexIsNotFinished()
        {
            int index = 0;
            foreach (var trainingExerciseViewModel in Items)
            {
                if (trainingExerciseViewModel.IsNotFinished)
                {
                    return index;
                }

                index++;
            }

            return index;
        }

        public void FinishIfLast()
        {
            App.Database.SaveLastTrainingItem(new LastTraining()
            {
                ElapsedTime = DateTime.Now - startTrainingDateTime,
                Time = startTrainingDateTime,
                Title = TrainingItem.Title,
                TrainingId = TrainingItem.Id,
            });
            var id = App.Database.GetLastInsertId();
            foreach (var item in TrainingItem.Exercises)
            {
                App.Database.SaveLastTrainingExerciseItem(new LastTrainingExercise()
                {
                    CountOfApproches = item.CountOfApproches,
                    CountOfTimes = item.CountOfTimes,
                    LastTrainingId = id,
                    Weight = item.Weight,
                    OrderNumber = item.OrderNumber,
                    ExerciseName = item.ExerciseItemName,
                    Muscles = MusclesConverter.ConvertBack(item.Muscles.ToList()),
                    Description = item.ShortDescription,
                    ExerciseImageUrl = item.ExerciseImageUrl
                });
            }
            SaveChangedExercises();
            DependencyService.Get<IMessage>().ShowMessage(Resource.AdviceAfterTrainingMessage, Resource.AdviceString);
            DependencyService.Get<IMessage>().ShortAlert(Resource.TrainingFinishedString);

            DependencyService.Get<IAdInterstitial>().ShowAd("ca-app-pub-8728883017081055/7837401616");
        }

        private void SaveChangedExercises()
        {
            int index = 0;
            foreach (var exerciseSelectViewModel in Items)
            {
                int id = App.Database.SaveExerciseItem(exerciseSelectViewModel.GetExercise());
                App.Database.SaveTrainingExerciseItem(new TrainingExerciseComm()
                {
                    Id= exerciseSelectViewModel.TrainingExerciseId,
                    Weight = exerciseSelectViewModel.Weight,
                    OrderNumber = index,
                    ExerciseId = id,
                    CountOfTimes = exerciseSelectViewModel.CountOfTimes,
                    CountOfApproches = exerciseSelectViewModel.CountOfApproches,
                    TrainingId = TrainingItem.Id,
                });
                index++;
            }
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
                selectedItems.ForEach(a => a.IsSelected = false);
                foreach (var exerciseItem in selectedItems)
                {
                    Items.Add(exerciseItem);
                }
            }
            OnPropertyChanged(nameof(Items));
        }

        private void CancelTraniningClicked(object sender, EventArgs e)
        {
            QuestionPopup popup = new QuestionPopup("", Resource.CancelTrainingQuestion);
            popup.PopupClosed += (o, closedArgs) =>
            {
                if (closedArgs.Button == "Yes")
                {
                    Navigation.PopAsync();
                }
            };
            popup.Show("Yes","No");
        }
    }
}