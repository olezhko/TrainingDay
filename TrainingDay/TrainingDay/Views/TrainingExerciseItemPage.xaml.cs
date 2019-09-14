using System;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.ViewModels;
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


        // if we make chages, but after press back, changes is saved
        protected override void OnDisappearing()
        {
            var item = ExerciseView.CurrentExercise;
            item.Weight = _item.Weight;
            item.ExerciseItemName = _item.ExerciseItemName;
            item.CountOfApproches = _item.CountOfApproches;
            item.CountOfTimes = _item.CountOfTimes;
            item.TrainingExerciseId = _item.TrainingExerciseId;
            item.ExerciseId = _item.ExerciseId;
            item.TrainingId = _item.TrainingId;
            item.ExerciseImageUrl = _item.ExerciseImageUrl;
            item.Muscles = _item.Muscles;
            item.OrderNumber = _item.OrderNumber;
            item.ShortDescription = _item.ShortDescription;

            base.OnDisappearing();
        }

        private void Save_clicked(object sender, EventArgs e)
        {        
            if (!String.IsNullOrEmpty(ExerciseView.CurrentExercise.ExerciseItemName))
            {
                _item = ExerciseView.CurrentExercise;
                DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
                App.Database.SaveExerciseItem(ExerciseView.CurrentExercise.GetExercise());
                App.Database.SaveTrainingExerciseItem(ExerciseView.CurrentExercise.GeTrainingExerciseComm());
                this.Navigation.PopAsync();
            }
        }

        private TrainingExerciseViewModel _item;
        public void LoadExercise(TrainingExerciseViewModel item)
        {
            _item = item.Clone();
            ExerciseView.BindingContext = item;
        }
    }
}