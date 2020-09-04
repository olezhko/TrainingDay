using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using TrainingDay.Model;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.ViewModels;
using TrainingDay.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Exception = System.Exception;

namespace TrainingDay.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExerciseItemPage : ContentPage
    {
        public ExerciseItemPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ExerciseViewModel item = BindingContext as ExerciseViewModel;
            if (item.ExerciseImageUrl != null)
            {
                AdviceDescEditor.IsReadOnly = true;
                StartingDescEditor.IsReadOnly = true;
                ExecDescEditor.IsReadOnly = true;
                NameEditor.IsReadOnly = true;
                MusclesWrapPanel.IsEditableItems = false;
            }
            else
            {
                AdviceDescEditor.IsReadOnly = false;
                StartingDescEditor.IsReadOnly = false;
                ExecDescEditor.IsReadOnly = false;
                NameEditor.IsReadOnly = false;
                MusclesWrapPanel.IsEditableItems = true;
            }

            NameLabel.IsVisible = item.ExerciseImageUrl != null;

            NameEditor.IsVisible = item.ExerciseImageUrl == null;
            ExerciseByDistanceCheckBox.IsEnabled = item.ExerciseImageUrl == null;
            ExerciseByRepsAndWeightCheckBox.IsEnabled = item.ExerciseImageUrl == null;
            ExerciseByTimeCheckBox.IsEnabled = item.ExerciseImageUrl == null;

            ExerciseByDistanceCheckBox.IsChecked = item.Tags.Contains(ExerciseTags.ExerciseByDistance);
            ExerciseByRepsAndWeightCheckBox.IsChecked = item.Tags.Contains(ExerciseTags.ExerciseByRepsAndWeight);
            ExerciseByTimeCheckBox.IsChecked = item.Tags.Contains(ExerciseTags.ExerciseByTime);
        }

        private void Save_clicked(object sender, EventArgs e)
        {
            try
            {
                ExerciseViewModel ex = BindingContext as ExerciseViewModel;
                ex.Tags.Clear();
                if(ExerciseByDistanceCheckBox.IsChecked) ex.Tags.Add(ExerciseTags.ExerciseByDistance);
                if(ExerciseByRepsAndWeightCheckBox.IsChecked) ex.Tags.Add(ExerciseTags.ExerciseByRepsAndWeight);
                if(ExerciseByTimeCheckBox.IsChecked) ex.Tags.Add(ExerciseTags.ExerciseByTime);

                if (!String.IsNullOrEmpty(ex.ExerciseItemName))
                {
                    App.Database.SaveExerciseItem(ex.GetExercise());
                    if (ex.Id == 0)
                        NewExerciseAdded?.Invoke(this, ex.GetExercise());
                    else
                        ExerciseChanged?.Invoke(this, ex.GetExercise());

                    DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
                    this.Navigation.PopAsync();
                }
            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        public event EventHandler<Exercise> NewExerciseAdded;
        public event EventHandler<Exercise> ExerciseChanged;
    }

    public class ExerciseViewModel: BaseViewModel
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

        private List<ExerciseTags> _tags;
        public List<ExerciseTags> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                OnPropertyChanged();
            }
        }

        private string startingPositionDescription;
        public string StartingPositionDescription
        {
            get => startingPositionDescription;
            set
            {
                startingPositionDescription = value;
                OnPropertyChanged();
            }
        }

        private string executionDescription;
        public string ExecutionDescription
        {
            get => executionDescription;
            set
            {
                executionDescription = value;
                OnPropertyChanged();
            }
        }

        private string adviceDescription;
        public string AdviceDescription
        {
            get => adviceDescription;
            set
            {
                adviceDescription = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MuscleViewModel> Muscles { get; set; }
        public ExerciseViewModel()
        {
            Muscles = new ObservableCollection<MuscleViewModel>();
            Tags = new List<ExerciseTags>();
        }

        public ExerciseViewModel(Exercise exercise)
        {
            Tags = ExerciseTagExtension.ConvertFromIntToList(exercise.TagsValue);
            ShortDescription = exercise.Description;
            ExerciseItemName = exercise.ExerciseItemName;
            Id = exercise.Id;
            ExerciseImageUrl = exercise.ExerciseImageUrl;
            Muscles = new ObservableCollection<MuscleViewModel>(MusclesConverter.ConvertFromStringToList(exercise.MusclesString));
            try
            {
                TrainingExerciseViewModel.Description descriptionsStrings = JsonConvert.DeserializeObject<TrainingExerciseViewModel.Description>(exercise.Description);
                AdviceDescription = descriptionsStrings.advice;
                ExecutionDescription = descriptionsStrings.exec;
                StartingPositionDescription = descriptionsStrings.start;
            }
            catch (Exception e)
            {
                ExecutionDescription = exercise.Description;
            }
        }

        public Exercise GetExercise()
        {
            return new Exercise()
            {
                Id = Id,
                Description = ShortDescription,
                ExerciseImageUrl = ExerciseImageUrl,
                ExerciseItemName = _name,
                MusclesString = MusclesConverter.ConvertFromListToString(Muscles.ToList()),
                TagsValue = ExerciseTagExtension.ConvertListToInt(Tags)
            };
        }
    }
}