using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Newtonsoft.Json;
using TrainingDay.ViewModels;
using TrainingDay.Views.Controls;
using Xamarin.Forms;

namespace TrainingDay.Model
{
    public enum ExerciseTags
    {
        CanDoAtHome = 0,
        // type of reps 
        ExerciseByTime,
        ExerciseByDistance,
        ExerciseByRepsAndWeight,

        // inventory
        BarbellExist,   //штанга
        DumbbellExist,  //гантели
        BenchExist, 
        
        DatabaseExercise,
        Last
    }


    public class ExerciseTagExistsConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ExerciseTags tagChecked = (ExerciseTags)parameter;
            List<ExerciseTags> tags = value as List<ExerciseTags>;
            if (tags == null)
            {
                return false;
            }
            return tags.Contains(tagChecked);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
            //ExerciseTags tagChecked = (ExerciseTags)parameter;
            //List<ExerciseTags> tags = new List<ExerciseTags>();
            //if ((bool)value)
            //{
            //    tags.Add(tagChecked);
            //}

            //return tags;
        }
    }


    public class ExerciseTagExtension
    {
        /// <summary>
        /// Convert from base to value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ConvertFromStringToInt(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            var tagsString = value.Split(',');
            List<ExerciseTags> enumLists = new List<ExerciseTags>();
            foreach (var tagStr in tagsString)
            {
                ExerciseTags res = (ExerciseTags)Enum.Parse(typeof(ExerciseTags), tagStr);
                enumLists.Add(res);
            }

            return ConvertListToInt(enumLists);
        }

        /// <summary>
        /// Convert from value to model
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<ExerciseTags> ConvertFromIntToList(int value)
        {
            if (value == 0)
            {
                return new List<ExerciseTags>();
            }
            List<ExerciseTags> result = new List<ExerciseTags>();

            BitArray array = new BitArray(new [] {value});
            for (int i = 0; i < (int)ExerciseTags.Last; i++)
            {
                var flagValue = array.Get(i);
                if (flagValue)
                {
                    result.Add((ExerciseTags)i);
                }
            }

            return result;
        }

        public static int ConvertListToInt(List<ExerciseTags> tagsList)
        {
            if (tagsList.Count == 0)
            {
                return 0;
            }
            BitArray array = new BitArray(32);
            int[] res = new int[1];
            foreach (var exerciseTags in tagsList)
            {
                array.Set((int)exerciseTags, true);
            }

            array.CopyTo(res,0);
            return res[0];
        }

        public static string ConvertJson(List<ExerciseTags> tagsList, TrainingExerciseViewModel viewmodel)
        {
            string weightAndReps = "";
            if (tagsList.Contains(ExerciseTags.ExerciseByRepsAndWeight))
            {
                weightAndReps = JsonConvert.SerializeObject(viewmodel.WeightAndRepsItems);
            }
            if (tagsList.Contains(ExerciseTags.ExerciseByTime) || tagsList.Contains(ExerciseTags.ExerciseByDistance))
            {
                weightAndReps = JsonConvert.SerializeObject((viewmodel.Time, viewmodel.Distance));
            }

            return weightAndReps;
        }

        public static void ConvertJsonBack(TrainingExerciseViewModel viewmodel,string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            List<ExerciseTags> tagsList = viewmodel.Tags;

            if (tagsList.Contains(ExerciseTags.ExerciseByRepsAndWeight))
            {
                viewmodel.WeightAndRepsItems = JsonConvert.DeserializeObject<ObservableCollection<WeightAndReps>>(value);
            }
            if (tagsList.Contains(ExerciseTags.ExerciseByTime) || tagsList.Contains(ExerciseTags.ExerciseByDistance))
            {
                var obj = JsonConvert.DeserializeObject<(TimeSpan, double)>(value);
                viewmodel.Distance = obj.Item2;
                viewmodel.Time = obj.Item1;
            }
        }

    }
}
