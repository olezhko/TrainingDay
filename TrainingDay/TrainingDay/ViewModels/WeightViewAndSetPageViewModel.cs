using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Microcharts;
using SkiaSharp;
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
        #region Publ Prop
        public ICommand WeightPeriodChangedCommand { get; set; }

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
        #endregion

        public WeightViewAndSetPageViewModel()
        {
            WeightPeriodChangedCommand = new Command(PrepareBodyControlItems);
        }

        public void OnAppearing()
        {
            PrepareBodyControlItems();
        }

        public ICommand SaveGoalValueCommand => new Command<BodyControlItem>(SaveGoalValue);
        private void SaveGoalValue(BodyControlItem sender)
        {
            WeightType type = sender.Type;
            switch (type)
            {
                case WeightType.Weight:
                    Settings.WeightGoal = BodyControlItems.First(item=>item.Type == type).GoalValue;
                    break;
                case WeightType.Waist:
                    Settings.WaistGoal = BodyControlItems.First(item => item.Type == type).GoalValue;
                    break;
                case WeightType.Hip:
                    Settings.HipGoal = BodyControlItems.First(item => item.Type == type).GoalValue;
                    break;
            }
            sender.Chart = PrepareChart(sender.GoalValue, sender.ChartItems);
            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
        }

        public ICommand SaveCurrentValueCommand => new Command<BodyControlItem>(SaveCurrentValue);
        private async void SaveCurrentValue(BodyControlItem sender)
        {
            WeightType type = sender.Type;
            WeightNote note = new WeightNote
            {
                Date = DateTime.Now,
                Weight = sender.CurrentValue,
                Type = (int)type
            };
            App.Database.SaveWeightNotesItem(note);

            sender.ChartItems.Add(note);
            sender.Chart = PrepareChart(sender.GoalValue, sender.ChartItems);

            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
            DependencyService.Get<IMessage>().CancelNotification(App.WeightNotificationId);
            await SiteService.SendBodyControl(Settings.Token);
        }

        private static int GetDaysByPeriod(ChartWeightPeriod period)
        {
            int countDaysPeriod = 0;

            switch (period)
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

            return countDaysPeriod;
        }

        private static List<WeightNote> PrepareChartData(IEnumerable<WeightNote> bodyControlItems, DateTime startDate, WeightType type)
        {
            var periodWeightItems = bodyControlItems.Where(a => a.Date > startDate).Where(a => a.Type == (int)type);
            var chartItems = new List<WeightNote>();
            try
            {
                var lastItem = bodyControlItems.Last(item => item.Date < startDate && item.Type == (int)type);
                chartItems.Add(lastItem);
            }
            catch (Exception e) { }
            chartItems.AddRange(periodWeightItems);

            return chartItems;
        }

        public ObservableCollection<BodyControlItem> BodyControlItems { get; set; } = new ObservableCollection<BodyControlItem>();
        private void PrepareBodyControlItems()
        {
            IsBusy = true;
            double currentWeightValue = 0, currentWaistValue = 0, currentHipsValue = 0;
            BodyControlItems.Clear();
            var bodyControlItems = App.Database.GetWeightNotesItems();

            int countDaysPeriod = GetDaysByPeriod((ChartWeightPeriod)WeightChartPeriod);
            var startDate = DateTime.Now.AddDays(-countDaysPeriod);

            var weightItems = PrepareChartData(bodyControlItems, startDate, WeightType.Weight);
            if (weightItems.Any())
            {
                currentWeightValue = weightItems.Last().Weight;
            }

            var waistItems = PrepareChartData(bodyControlItems, startDate, WeightType.Waist);
            if (waistItems.Any())
            {
                currentWaistValue = waistItems.Last().Weight;
            }

            var hipsItems = PrepareChartData(bodyControlItems, startDate, WeightType.Hip);
            if (hipsItems.Any())
            {
                currentHipsValue = hipsItems.Last().Weight;
            }

            BodyControlItems.Add(new BodyControlItem(weightItems)
            {
                Name = Resource.WeightString,
                GoalValue = Settings.WeightGoal,
                CurrentValue = currentWeightValue,
                Chart = PrepareChart(Settings.WeightGoal, weightItems)
            });

            BodyControlItems.Add(new BodyControlItem(waistItems)
            {
                Name = Resource.WaistString,
                GoalValue = Settings.WaistGoal,
                CurrentValue = currentWaistValue,
                Chart = PrepareChart(Settings.WaistGoal, waistItems)
            });

            BodyControlItems.Add(new BodyControlItem(hipsItems)
            {
                Name = Resource.HipsString,
                GoalValue = Settings.HipGoal,
                CurrentValue = currentHipsValue,
                Chart = PrepareChart(Settings.HipGoal, hipsItems)
            });

            IsBusy = false;
        }

        private LineChart PrepareChart(double goal, IEnumerable<WeightNote> items)
        {
            if (!items.Any())
            {
                return null;
            }
            var start = items.First().Date;
            var end = items.Last().Date;
            var goalEntries = new List<ChartEntry>();
            goalEntries.Add(new ChartEntry((float)goal)
            {
                ValueLabel = goal.ToString(),
                Label = start.ToShortDateString(),
                ValueLabelColor = SKColors.Gold,
            });
            goalEntries.Add(new ChartEntry((float)goal)
            {
                ValueLabel = goal.ToString(),
                Label = end.ToShortDateString(),
                ValueLabelColor = SKColors.Gold,
            });

            var dictDate = items.GroupBy(k => k.Date.Date)
                .OrderByDescending(k => k.Key)
                .ToDictionary(k => k.Key, v => v.OrderByDescending(x => x.Date).Last());
            var entries = dictDate.Select(item => new ChartEntry((float)item.Value.Weight)
            {
                ValueLabel = item.Value.Weight.ToString(),
                Label = item.Key.ToLongDateString(),
                ValueLabelColor = Settings.IsLightTheme ? SKColors.Black : SKColors.White,
            }).ToList();

            var minValue = items.Select(item => item.Weight).Min() - 1;
            return new LineChart
            {
                Margin = 30,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelTextSize = 42,
                ValueLabelTextSize = 42,
                SerieLabelTextSize = 42,
                LineMode = LineMode.Straight,
                ShowYAxisLines = true,
                ShowYAxisText = true,
                MinValue = (float)minValue,
                BackgroundColor = SKColors.Transparent,
                YAxisPosition = Position.Left,
                YAxisTextPaint = new SKPaint
                {
                    Color = Settings.IsLightTheme ? SKColors.Black : SKColors.White,
                    IsAntialias = true,
                    Style = SKPaintStyle.StrokeAndFill,
                    TextSize = 42
                },

                YAxisLinesPaint = new SKPaint
                {
                    Color = Settings.IsLightTheme ? SKColors.Black : SKColors.White,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke
                },
                LabelColor = Settings.IsLightTheme ? SKColors.Black : SKColors.White,
                Series = new List<ChartSerie>()
                {
                    new ChartSerie()
                    {
                        Color = SKColors.Green,
                        Entries = entries
                    },
                    new ChartSerie()
                    {
                        Color = SKColors.Gold,
                        Entries = goalEntries
                    }
                }
            };
        }



        public ObservableCollection<WeightNote> WeightNotes { get; set; } = new ObservableCollection<WeightNote>();
        public ICommand DeleteWeightNoteCommand => new Command<WeightNote>(DeleteWeightNote);
        private void DeleteWeightNote(WeightNote sender)
        {
            App.Database.DeleteWeightNote(sender.Id);
            WeightNotes.Remove(sender);
            DependencyService.Get<IMessage>().ShortAlert(Resource.DeletedString);
        }
    }

    public class BodyControlItem: BaseViewModel
    {
        public string GoalValueString
        {
            get => GoalValue.ToString(CultureInfo.InvariantCulture);
            set
            {
                var res = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var weight);
                if (res)
                {
                    GoalValue = weight;
                }
            }
        }

        public string CurrentValueString
        {
            get => CurrentValue.ToString(CultureInfo.InvariantCulture);
            set
            {
                var res = double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var weight);
                if (res)
                {
                    CurrentValue = weight;
                }
            }
        }

        private WeightType type;
        public WeightType Type
        {
            get => type;
            set
            {
                type = value;
                OnPropertyChanged();
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        private double _currentValue;
        public double CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnPropertyChanged();
            } }

        private double goalValue;
        public double GoalValue
        {
            get => goalValue;
            set
            {
                goalValue = value;
                OnPropertyChanged();
            }
        }

        private LineChart chart;
        public LineChart Chart
        {
            get => chart;
            set
            {
                chart = value;
                OnPropertyChanged();
            }}

        public List<WeightNote> ChartItems { get; set; }

        public BodyControlItem(List<WeightNote> chartItems)
        {
            ChartItems = new List<WeightNote>(chartItems);
        }
    }
}
