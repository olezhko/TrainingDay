using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.Model;
using TrainingDay.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Code
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrainingView : ContentView,INotifyPropertyChanged
    {
        public event EventHandler TrainingTapped;
        public TrainingView()
        {
            InitializeComponent();
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += Tap_Tapped;
            GestureRecognizers.Add(tap);
            this.BindingContext = this;
            Exercises = new ObservableCollection<Exercise>();
        }

        private void Tap_Tapped(object sender, EventArgs e)
        {
            TrainingTapped?.Invoke(this,null);
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }
        private string description;
        public string Description
        {
            get { return description; }
            set { description = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Exercise> Exercises { get; set; }
        public int TrainingId { get; set; }

        public void LoadExercise(ObservableCollection<Exercise> exercises)
        {
            FrameListView.IsVisible = exercises.Count > 0;
            foreach (var exercise in exercises)
            {
                Exercises.Add(exercise);
                ExercisesStackLayout.Children.Add(new Label(){ Text =  exercise.ExerciseItemName});
                ExercisesStackLayout.Children.Add(new BoxView() { HeightRequest = 1, Color = Color.Black});
            }
        }
    }
}