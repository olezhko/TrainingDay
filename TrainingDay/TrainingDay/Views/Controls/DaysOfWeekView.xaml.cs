using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TrainingDay.Resources;
using TrainingDay.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DaysOfWeekView : ContentView
    {
        StringBuilder _sb;

        private static string[] days = new[]
        {
            Resource.DayTextMonday, Resource.DayTextThusday, Resource.DayTextWensdey,
            Resource.DayTextThursday, Resource.DayTextFriday, Resource.DayTextSaturday, Resource.DayTextSunday
        };
        public DaysOfWeekView()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext == null) return;
            var alarm = ((AlarmViewModel)BindingContext);
            IsEnabled = alarm.IsActive;

            var daysOfWeek = alarm.Days;
            _sb = new StringBuilder();

            bool isEveryDay = alarm.Days.AllDays.All(X => X == true);

            if (isEveryDay)
            {
                DaysLabel.Text = Resource.DayTextEveryDay;
                return;
            }

            for (int i = 0; i < daysOfWeek.AllDays.Length; i++)
            {
                var hasDay = daysOfWeek.AllDays[i];
                if (hasDay)
                {
                    if (i > 0 && !string.IsNullOrWhiteSpace(_sb.ToString()))
                    {
                        _sb.Append(", ");
                    }
                    _sb.Append(days[i]);
                }
            }

            DaysLabel.Text = _sb.ToString();
            //MondayLabel.IsVisible = daysOfWeek.Monday;
            //TuesdayLabel.IsVisible = daysOfWeek.Tuesday;
            //WednesdayLabel.IsVisible = daysOfWeek.Wednesday;
            //ThursdayLabel.IsVisible = daysOfWeek.Thursday;
            //FridayLabel.IsVisible = daysOfWeek.Friday;
            //SaturdayLabel.IsVisible = daysOfWeek.Saturday;
            //SundayLabel.IsVisible = daysOfWeek.Sunday;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == IsEnabledProperty.PropertyName)
            {
                //SetLabelStyle(IsEnabled);
            }
        }

        void SetLabelStyle(bool state)
        {
            if (state)
                DaysLabel.Style = (Style)Resources["AlarmExtrasHeading"];
            else
                DaysLabel.Style = (Style)Resources["AlarmExtrasDisabledHeading"];
        }
    }
}