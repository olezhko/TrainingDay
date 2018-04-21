using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace TrainingDay.Model
{
    public class TrainingItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ObservableCollection<Exercise> Exercises { get; set; }

        public TrainingItem()
        {
            Exercises = new ObservableCollection<Exercise>();
        }
    }
    [Table("TrainingExerciseComm")]
    public class TrainingExerciseComm
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public int TrainingId { get; set; }
        public int ExerciseId { get; set; }
    }


    [Table("Exercises")]
    public class Exercise
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public double Weight { get; set; } // in kilograms
        public string ExerciseItemName { get; set; }
        public int CountOfApproches { get; set; }
        public int CountOfTimes { get; set; }
        public string ShortDescription { get; set; }
        public int Muscles { get; set; }
    }

    [Table("Trainings")]
    public class Training
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
    }

    [Table("LastTrainings")]
    public class LastTraining
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        public int TrainingId { get; set; }
        public DateTime Time { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }

}
