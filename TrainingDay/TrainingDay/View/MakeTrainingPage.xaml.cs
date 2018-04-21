using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class MakeTrainingPage : ContentPage
    {
        public bool IsVisibleNoTrainingsNeedAddNewLabel { get; set; }
        public ObservableCollection<TrainingViewModel> Items { get; set; }
        public MakeTrainingPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadItems();
        }

        public void LoadItems()
        {
            if (Items==null)
            {
                Items = new ObservableCollection<TrainingViewModel>();
            }
            Items.Clear();
            var trainingExerciseItems = App.Database.GetTrainingExerciseItems();
            var exerciseItems = App.Database.GetExerciseItems();
            var trainingsItems = App.Database.GetTrainingItems();

            if (trainingsItems != null && trainingsItems.Any())
            {
                foreach (var training in trainingsItems)
                {
                    if (!Items.Any(a => a.Id == training.Id))
                    {
                        Items.Add(new TrainingViewModel() { Id = training.Id, Description = training.Description, Title = training.Title });
                    }

                    var allExercises = trainingExerciseItems.Where(ex => ex.TrainingId == training.Id).ToList();
                    var trainingViewModel = Items.First(tr => tr.Id == training.Id);
                    foreach (var allExercise in allExercises)
                    {
                        trainingViewModel.Exercises.Add(exerciseItems.First(ex => ex.Id == allExercise.ExerciseId));
                    }
                }
            }
            DrawItems();
            IsVisibleNoTrainingsNeedAddNewLabel = !Items.Any();
            OnPropertyChanged(nameof(IsVisibleNoTrainingsNeedAddNewLabel));
        }

        private void DrawItems()
        {
            LeftStackLayout.Children.Clear();
            foreach (var trainingViewModel in Items)
            {
                TrainingView item = new TrainingView();
                item.Title = trainingViewModel.Title;
                item.Description = trainingViewModel.Description;
                item.TrainingId = trainingViewModel.Id;
                item.LoadExercise(trainingViewModel.Exercises);
                item.TrainingTapped += Item_TrainingTapped;
                LeftStackLayout.Children.Add(item);
            }
        }

        private void Item_TrainingTapped(object sender, EventArgs e)
        {
            var training = sender as TrainingView;
            Navigation.PushAsync(new TrainingImplementPage() {TrainingItem = training, Title = training.Title});
        }
    }
}