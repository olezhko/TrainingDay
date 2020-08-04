﻿using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using TrainingDay.Model;
using TrainingDay.Services;
using TrainingDay.Views.Controls;

namespace TrainingDay.ViewModels
{
    [Serializable]
    public class TrainingExerciseViewModel : BaseViewModel
    {
        #region Prop
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

        public int OrderNumber { get; set; }

        private ObservableCollection<MuscleViewModel> muscles;
        public ObservableCollection<MuscleViewModel> Muscles
        {
            get => muscles;
            set
            {
                muscles = value;
                OnPropertyChanged();
            }
        }

        private int superSetId = -1;
        public int SuperSetId
        {
            get => superSetId;
            set
            {
                superSetId = value;
                OnPropertyChanged();
            }
        }

        private int _superSetNumInTraining = 0;
        public int SuperSetNum // see TrainingExercisesPage
        {
            get => _superSetNumInTraining;
            set
            {
                _superSetNumInTraining = value;
                OnPropertyChanged();
            }
        }
        
        public ObservableCollection<WeightAndReps> WeightAndRepsItems { get; set; } = new ObservableCollection<WeightAndReps>();

        private List<ExerciseTags> tags = new List<ExerciseTags>();
        public List<ExerciseTags> Tags
        {
            get => tags;
            set
            {
                tags = value;
                OnPropertyChanged();
            }
        }


        public TimeSpan Time { get; set; }
        public int TimeHours
        {
            get { return (int)Time.TotalHours; }
            set
            {
                Time = new TimeSpan(value,TimeMinutes,TimeSeconds);
                OnPropertyChanged();
            }
        }
        public int TimeMinutes
        {
            get { return Time.Minutes; }
            set
            {
                Time = new TimeSpan(TimeHours, value, TimeSeconds);
                OnPropertyChanged();
            }
        }
        public int TimeSeconds
        {
            get { return Time.Seconds; }
            set
            {
                Time = new TimeSpan(TimeHours, TimeMinutes, value); 
                OnPropertyChanged();
            }
        }


        public string DistanceString
        {
            get => Distance.ToString(CultureInfo.InvariantCulture);
            set
            {
                var res = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var newvalue);
                if (res)
                {
                    Distance = newvalue;
                }
            }
        }

        private double distance;
        public double Distance
        {
            get => distance;
            set
            {
                distance = value;
                OnPropertyChanged();
            } }
        #endregion

        public TrainingExerciseViewModel() { }

        public TrainingExerciseViewModel(Exercise exercise, TrainingExerciseComm comm)
        {
            try
            {
                ExerciseId = exercise.Id;
                Muscles = new ObservableCollection<MuscleViewModel>(MusclesConverter.ConvertFromStringToList(exercise.MusclesString));
                ShortDescription = exercise.Description;
                ExerciseItemName = exercise.ExerciseItemName;
                ExerciseImageUrl = exercise.ExerciseImageUrl;
                TrainingId = comm.TrainingId;
                OrderNumber = comm.OrderNumber;
                TrainingExerciseId = comm.Id;
                SuperSetId = comm.SuperSetId;
                Tags = ExerciseTagExtension.ConvertFromIntToList(exercise.TagsValue);
                
                ExerciseTagExtension.ConvertJsonBack(this, comm.WeightAndRepsString);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public Exercise GetExercise()
        {
            return new Exercise()
            {
                Id = ExerciseId,
                Description = ShortDescription,
                ExerciseImageUrl = ExerciseImageUrl,
                ExerciseItemName = _name,
                MusclesString = MusclesConverter.ConvertFromListToString(Muscles.ToList()),
                TagsValue = ExerciseTagExtension.ConvertListToInt(Tags)
            };
        }

        public TrainingExerciseComm GetTrainingExerciseComm()
        {
            string weightAndReps = ExerciseTagExtension.ConvertJson(Tags, this);

            return new TrainingExerciseComm()
            {
                ExerciseId = ExerciseId,
                WeightAndRepsString = weightAndReps,
                TrainingId = TrainingId,
                Id = TrainingExerciseId,
                OrderNumber = OrderNumber,
                SuperSetId = SuperSetId
            };
        }

        public TrainingExerciseViewModel Clone()
        {
            return new TrainingExerciseViewModel(GetExercise(),GetTrainingExerciseComm());
        }
    }
}
