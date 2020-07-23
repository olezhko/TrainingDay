using System;
using System.Collections.ObjectModel;
using System.Globalization;
using TrainingDay.Resources;
using TrainingDay.ViewModels;
using Xamarin.Forms;

namespace TrainingDay.Services
{
    public class CountToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int)) return false;

            var count = (int)value;
            return count > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return false;

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringIsNotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            string str = value.ToString();
            return !string.IsNullOrEmpty(str);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("IsNullConverter can only be used OneWay.");
        }
    }

    public class SuperSetMenuConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SuperSetViewModel items)
            {
                bool res = items.Count == 1;
                return !res;
            }
            else
            {
                if (value is TrainingExerciseViewModel item)
                {
                    bool res = item.SuperSetId == 0;
                    return !res;
                }

                if (value is int superSetId)
                {
                    bool res = superSetId == 0;
                    return !res;
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("IsNullConverter can only be used OneWay.");
        }
    }

    public class NumberSuperSetInTrainingConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var training = value as TrainingViewModel;
            var exercise = parameter as TrainingExerciseViewModel;

            int result = 1;
            foreach (var item in training.Exercises)
            {
                if (item.SuperSetId != 0 )
                {
                    if (item.SuperSetId == exercise.SuperSetId)
                    {
                        return result;
                    }
                    else
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TrainingGroupNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }
            var groupId = (int)value;
            return groupId == 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
