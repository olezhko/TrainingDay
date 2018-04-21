using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.Code;
using TrainingDay.Model;
using TrainingDay.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ExerciseView : ContentView
	{
		public ExerciseView()
		{
			InitializeComponent();
		    for (double i = 0.5; i < 400; i += 0.5)
		    {
		        WeightPicker.Items.Add(i.ToString());
		    }
        }

	    public ExerciseSelectViewModel CurrentExercise
	    {
	        get
	        {
	            var ex = (ExerciseSelectViewModel)BindingContext;
	            ex.Weight = Convert.ToDouble(WeightPicker.SelectedItem, CultureInfo.CurrentCulture);
                return ex;
	        }
	    }

	    private int Muscles;
	    private void Picker_OnSelectedIndexChanged(object sender, EventArgs e)
	    {
            Muscles = MusclesConverter.SetMuscles(Muscles, MusclesPicker.SelectedItem);
            MusclesWrapPanel.ItemsSource = null;
            MusclesWrapPanel.ItemsSource = new ObservableCollection<MuscleViewModel>(MusclesConverter.Convert(Muscles));
        }
    }


    public class ExerciseSelectViewModel : BaseViewModel
    {
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public int Id { get; set; }
        public string ExerciseItemName { get; set; }
        public int CountOfApproches { get; set; }
        public int CountOfTimes { get; set; }
        public string ShortDescription { get; set; }
        public ObservableCollection<MuscleViewModel> Muscles { get; set; }
        public double Weight { get; set; }

        public ExerciseSelectViewModel(Exercise baseExercise)
        {
            Id = baseExercise.Id;
            Weight = baseExercise.Weight;
            Muscles = new ObservableCollection<MuscleViewModel>(MusclesConverter.Convert(baseExercise.Muscles));
            CountOfApproches = baseExercise.CountOfApproches;
            CountOfTimes = baseExercise.CountOfTimes;
            ShortDescription = baseExercise.ShortDescription;
            ExerciseItemName = baseExercise.ExerciseItemName;
        }

        public ExerciseSelectViewModel()
        {
            Muscles = new ObservableCollection<MuscleViewModel>();
        }

        public Exercise GetExercise()
        {
            return new Exercise(){Id = Id,CountOfApproches = CountOfApproches,CountOfTimes = CountOfTimes, Weight = Weight,ShortDescription = ShortDescription,
                ExerciseItemName = ExerciseItemName, Muscles = MusclesConverter.ConvertBack(Muscles.ToList())};
        }
    }
}