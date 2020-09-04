using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TrainingDay.Views.Controls
{
    public class StateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            States current = (States)value;
            States checkedState = (States) parameter;
            return current == checkedState;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public enum States
    {
        Loading,
        Normal,
        Error,
        NoInternet,
        NoData,
        View1,
        View2
    }
}
