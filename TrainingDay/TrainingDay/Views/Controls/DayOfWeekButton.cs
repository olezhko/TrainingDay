using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace TrainingDay.Controls
{
    public class DaysOfWeek: INotifyPropertyChanged
    {
        public static DaysOfWeek Parse(int alarmItemDays)
        {
            DaysOfWeek value = new DaysOfWeek();
            BitArray days = new BitArray(new [] {alarmItemDays});
            value.Monday = days.Get(0);
            value.Tuesday = days.Get(1);
            value.Wednesday = days.Get(2);
            value.Thursday = days.Get(3);
            value.Friday = days.Get(4);
            value.Saturday = days.Get(5);
            value.Sunday = days.Get(6);
            return value;
        }
        public int Value
        {
            get
            {
               return (int)(Math.Pow(2, 0) * (Monday ? 1 : 0) + 
                            Math.Pow(2, 1) * (Tuesday ? 1 : 0) +
                            Math.Pow(2, 2) * (Wednesday ? 1 : 0) +
                            Math.Pow(2, 3) * (Thursday ? 1 : 0) +
                            Math.Pow(2, 4) * (Friday ? 1 : 0) +
                            Math.Pow(2, 5) * (Saturday ? 1 : 0) +
                            Math.Pow(2, 6) * (Sunday ? 1 : 0));
            }
        }

        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public bool[] AllDays => new bool[] { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday };

        public DaysOfWeek() { }

        public DaysOfWeek(bool[] allDays)
        {
            if (allDays.Length != 7) return;

            Monday = allDays[0];
            Tuesday = allDays[1];
            Wednesday = allDays[2];
            Thursday = allDays[3];
            Friday = allDays[4];
            Saturday = allDays[5];
            Sunday = allDays[6];
        }

        public DaysOfWeek(bool monday, bool tuesday, bool wednesday, bool thursday, bool friday, bool saturday, bool sunday) : this(new bool[] { monday, tuesday, wednesday, thursday, friday, saturday, sunday })
        {
        }


        public static bool GetHasADayBeenSelected(DaysOfWeek days)
        {
            if (days == null) return false;
            return days.AllDays.Contains(true);
        }

        //public override bool Equals(object obj)
        //{
        //    if (obj is DayOfWeek)
        //    {
        //        //cast enum to int (sunday = 0, Saturday = 6)
        //        var dayOfWeek = (int)obj;
        //        if (dayOfWeek == 0)
        //        {
        //            if (Sunday)
        //                return true;
        //            else
        //                return false;
        //        }
        //        else
        //        {
        //            var day = AllDays[dayOfWeek - 1];
        //            if (day)
        //                return true;
        //            else
        //                return false;
        //        }
        //    }

        //    if (obj is DaysOfWeek)
        //    {
        //        var daysOfWeek = (DaysOfWeek)obj;
        //        if (this.AllDays == daysOfWeek.AllDays)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }

        //    return false;
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public bool Contains(int curDay)
        {
            if (curDay < 0)
            {
                return false;
            }
            return AllDays[curDay];
        }

       
    }

    public static class DayOfWeekExtension
    {
        public static int ConvertToSimple(this DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Friday:
                    return 4;
                case DayOfWeek.Monday:
                    return 0;
                case DayOfWeek.Saturday:
                    return 5;
                case DayOfWeek.Sunday:
                    return 6;
                case DayOfWeek.Thursday:
                    return 3;
                case DayOfWeek.Tuesday:
                    return 1;
                case DayOfWeek.Wednesday:
                    return 2;
            }

            return 0;
        }
    }

    public class EntryWithDot : Entry
    {

    }

    public class CustomPicker : Picker
    {
        public static readonly BindableProperty HintProperty = BindableProperty.Create("Hint", typeof(string), typeof(CustomPicker), null);

        public string Hint
        {
            get { return (string)GetValue(HintProperty); }
            set { SetValue(HintProperty, value); }
        }

        public static readonly BindableProperty IsValidProperty = BindableProperty.Create("IsValid", typeof(bool?), typeof(CustomPicker), null, propertyChanged: OnIsValidChanged);

        public bool? IsValid
        {
            get { return (bool?)GetValue(IsValidProperty); }
            set { SetValue(IsValidProperty, value); }
        }

        public event EventHandler IsValidChanged;

        static void OnIsValidChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // Property changed implementation goes here
            var picker = (CustomPicker)bindable;
            picker.IsValidChanged?.Invoke(picker, null);
        }
    }

    public class DayOfWeekButton : Button
    {
        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create("IsSelected", typeof(bool), typeof(DayOfWeekButton), false, BindingMode.TwoWay,propertyChanged: IsSelectedPropertyChanged);

        private static void IsSelectedPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = (DayOfWeekButton)bindable;
            picker.BackgroundColor = ((bool) newvalue) ? Color.White : Color.DimGray;
        }

        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }
    }

    public class FullScreenVideoWebView : WebView
    {
        /// <summary>
        /// Bindable property for <see cref="EnterFullScreenCommand"/>.
        /// </summary>
        public static readonly BindableProperty EnterFullScreenCommandProperty =
            BindableProperty.Create(
                nameof(EnterFullScreenCommand),
                typeof(ICommand),
                typeof(FullScreenVideoWebView),
                defaultValue: new Command(async (view) => await DefaultEnterAsync((Xamarin.Forms.View)view)));

        /// <summary>
        /// Bindable property for <see cref="ExitFullScreenCommand"/>.
        /// </summary>
        public static readonly BindableProperty ExitFullScreenCommandProperty =
            BindableProperty.Create(
                nameof(ExitFullScreenCommand),
                typeof(ICommand),
                typeof(FullScreenVideoWebView),
                defaultValue: new Command(async (view) => await DefaultExitAsync()));

        /// <summary>
        /// Gets or sets the command executed when the web view content requests entering full-screen.
        /// The command is passed a <see cref="View"/> containing the content to display.
        /// The default command displays the content as a modal page.
        /// </summary>
        public ICommand EnterFullScreenCommand
        {
            get => (ICommand)GetValue(EnterFullScreenCommandProperty);
            set => SetValue(EnterFullScreenCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the command executed when the web view content requests exiting full-screen.
        /// The command is passed no parameters.
        /// The default command pops a modal page off the navigation stack.
        /// </summary>
        public ICommand ExitFullScreenCommand
        {
            get => (ICommand)GetValue(ExitFullScreenCommandProperty);
            set => SetValue(ExitFullScreenCommandProperty, value);
        }

        private static async Task DefaultEnterAsync(Xamarin.Forms.View view)
        {
            var page = new ContentPage
            {
                Content = view
            };

            await Application.Current.MainPage.Navigation.PushModalAsync(page);
        }

        private static async Task DefaultExitAsync()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}