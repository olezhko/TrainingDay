using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TrainingDay.Controls;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.Views;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    // при начале нового дня создавать alarm activity и проверять дни недели
    // сделать для ios тоже самое 
    public class TrainingAlarmListPageViewModel:BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public TrainingAlarmListPageViewModel()
        {
            DeleteAlarmCommand = new Command<ViewCell>(DeleteSelectedAlarm);
        }

        public ObservableCollection<AlarmViewModel> Alarms { get; set; } = new ObservableCollection<AlarmViewModel>();
        public void LoadItems()
        {
            Alarms.Clear();
            var itemsfromBase = App.Database.GetAlarmItems();
            foreach (var alarm in itemsfromBase)
            {
                var newItem = new AlarmViewModel(alarm);
                newItem.PropertyChanged+=NewItemOnPropertyChanged;
                Alarms.Add(newItem);
            }
        }

        private async void NewItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as AlarmViewModel;
            if (e.PropertyName == nameof(AlarmViewModel.IsActive) && item != null)
            {
                await SiteService.UpdateAlarm(item.AlarmItem);
                if (item.IsActive)
                {
                    DependencyService.Get<IAlarmSetter>().SetAlarm(item.AlarmItem);
                }
                else
                {
                    DependencyService.Get<IAlarmSetter>().DeleteAlarm(item.AlarmItem);
                }
            }
        }


        AlarmViewModel _selectedAlarm;
        public AlarmViewModel SelectedAlarm
        {
            get { return _selectedAlarm; }
            set
            {
                _selectedAlarm = value;
                OnPropertyChanged();
            }
        }

        public ICommand NewAlarmCommand
        {
            get
            {
                return new Command(()=>
                {
                    try
                    {
                        MakeTrainingAlarmPageViewModel vm = new MakeTrainingAlarmPageViewModel() { Navigation = Navigation };
                        MakeTrainingAlarmPage page = new MakeTrainingAlarmPage() { BindingContext = vm };
                        Navigation.PushAsync(page);
                    }
                    catch (Exception e)
                    {

                    }
                });
            }
        }

        public ICommand DeleteAlarmCommand { get; set; }
        private void DeleteSelectedAlarm(ViewCell viewCell)
        {
            viewCell.ContextActions.Clear();
            var item = (AlarmViewModel) viewCell.BindingContext;
            item.IsActive = false;
            SiteService.DeleteAlarm(item.AlarmItem.ServerId);
            DependencyService.Get<IAlarmSetter>().DeleteAlarm(item.AlarmItem);
            App.Database.DeleteAlarmItem(item.AlarmItem.Id);
            Alarms.Remove(item);
            OnPropertyChanged(nameof(Alarms));
            DependencyService.Get<IMessage>().ShortAlert(Resource.DeletedString);
        }

        public ICommand ItemTappedCommand
        {
            get
            {
                return new Command(() =>
                {
                    MakeTrainingAlarmPage page = new MakeTrainingAlarmPage();
                    MakeTrainingAlarmPageViewModel vm = new MakeTrainingAlarmPageViewModel() { Navigation = Navigation };

                    vm.Alarm = SelectedAlarm;
                    vm.Alarm.TrainingId = SelectedAlarm.AlarmItem.TrainingId;
                    vm.SelectedTrainingItem = new TrainingViewModel(App.Database.GetTrainingItem(SelectedAlarm.AlarmItem.TrainingId));
                    page.BindingContext = vm;//SelectedTrainingItem null
                    Navigation.PushAsync(page, true);
                });
            }
        }
    }

    public class AlarmViewModel:BaseViewModel
    {
        public Alarm AlarmItem { get; set; }
        public AlarmViewModel()
        {
            AlarmItem = new Alarm();
            AlarmItem.TimeOffset = DateTimeOffset.Now;
        }

        public AlarmViewModel(Alarm alarmItem)
        {
            AlarmItem = alarmItem;
        }

        public int TrainingId { get; set; }

        public bool IsActive
        {
            get => AlarmItem.IsActive;
            set
            {
                if (AlarmItem.IsActive != value)
                {
                    AlarmItem.IsActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public DaysOfWeek Days
        {
            get => DaysOfWeek.Parse(AlarmItem.Days);
            set
            {
                if (AlarmItem.Days != value.Value)
                {
                    AlarmItem.Days = value.Value;
                    OnPropertyChanged();
                }
            }
        }

        //public string ToneFilename
        //{
        //    get => AlarmItem.ToneFilename;
        //    set
        //    {
        //        AlarmItem.ToneFilename = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public byte[] Tone
        //{
        //    get => AlarmItem.Tone;
        //    set
        //    {
        //        AlarmItem.Tone = value;
        //        OnPropertyChanged();
        //    }
        //}

        public string Name{
            get => AlarmItem.Name;
            set
            {
                AlarmItem.Name = value;
                OnPropertyChanged();
            }
        }

        public DateTimeOffset TimeOffset
        {
            get => AlarmItem.TimeOffset;
            set
            {
                AlarmItem.TimeOffset = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan Time
        {
            get { return AlarmItem.TimeOffset.LocalDateTime.TimeOfDay; }
            set { AlarmItem.TimeOffset = GetDateTimeOffsetFromTimeSpan(value); }
        }

        public DateTimeOffset GetDateTimeOffsetFromTimeSpan(TimeSpan time)
        {
            var now = DateTime.Now;
            var dateTime = new DateTime(now.Year, now.Month, now.Day, time.Hours, time.Minutes, time.Seconds);
            return new DateTimeOffset(dateTime);
        }
    }
}
