using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.Code;
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
            return true;
        }

        public string CurrentTime { get; set; }
        public ObservableCollection<ExerciseSelectViewModel> Items { get; set; }
        public TrainingView TrainingItem { get; set; }

        private void FinishButtonClicked(object sender, EventArgs e)
        {
            App.Database.SaveLastTrainingItem(new LastTraining()
            {
                ElapsedTime = DateTime.Now - startTrainingDateTime,
                Time = startTrainingDateTime,
                TrainingId = TrainingItem.TrainingId,
            });
            DependencyService.Get<IMessage>().ShortAlert(Resource.TrainingFinishedString);
        }
    }
}