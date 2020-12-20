using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Microsoft.AppCenter.Crashes;
using TrainingDay.Resources;
using TrainingDay.Services;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    public enum WeightType
    {
        Weight,
        Waist,
        Hip
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

    class WeightViewAndSetPageViewModel : BaseViewModel
    {
        public WeightViewAndSetPageViewModel()
        {
            GoalWeight = Settings.WeightGoal;
            GoalWaist = Settings.WaistGoal;
            GoalHips = Settings.HipGoal;
            SaveCurrentWeightCommand = new Command(SaveCurrentWeight);
            SaveGoalWeightCommand = new Command(SaveGoalWeight);
            WeightPeriodChangedCommand = new Command(LoadWeightPlot);
            var items = App.Database.GetWeightNotesItems();

            if (items != null && items.Any())
            {
                var weightItems = items.Where(a => a.Type == (int) WeightType.Weight);
                if (weightItems.Any())
                {
                    CurrentWeightValue = weightItems.Last().Weight;
                }

                var waistItems = items.Where(a => a.Type == (int)WeightType.Waist);
                if (waistItems.Any())
                {
                    CurrentWaistValue = waistItems.Last().Weight;
                }

                var hipsItems = items.Where(a => a.Type == (int)WeightType.Hip);
                if (hipsItems.Any())
                {
                    CurrentHipsValue = hipsItems.Last().Weight;
                }
            }
        }

        public ICommand WeightPeriodChangedCommand { get; set; }
        public void LoadWeightPlot()
        {
            try
            {
                PlotViewWeight = new PlotModel();

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
                var periodWeightItems = weightItems.Where(a => a.Date > startDate).Where(a => a.Type == (int)WeightType.Weight);
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
                    LineStyle = LineStyle.Solid,
                    StrokeThickness = 1,
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 2,
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

                PlotViewWeight.Axes.Add(new DateTimeAxis
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
                    TextColor = Settings.IsLightTheme ? OxyColors.Black : OxyColors.White,
                });

                var max = weightNotes.Length == 0 ? 100 : weightNotes.Max(a => a.Weight) + 1;
                var min = weightNotes.Length == 0 ? 0 : weightNotes.Min(a => a.Weight) - 1;
                min = Math.Min(min, GoalWeight - 1);
                max = Math.Max(max, GoalWeight + 1);


                PlotViewWeight.Axes.Add(new LinearAxis
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

                PlotViewWeight.PlotAreaBorderColor = OxyColors.Transparent;
                PlotViewWeight.Series.Add(lineSeries);
                goalSeriesWeight = null;
                FillWeightGoalLine(minValue, maxValue);

                OnPropertyChanged(nameof(PlotViewWeight));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                // ignored
            }
        }

        private void FillWeightGoalLine(double startDate, double endDate)
        {
            try
            {
                if (goalSeriesWeight == null)
                {
                    goalSeriesWeight = new LineSeries()
                    {
                        Color = OxyColors.Orange
                    };
                }
                goalSeriesWeight.Points.Clear();
                goalSeriesWeight.Points.Add(new DataPoint(startDate, GoalWeight));
                goalSeriesWeight.Points.Add(new DataPoint(endDate, GoalWeight));
                if (PlotViewWeight.Series.Count == 1)
                {
                    PlotViewWeight.Series.Add(goalSeriesWeight);
                }
                OnPropertyChanged(nameof(PlotViewWeight));
            }
            catch (Exception ex)
            {
                LoadWeightPlot();
                Crashes.TrackError(ex);
            }
        }


        public void LoadWaistPlot()
        {
            try
            {
                PlotViewWaist = new PlotModel();

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
                var periodWeightItems = weightItems.Where(a => a.Date > startDate).Where(a => a.Type == (int)WeightType.Waist);
                OnPropertyChanged(nameof(WeightNotes));
                var chartItems = new List<WeightNote>();
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
                    LineStyle = LineStyle.Solid,
                    StrokeThickness = 1,
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 2,
                    MarkerStroke = Settings.IsLightTheme ? OxyColors.Black : OxyColors.White,
                    CanTrackerInterpolatePoints = true,
                    Color = OxyColors.Green,
                    TrackerFormatString = "X={2},\nY={4}",
                };

                WeightNote[] weightNotes = chartItems.ToArray();
                foreach (var periodWeightItem in weightNotes)
                {
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(periodWeightItem.Date), periodWeightItem.Weight));
                }

                PlotViewWaist.Axes.Add(new DateTimeAxis
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
                min = Math.Min(min, GoalWaist - 1);
                max = Math.Max(max, GoalWaist + 1);


                PlotViewWaist.Axes.Add(new LinearAxis
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

                PlotViewWaist.PlotAreaBorderColor = OxyColors.Transparent;
                PlotViewWaist.Series.Add(lineSeries);
                goalWaistSeries = null;
                FillWaistGoalLine(minValue, maxValue);

                OnPropertyChanged(nameof(PlotViewWaist));
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);// ignored
            }
        }

        private void FillWaistGoalLine(double startDate, double endDate)
        {
            try
            {
                if (goalWaistSeries == null)
                {
                    goalWaistSeries = new LineSeries()
                    {
                        Color = OxyColors.Orange
                    };
                }
                goalWaistSeries.Points.Clear();
                goalWaistSeries.Points.Add(new DataPoint(startDate, GoalWaist));
                goalWaistSeries.Points.Add(new DataPoint(endDate, GoalWaist));
                if (PlotViewWaist.Series.Count == 1)
                {
                    PlotViewWaist.Series.Add(goalWaistSeries);
                }
                OnPropertyChanged(nameof(PlotViewWaist));
            }
            catch (Exception ex)
            {
                LoadWaistPlot();
                Crashes.TrackError(ex);
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


        public int MaxLengthWeightField { get; set; }

        #region Weight
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

        private PlotModel _plotModelWeight;
        public PlotModel PlotViewWeight
        {
            get => _plotModelWeight;
            set
            {
                _plotModelWeight = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveGoalWeightCommand { get; set; }
        private LineSeries goalSeriesWeight;
        private void SaveGoalWeight()
        {
            Settings.WeightGoal = GoalWeight;
            FillWeightGoalLine(goalSeriesWeight.Points.First().X, goalSeriesWeight.Points.Last().X);
            LoadWeightPlot();
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
            LoadWeightPlot();

            DependencyService.Get<IMessage>().CancelNotification(App.WeightNotificationId);
        }
        #endregion

        #region Waist
        private double _currentWaistValue;
        public double CurrentWaistValue
        {
            get => _currentWaistValue;
            set
            {
                _currentWaistValue = value;
                OnPropertyChanged(nameof(CurrentWaistValue));
            }
        }

        private double _currentGoalWaist;
        public double GoalWaist
        {
            get => _currentGoalWaist;
            set
            {
                _currentGoalWaist = value;
                OnPropertyChanged();
            }
        }

        public string GoalWaistString
        {
            get => GoalWaist.ToString(CultureInfo.InvariantCulture);
            set
            {
                var res = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var weight);
                if (res)
                {
                    GoalWaist = weight;
                }
            }
        }
        public string CurrentWaistString
        {
            get => CurrentWaistValue.ToString(CultureInfo.InvariantCulture);
            set
            {
                var res = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var weight);
                if (res)
                {
                    CurrentWaistValue = weight;
                }
            }
        }

        private PlotModel _plotModelWaist;
        public PlotModel PlotViewWaist
        {
            get => _plotModelWaist;
            set
            {
                _plotModelWaist = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveGoalWaistCommand => new Command(SaveGoalWaist);
        private LineSeries goalWaistSeries;
        private void SaveGoalWaist()
        {
            Settings.WaistGoal = GoalWaist;
            FillWaistGoalLine(goalWaistSeries.Points.First().X, goalWaistSeries.Points.Last().X);
            LoadWaistPlot();
            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
        }

        public ICommand SaveCurrentWaistCommand => new Command(SaveCurrentWaist);
        private void SaveCurrentWaist()
        {
            WeightNote note = new WeightNote
            {
                Date = DateTime.Now,
                Weight = CurrentWaistValue,
                Type = (int)WeightType.Waist
            };
            App.Database.SaveWeightNotesItem(note);
            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
            LoadWaistPlot();

            DependencyService.Get<IMessage>().CancelNotification(App.WeightNotificationId);
        }
        #endregion

        #region Hips
        private double _currentHipsValue;
        public double CurrentHipsValue
        {
            get => _currentHipsValue;
            set
            {
                _currentHipsValue = value;
                OnPropertyChanged(nameof(CurrentHipsValue));
            }
        }

        private double _currentGoalHips;
        public double GoalHips
        {
            get => _currentGoalHips;
            set
            {
                _currentGoalHips = value;
                OnPropertyChanged();
            }
        }

        public string GoalHipsString
        {
            get => GoalHips.ToString(CultureInfo.InvariantCulture);
            set
            {
                var res = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var weight);
                if (res)
                {
                    GoalHips = weight;
                }
            }
        }
        
        public string CurrentHipsString
        {
            get => CurrentHipsValue.ToString(CultureInfo.InvariantCulture);
            set
            {
                var res = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var weight);
                if (res)
                {
                    CurrentHipsValue = weight;
                }
            }
        }

        private PlotModel _plotModelHips;
        public PlotModel PlotViewHips
        {
            get => _plotModelHips;
            set
            {
                _plotModelHips = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveGoalHipsCommand => new Command(SaveGoalHips);
        private LineSeries goalHipsSeries;
        private void SaveGoalHips()
        {
            Settings.HipGoal = GoalHips;
            FillHipsGoalLine(goalHipsSeries.Points.First().X, goalHipsSeries.Points.Last().X);
            LoadHipsPlot();
            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
        }

        private void FillHipsGoalLine(double startDate, double endDate)
        {
            try
            {
                if (goalHipsSeries == null)
                {
                    goalHipsSeries = new LineSeries()
                    {
                        Color = OxyColors.Orange
                    };
                }
                goalHipsSeries.Points.Clear();
                goalHipsSeries.Points.Add(new DataPoint(startDate, GoalHips));
                goalHipsSeries.Points.Add(new DataPoint(endDate, GoalHips));
                if (PlotViewHips.Series.Count == 1)
                {
                    PlotViewHips.Series.Add(goalHipsSeries);
                }
                OnPropertyChanged(nameof(PlotViewHips));
            }
            catch (Exception ex)
            {
                LoadHipsPlot();
                Crashes.TrackError(ex);
            }
        }

        public void LoadHipsPlot()
        {
            try
            {
                PlotViewHips = new PlotModel();

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
                var periodWeightItems = weightItems.Where(a => a.Date > startDate).Where(a => a.Type == (int)WeightType.Hip);
                OnPropertyChanged(nameof(WeightNotes));
                var chartItems = new List<WeightNote>();
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
                    LineStyle = LineStyle.Solid,
                    StrokeThickness = 1,
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 2,
                    MarkerStroke = Settings.IsLightTheme ? OxyColors.Black : OxyColors.White,
                    CanTrackerInterpolatePoints = true,
                    Color = OxyColors.Green,
                    TrackerFormatString = "X={2},\nY={4}",
                };

                WeightNote[] weightNotes = chartItems.ToArray();
                foreach (var periodWeightItem in weightNotes)
                {
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(periodWeightItem.Date), periodWeightItem.Weight));
                }

                PlotViewHips.Axes.Add(new DateTimeAxis
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
                min = Math.Min(min, GoalHips - 1);
                max = Math.Max(max, GoalHips + 1);

                PlotViewHips.Axes.Add(new LinearAxis
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

                PlotViewHips.PlotAreaBorderColor = OxyColors.Transparent;
                PlotViewHips.Series.Add(lineSeries);
                goalHipsSeries = null;
                FillHipsGoalLine(minValue, maxValue);

                OnPropertyChanged(nameof(PlotViewHips));
            }
            catch (Exception ex)
            {
                // ignored
                Crashes.TrackError(ex);
            }
        }

        public ICommand SaveCurrentHipsCommand => new Command(SaveCurrentHips);
        private void SaveCurrentHips()
        {
            WeightNote note = new WeightNote
            {
                Date = DateTime.Now,
                Weight = CurrentHipsValue,
                Type = (int)WeightType.Hip
            };
            App.Database.SaveWeightNotesItem(note);
            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
            LoadHipsPlot();

            DependencyService.Get<IMessage>().CancelNotification(App.WeightNotificationId);
        }
        #endregion


        public ObservableCollection<WeightNote> WeightNotes { get; set; }= new ObservableCollection<WeightNote>();
        public ICommand DeleteWeightNoteCommand => new Command<WeightNote>(DeleteWeightNote);
        private void DeleteWeightNote(WeightNote sender)
        {
            App.Database.DeleteWeightNote(sender.Id);
            WeightNotes.Remove(sender);
            DependencyService.Get<IMessage>().ShortAlert(Resource.DeletedString);
            LoadWeightPlot();
        }
    }
}
