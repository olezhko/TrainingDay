using System;
using System.Linq;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using TrainingDay.Code;
using TrainingDay.Model;
using Xamarin.Forms;

namespace TrainingDay.ViewModel
{
    class WeightViewAndSetPageViewModel:BaseViewModel
    {
        public WeightViewAndSetPageViewModel()
        {
            SaveCurrentWeightCommand = new Command(SaveCurrentWeight);
            WeightPeriodChangedCommand = new Command(WeightPeriodChanged);
        }

        public void WeightPeriodChanged()
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
                    countDaysPeriod = (3 *31);
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
                MarkerStroke = OxyColors.Black,
                CanTrackerInterpolatePoints = true
            };
            foreach (var periodWeightItem in periodWeightItems)
            {
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(periodWeightItem.Date), periodWeightItem.Weight));
            }
            PlotView.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom , Minimum = minValue, Maximum = maxValue,
                IntervalType = DateTimeIntervalType.Days, StringFormat = "dd/MM/yyyy", IsPanEnabled = false,IsZoomEnabled = false, });
            PlotView.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Maximum = 200, Minimum = 0, IsPanEnabled = false, IsZoomEnabled = false });

            PlotView.Series.Add(lineSeries);
            OnPropertyChanged(nameof(PlotView));
        }

        private void SaveCurrentWeight()
        {
            WeightNote note = new WeightNote();
            note.Date = DateTime.Now;
            note.Weight = CurrentWeightValue;
            int res = App.Database.SaveWeightNotesItem(note);
            if (res != 1)
            {
                DependencyService.Get<IMessage>().ShortAlert("Не сохранено");
                return;
            }
            DependencyService.Get<IMessage>().ShortAlert("Сохранено");
            WeightPeriodChanged();
        }

        public ICommand WeightPeriodChangedCommand { get; set; }
        public ICommand SaveCurrentWeightCommand { get; set; }

        private double currentWeightValue;
        public double CurrentWeightValue
        {
            get { return currentWeightValue; }
            set
            {
                currentWeightValue = value;
                OnPropertyChanged(nameof(CurrentWeightValue));
            }
        }

        private int weightChartPeriod;
        public int WeightChartPeriod
        {
            get { return weightChartPeriod; }
            set
            {
                weightChartPeriod = value;
                OnPropertyChanged(nameof(WeightChartPeriod));
            }
        }

        private PlotModel _plotModel;

        public PlotModel PlotView
        {
            get { return _plotModel; }
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
    }
}
