using System;
using System.Collections.ObjectModel;
using System.Linq;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.ViewModels;
using TrainingDay.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExerciseItemPage : ContentPage
    {
        public ExerciseItemPage()
        {
            InitializeComponent();
        }

        private void Save_clicked(object sender, EventArgs e)
        {
            try
            {
                ExerciseViewModel ex = BindingContext as ExerciseViewModel;

                if (!String.IsNullOrEmpty(ex.ExerciseItemName))
                {
                    DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
                    App.Database.SaveExerciseItem(ex.GetExercise());
                    this.Navigation.PopAsync();
                }

            }
            catch (Exception exception)
            {
            }
        }

        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            ExerciseViewModel ex = BindingContext as ExerciseViewModel;

            EntryPopup popup = new EntryPopup(Resource.EnterImageUrlString, ex.ExerciseImageUrl);
            popup.PopupClosed += (o, closedArgs) =>
            {
                if (closedArgs.Button == "OK")
                {
                    ex.ExerciseImageUrl = closedArgs.Text;
                    OnPropertyChanged(nameof(ExerciseViewModel.ExerciseImageUrl));
                }
            };

            popup.Show();
        }
    }


    public class ExerciseTagViewModel : BaseViewModel
    {

    }


    class ExerciseViewModel: BaseViewModel
    {
        public int Id { get; set; }
        private string _name;
        public string ExerciseItemName
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        private string _description;
        public string ShortDescription
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string imageUrl;

        public string ExerciseImageUrl
        {
            get => imageUrl;
            set
            {
                imageUrl = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MuscleViewModel> Muscles { get; set; }




        public ExerciseViewModel()
        {
            Muscles = new ObservableCollection<MuscleViewModel>();
        }

        public ExerciseViewModel(Exercise exercise)
        {
            ShortDescription = exercise.Description;
            ExerciseItemName = exercise.ExerciseItemName;
            Id = exercise.Id;
            ExerciseImageUrl = exercise.ExerciseImageUrl;
            Muscles = new ObservableCollection<MuscleViewModel>(MusclesConverter.Convert(exercise.Muscles));
        }

        public Exercise GetExercise()
        {
            var res = new Exercise()
            {
                Description = ShortDescription,
                Muscles = MusclesConverter.ConvertBack(Muscles.ToList()),
                Id = Id,
                ExerciseImageUrl = ExerciseImageUrl,
                ExerciseItemName = ExerciseItemName
            };
            return res;
        }
    }
}