using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrainingDay.Code;
using TrainingDay.Controls;
using TrainingDay.Helpers;
using TrainingDay.Model;
using TrainingDay.ViewModel;
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
            Items = new ObservableCollection<ExerciseSelectViewModel>();
            this.BindingContext = this;
            startTrainingDateTime = DateTime.Now;
            EnabledTimer = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            foreach (var trainingItemExercise in TrainingItem.Exercises)
            {
                Items.Add(new ExerciseSelectViewModel(trainingItemExercise));
            }
            OnPropertyChanged(nameof(Items));
        }

        private bool OnTimerTick()
        {
            CurrentTime = (DateTime.Now - startTrainingDateTime).ToString(@"hh\:mm\:ss");
            OnPropertyChanged(nameof(CurrentTime));
            return EnabledTimer;
        }

        public string CurrentTime { get; set; }
        public ObservableCollection<ExerciseSelectViewModel> Items { get; set; }
        public TrainingView TrainingItem { get; set; }

        private bool EnabledTimer = false;
        private void FinishButtonClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
            App.Database.SaveLastTrainingItem(new LastTraining()
            {
                ElapsedTime = DateTime.Now - startTrainingDateTime,
                Time = startTrainingDateTime,
                TrainingId = TrainingItem.TrainingId,
            });
            SaveChangedExercises();
            DependencyService.Get<IMessage>().ShortAlert(Resource.TrainingFinishedString);
            EnabledTimer = false;
        }

        private void SaveChangedExercises()
        {
            foreach (var exerciseSelectViewModel in Items)
            {
                App.Database.SaveExerciseItem(exerciseSelectViewModel.GetExercise());
            }
        }
    }
}