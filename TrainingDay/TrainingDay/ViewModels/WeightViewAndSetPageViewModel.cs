using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Plugin.LocalNotifications;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.View;
using TrainingDay.Views.Controls;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    class WeightViewAndSetPageViewModel : BaseViewModel
    {
        public WeightViewAndSetPageViewModel()
        {
            GoalWeight = Settings.WeightGoal;
            SaveCurrentWeightCommand = new Command(SaveCurrentWeight);
            SaveGoalWeightCommand = new Command(SaveGoalWeight);
            WeightPeriodChangedCommand = new Command(WeightPeriodChanged);
            var weightItems = App.Database.GetWeightNotesItems();
            if (weightItems != null && weightItems.Any())
                CurrentWeightValue = weightItems.Last().Weight;
        }

        public ICommand WeightPeriodChangedCommand { get; set; }
        public void WeightPeriodChanged()
        {
            try
            {
                PlotView = new PlotModel();

                int countDaysPeriod = 0;
                switch ((ChartWeightPeriod)WeightChartPeriod)
                {
                    case ChartWeightPeriod.Week:
                        countDaysPeriod = 7;
                        break;
                    case ChartWeightPeriod.TwoWeeks:
                        countDaysPeriod = (14);
                        break;
                    case ChartWeightPeriod.Mounth:
                        countDaysPeriod = (31);
                        break;
                    case ChartWeightPeriod.TwoMounth:
                        countDaysPeriod = (62);
                        break;
                    case ChartWeightPeriod.ThreeMounth:
                        countDaysPeriod = (3 * 31);
                        break;
                    case ChartWeightPeriod.HalfYear:
                        countDaysPeriod = (6 * 31);
                        break;
                    case ChartWeightPeriod.Year:
                        countDaysPeriod = (365);
                        break;
                }
                var startDate = DateTime.Now.AddDays(-countDaysPeriod);
                var endDate = DateTime.Now;

                var minValue = DateTimeAxis.ToDouble(startDate);
                var maxValue = DateTimeAxis.ToDouble(endDate);

                var weightItems = App.Database.GetWeightNotesItems();
                var periodWeightItems = weightItems.Where(a => a.Date > startDate);

                var lineSeries = new LineSeries
                {
                    StrokeThickness = 1,
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 4,
                    MarkerStroke = Settings.IsLightTheme? OxyColors.Black : OxyColors.White,
                    CanTrackerInterpolatePoints = true
                };
                var weightNotes = periodWeightItems as WeightNote[] ?? periodWeightItems.ToArray();
                foreach (var periodWeightItem in weightNotes)
                {
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(periodWeightItem.Date), periodWeightItem.Weight));
                }
                PlotView.Axes.Add(new DateTimeAxis
                {
                    Position = AxisPosition.Bottom,
                    Minimum = minValue,
                    Maximum = maxValue,
                    IntervalType = DateTimeIntervalType.Days,
                    StringFormat = "dd/MM/yyyy",
                    IsPanEnabled = false,
                    IsZoomEnabled = false,
                    AxislineStyle = LineStyle.Solid,
                    AxislineColor = Settings.IsLightTheme ? OxyColors.Black : OxyColors.White,
                    TicklineColor = Settings.IsLightTheme ? OxyColors.Black : OxyColors.White,
                    TextColor = Settings.IsLightTheme ? OxyColors.Black : OxyColors.White
                });

                var max = weightNotes.Length == 0 ? 100 : weightNotes.Max(a => a.Weight) + 1;
                var min = weightNotes.Length == 0 ? 0 : weightNotes.Min(a => a.Weight) - 1;
                min = Math.Min(min, GoalWeight - 1);
                max = Math.Max(max, GoalWeight + 1);


                PlotView.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Maximum = max,
                    Minimum = min,
                    IsPanEnabled = false,
                    IsZoomEnabled = false,
                    AxislineStyle = LineStyle.Solid,
                    AxislineColor = Settings.IsLightTheme ? OxyColors.Black : OxyColors.White,
                    TicklineColor = Settings.IsLightTheme ? OxyColors.Black : OxyColors.White,
                    TextColor = Settings.IsLightTheme ? OxyColors.Black : OxyColors.White
                });

                PlotView.PlotAreaBorderColor = OxyColors.Transparent;
                PlotView.Series.Add(lineSeries);

                goalSeries = null;
                FillGoalLine(minValue, maxValue);

                OnPropertyChanged(nameof(PlotView));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void FillGoalLine(double startDate, double endDate)
        {
            try
            {
                if (goalSeries == null)
                {
                    goalSeries = new LineSeries();
                }
                goalSeries.Points.Clear();
                goalSeries.Points.Add(new DataPoint(startDate, GoalWeight));
                goalSeries.Points.Add(new DataPoint(endDate, GoalWeight));
                if (PlotView.Series.Count == 1)
                {
                    PlotView.Series.Add(goalSeries);
                }
                OnPropertyChanged(nameof(PlotView));
            }
            catch (Exception ex)
            {
                WeightPeriodChanged();
            }
        }

        private double _currentWeightValue;
        public double CurrentWeightValue
        {
            get => _currentWeightValue;
            set
            {
                _currentWeightValue = value;
                OnPropertyChanged(nameof(CurrentWeightValue));
            }
        }

        private int _weightChartPeriod = 0;
        public int WeightChartPeriod
        {
            get => _weightChartPeriod;
            set
            {
                _weightChartPeriod = value;
                OnPropertyChanged(nameof(WeightChartPeriod));
            }
        }

        private PlotModel _plotModel;
        public PlotModel PlotView
        {
            get => _plotModel;
            set
            {
                _plotModel = value;
                OnPropertyChanged();
            }
        }
        enum ChartWeightPeriod
        {
            Week,
            TwoWeeks,
            Mounth,
            TwoMounth,
            ThreeMounth,
            HalfYear,
            Year
        }

        public double GoalWeight { get; set; }
        public ICommand SaveGoalWeightCommand { get; set; }
        private LineSeries goalSeries;
        private void SaveGoalWeight()
        {
            Settings.WeightGoal = GoalWeight;
            FillGoalLine(goalSeries.Points.First().X, goalSeries.Points.Last().X);
            WeightPeriodChanged();
            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
        }

        public ICommand SaveCurrentWeightCommand { get; set; }
        private void SaveCurrentWeight()
        {
            WeightNote note = new WeightNote
            {
                Date = DateTime.Now,
                Weight = CurrentWeightValue
            };
            int res = App.Database.SaveWeightNotesItem(note);
            if (res != 1)
            {
                DependencyService.Get<IMessage>().ShortAlert(Resource.NotSavedString);
                return;
            }
            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
            WeightPeriodChanged();
            CrossLocalNotifications.Current.Cancel(App.WeightNotificationId);
        }
    }
}
