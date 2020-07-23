using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using TrainingDay.Resources;
using TrainingDay.Services;
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
                WeightNotes = new ObservableCollection<WeightNote>(weightItems.Reverse());
                var periodWeightItems = weightItems.Where(a => a.Date > startDate);
                OnPropertyChanged(nameof(WeightNotes));
                var chartItems = new List<WeightNote>( );
                try
                {
                    var lastItem = weightItems.Last(item => item.Date < startDate);
                    chartItems.Add(lastItem);
                }
                catch (Exception e)
                {
                }

                chartItems.AddRange(periodWeightItems);

                var lineSeries = new LineSeries
                {
                    StrokeThickness = 1,
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 4,
                    MarkerStroke = Settings.IsLightTheme? OxyColors.Black : OxyColors.White,
                    CanTrackerInterpolatePoints = true,
                    Color = OxyColors.Green,
                    TrackerFormatString = "X={2},\nY={4}",
                };

                WeightNote[] weightNotes = chartItems.ToArray();
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
            catch (Exception ex)
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
                    goalSeries = new LineSeries()
                    {
                        Color = OxyColors.Orange
                    };
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
                MaxLengthWeightField = value > 100 ? 3 : 4;
                OnPropertyChanged(nameof(MaxLengthWeightField));
            }
        }

        private double _currentGoalWeight;
        public double GoalWeight
        {
            get =>
                _currentGoalWeight;
            set
            {
                _currentGoalWeight = value;
                OnPropertyChanged();

                MaxLengthWeightField = value > 100 ? 3 : 4;
                OnPropertyChanged(nameof(MaxLengthWeightField));
            }
        }

        public string GoalWeightString
        {
            get => GoalWeight.ToString(CultureInfo.InvariantCulture);
            set
            {
                var res = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var weight);
                if (res)
                {
                    GoalWeight = weight;
                }
            }
        }
        public string CurrentWeightString
        {
            get => CurrentWeightValue.ToString(CultureInfo.InvariantCulture);
            set
            {
                var res = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var weight);
                if (res)
                {
                    CurrentWeightValue = weight;
                }
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
            App.Database.SaveWeightNotesItem(note);
            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
            WeightPeriodChanged();

            DependencyService.Get<IMessage>().CancelNotification(App.WeightNotificationId);
        }

        public ObservableCollection<WeightNote> WeightNotes { get; set; }= new ObservableCollection<WeightNote>();

        public ICommand DeleteWeightNoteCommand => new Command<WeightNote>(DeleteWeightNote);
        private void DeleteWeightNote(WeightNote sender)
        {
            App.Database.DeleteWeightNote(sender.Id);
            WeightNotes.Remove(sender);
            DependencyService.Get<IMessage>().ShortAlert(Resource.DeletedString);
            WeightPeriodChanged();
        }

        public int MaxLengthWeightField { get; set; }
    }
}
