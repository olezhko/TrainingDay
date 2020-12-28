using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Microcharts;
using Microsoft.AppCenter.Crashes;
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

        public void OnAppearing()
        {
            PrepareBodyControlItems();
        }

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

        public ICommand SaveGoalWeightCommand { get; set; }
        private void SaveGoalWeight()
        {
            Settings.WeightGoal = GoalWeight;
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

        public ICommand SaveGoalWaistCommand => new Command(SaveGoalWaist);
        private void SaveGoalWaist()
        {
            Settings.WaistGoal = GoalWaist;
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

        public ICommand SaveGoalHipsCommand => new Command(SaveGoalHips);
        private void SaveGoalHips()
        {
            Settings.HipGoal = GoalHips;
            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
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
        }

        #region New Logic

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

        public ObservableCollection<BodyControlItem> BodyControlItems { get; set; }= new ObservableCollection<BodyControlItem>();
        private void PrepareBodyControlItems()
        {
            IsBusy = true;
            BodyControlItems.Clear();
            //var bodyControlItems = App.Database.GetWeightNotesItems();
            List<WeightNote> bodyControlItems = GenerateData();

            int countDaysPeriod = GetDaysByPeriod((ChartWeightPeriod)WeightChartPeriod);
            var startDate = DateTime.Now.AddDays(-20);

            var weightItems = PrepareChartData(bodyControlItems,startDate,WeightType.Weight);
            if (weightItems.Any())
            {
                CurrentWeightValue = weightItems.Last().Weight;
            }

            var waistItems = PrepareChartData(bodyControlItems, startDate, WeightType.Waist);
            if (waistItems.Any())
            {
                CurrentWaistValue = waistItems.Last().Weight;
            }

            var hipsItems = PrepareChartData(bodyControlItems, startDate, WeightType.Hip);
            if (hipsItems.Any())
            {
                CurrentHipsValue = hipsItems.Last().Weight;
            }

            BodyControlItems.Add(new BodyControlItem()
            {
                Name = Resource.WeightString,
                GoalValue = Settings.WeightGoal,
                CurrentValue = CurrentWeightValue,
                Chart = PrepareChart(Settings.WeightGoal, weightItems)
            });


            BodyControlItems.Add(new BodyControlItem()
            {
                Name = Resource.WaistString,
                GoalValue = Settings.WaistGoal,
                CurrentValue = CurrentWaistValue,
                Chart = PrepareChart(Settings.WaistGoal, waistItems)
            });


            BodyControlItems.Add(new BodyControlItem()
            {
                Name = Resource.HipsString,
                GoalValue = Settings.HipGoal,
                CurrentValue = CurrentHipsValue,
                Chart = PrepareChart(Settings.HipGoal, hipsItems)
            });

            IsBusy = false;
        }

        private List<WeightNote> GenerateData()
        {
            var bodyControlItems = new List<WeightNote>();
            bodyControlItems.Add(new WeightNote()
            {
                Date = new DateTime(2020, 12, 20),
                Weight = 86,
                Type = (int)WeightType.Weight
            });
            bodyControlItems.Add(new WeightNote()
            {
                Date = new DateTime(2020, 12, 25),
                Weight = 80,
                Type = (int)WeightType.Weight
            });

            bodyControlItems.Add(new WeightNote()
            {
                Date = new DateTime(2020, 12, 10),
                Weight = 81,
                Type = (int)WeightType.Waist
            });

            bodyControlItems.Add(new WeightNote()
            {
                Date = new DateTime(2020, 12, 21),
                Weight = 86,
                Type = (int)WeightType.Waist
            });
            bodyControlItems.Add(new WeightNote()
            {
                Date = new DateTime(2020, 12, 01),
                Weight = 86,
                Type = (int)WeightType.Hip
            });

            bodyControlItems.Add(new WeightNote()
            {
                Date = new DateTime(2020, 12, 23),
                Weight = 81,
                Type = (int)WeightType.Hip
            });

            return bodyControlItems;
        }

        private LineChart PrepareChart(double goal, IEnumerable<WeightNote> items)
        {
            var start = items.First().Date;
            var end = items.Last().Date;
            var goalEntries = new List<ChartEntry>();
            goalEntries.Add(new ChartEntry((float)goal)
            {
                ValueLabel = goal.ToString(),
                Label = start.ToLongDateString(),
                ValueLabelColor = SKColors.Gold,
            });
            goalEntries.Add(new ChartEntry((float)goal)
            {
                ValueLabel = goal.ToString(),
                Label = end.ToLongDateString(),
                ValueLabelColor = SKColors.Gold,
            });


            var entries = items.Select(item => new ChartEntry((float)item.Weight)
            {
                ValueLabel = item.Weight.ToString(),
                Label = item.Date.ToLongDateString(),
                ValueLabelColor = Settings.IsLightTheme ? SKColors.Black : SKColors.White,
            }).ToList();

            var minValue = items.Select(item => item.Weight).Min() - 1;
            return new LineChart
            {
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
                },

                YAxisLinesPaint = new SKPaint
                {
                Color = Settings.IsLightTheme ? SKColors.Black : SKColors.White,
                    IsAntialias = true,
                Style = SKPaintStyle.Stroke
            },
            LabelColor = Settings.IsLightTheme?SKColors.Black : SKColors.White,
                Series = new List<ChartSerie>()
            {
                new ChartSerie()
                {
                    Color = SKColors.Green,
                    Entries = entries
                },
                //new ChartSerie()
                //{
                //    Color = SKColor.Parse("#77d065"),
                //    Entries = goalEntries
                //}
            }};
        }

        #endregion
    }

    public class BodyControlItem: BaseViewModel
    {
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

        public LineChart Chart { get; set; }

        public BodyControlItem()
        {
            
        }
    }
}
