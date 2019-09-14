using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TrainingDay.Services;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    public class TrainingExerciseViewModel : BaseViewModel
    {
        public int TrainingExerciseId { get; set; }

        public int ExerciseId { get; set; }
        public int TrainingId { get; set; }

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

        private int _approaches = 5;
        public int CountOfApproches
        {
            get => _approaches;
            set
            {
                _approaches = value;
                OnPropertyChanged();
            }
        }

        private int _times = 15;
        public int CountOfTimes
        {
            get => _times;
            set
            {
                _times = value;
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

        private double _weight = 5;
        public double Weight
        {
            get => _weight;
            set
            {
                _weight = value;
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

        public int OrderNumber { get; set; }

        public ObservableCollection<MuscleViewModel> Muscles { get; set; }

        public TrainingExerciseViewModel()
        {
            ChangeExpanedCommand = new Command(() =>
            {
                State = State == ExpandedState.Expanded ? ExpandedState.NoExpanded : ExpandedState.Expanded;
                OnPropertyChanged(nameof(State));
            });
        }

        public TrainingExerciseViewModel(Exercise exercise, TrainingExerciseComm comm):this()
        {
            ExerciseId = exercise.Id;
            Weight = comm.Weight;
            Muscles = new ObservableCollection<MuscleViewModel>(MusclesConverter.Convert(exercise.Muscles));
            CountOfApproches = comm.CountOfApproches;
            CountOfTimes = comm.CountOfTimes;
            ShortDescription = exercise.Description;
            ExerciseItemName = exercise.ExerciseItemName;
            ExerciseImageUrl = exercise.ExerciseImageUrl;
            TrainingId = comm.TrainingId;
            OrderNumber = comm.OrderNumber;
            TrainingExerciseId = comm.Id;
        }

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


        public enum ExpandedState
        {
            NoExpanded,
            Expanded
        }

        public ExpandedState State { get; set; } = ExpandedState.NoExpanded;
        public ICommand ChangeExpanedCommand { get; set; }

        public Exercise GetExercise()
        {
            return new Exercise()
            {
                Id = ExerciseId,
                Description = ShortDescription,
                ExerciseImageUrl = ExerciseImageUrl,
                ExerciseItemName = _name,
                Muscles = MusclesConverter.ConvertBack(Muscles.ToList())
            };
        }

        public TrainingExerciseComm GeTrainingExerciseComm()
        {
            return new TrainingExerciseComm()
            {
                ExerciseId = ExerciseId,
                CountOfApproches = CountOfApproches,
                CountOfTimes = CountOfTimes,
                Weight = Weight,
                TrainingId = TrainingId,
                Id = TrainingExerciseId,
                OrderNumber = OrderNumber
            };
        }

        public TrainingExerciseViewModel Clone()
        {
            return new TrainingExerciseViewModel(GetExercise(),GeTrainingExerciseComm());
        }


        private bool notFinished = true;
        public bool IsNotFinished
        {
            get => notFinished;
            set
            {
                notFinished = value; 
                OnPropertyChanged();
            }
        }
    }
}
