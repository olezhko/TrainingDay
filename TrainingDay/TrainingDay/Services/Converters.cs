using System;
using System.Globalization;
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

    
}
