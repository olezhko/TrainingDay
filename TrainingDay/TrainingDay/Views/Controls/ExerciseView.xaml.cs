using System;
using System.ComponentModel;
using TrainingDay.Resources;
using TrainingDay.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExerciseView : ContentView,INotifyPropertyChanged
    {
        public ExerciseView()
        {
            InitializeComponent();

            BindingContextChanged += ExerciseView_BindingContextChanged;
        }

        private void ExerciseView_BindingContextChanged(object sender, EventArgs e)
        {
            MusclesWrapPanel.ItemsSource = ((TrainingExerciseViewModel)BindingContext).Muscles;
        }

        public TrainingExerciseViewModel CurrentExercise
        {
            get
            {
                var ex = (TrainingExerciseViewModel) BindingContext;
                return ex;
            }
        }

        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            EntryPopup popup = new EntryPopup(Resource.EnterImageUrlString, CurrentExercise.ExerciseImageUrl);
            popup.PopupClosed += (o, closedArgs) =>
            {
                if (closedArgs.Button == "OK")
                {
                    CurrentExercise.ExerciseImageUrl = closedArgs.Text;
                    OnPropertyChanged(nameof(TrainingExerciseViewModel.ExerciseImageUrl));
                }
            };

            popup.Show();
        }
    }
}