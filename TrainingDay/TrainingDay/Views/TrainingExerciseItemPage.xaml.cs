using System;
using System.Collections.ObjectModel;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.ViewModels;
using TrainingDay.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrainingExerciseItemPage : ContentPage
    {
        public TrainingExerciseItemPage()
        {
            InitializeComponent();
        }

        // if we make changes, but after press back, changes is saved
        protected override void OnDisappearing()
        {
            if (isSaved)
            {
                return;
            }
            var item = ExerciseView.CurrentExercise;
            item.ExerciseItemName = _itemClone.ExerciseItemName;
            item.TrainingExerciseId = _itemClone.TrainingExerciseId;
            item.ExerciseId = _itemClone.ExerciseId;
            item.TrainingId = _itemClone.TrainingId;
            item.ExerciseImageUrl = _itemClone.ExerciseImageUrl;
            item.Muscles = _itemClone.Muscles;
            item.Tags = _itemClone.Tags;
            item.Distance = _itemClone.Distance;
            item.Time = _itemClone.Time;
            item.OrderNumber = _itemClone.OrderNumber;
            item.ShortDescription = _itemClone.ShortDescription;
            item.WeightAndRepsItems = new ObservableCollection<WeightAndReps>();
            foreach (var itemWeightAndReps in _itemClone.WeightAndRepsItems)
            {
                item.WeightAndRepsItems.Add(new WeightAndReps(itemWeightAndReps.Weight, itemWeightAndReps.Repetitions));
            }
            base.OnDisappearing();
        }

        private bool isSaved;
        private void Save_clicked(object sender, EventArgs e)
        {        
            if (!String.IsNullOrEmpty(ExerciseView.CurrentExercise.ExerciseItemName))
            {
                isSaved = true;
                DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
                App.Database.SaveExerciseItem(ExerciseView.CurrentExercise.GetExercise());
                App.Database.SaveTrainingExerciseItem(ExerciseView.CurrentExercise.GetTrainingExerciseComm());
                this.Navigation.PopAsync();
            }
        }

        private TrainingExerciseViewModel _itemClone;
        public void LoadExercise(TrainingExerciseViewModel item)
        {
            _itemClone = item.Clone();
            Title = item.ExerciseItemName;
            ExerciseView.BindingContext = item;
        }

        private void ExerciseView_OnImageTappedEvent(object sender, ImageSource e)
        {
            FullscreenImage.Source = e;
            ImageFrame.IsVisible = true;
        }

        private void FullscreenImageTapped(object sender, EventArgs e)
        {
            ImageFrame.IsVisible = false;
        }
    }
}