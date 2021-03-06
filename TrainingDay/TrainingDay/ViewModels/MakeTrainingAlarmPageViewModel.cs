﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrainingDay.Controls;
using TrainingDay.Resources;
using TrainingDay.Services;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    public class MakeTrainingAlarmPageViewModel : BaseViewModel
    {
        public ObservableCollection<TrainingViewModel> TrainingItems { get; set; } = new ObservableCollection<TrainingViewModel>();

        private TrainingViewModel _selTrainingViewModel;
        public TrainingViewModel SelectedTrainingItem
        {
            get => _selTrainingViewModel;
            set
            {
                _selTrainingViewModel = value;
                OnPropertyChanged();
            }
        }

        private AlarmViewModel alarm;
        public AlarmViewModel Alarm
        {
            get => alarm;
            set
            {
                alarm = value;
                OnPropertyChanged();
            }
        }
        public INavigation Navigation { get; set; }

        public MakeTrainingAlarmPageViewModel()
        {
            Alarm = new AlarmViewModel();
            LoadTrainingItems();
        }

        public ICommand SaveAlarmCommand => new Command(SaveAlarm);
        private async void SaveAlarm()
        {
            if (!ValidateFields())
            {
                DependencyService.Get<IMessage>().ShortAlert(Resource.NotAllFieldsEntered);
                return;
            }

            Alarm.IsActive = true;
            //Set alarm and add to our list of alarms
            Alarm.AlarmItem.TrainingId = SelectedTrainingItem.Id;
            var newItem = new Alarm()
            {
                Id = Alarm.AlarmItem.Id,
                Days = Alarm.Days.Value,
                Name = Alarm.Name,
                TimeOffset = Alarm.GetDateTimeOffsetFromTimeSpan(Alarm.Time),
                IsActive = Alarm.IsActive,
                TrainingId = SelectedTrainingItem.Id,
                ServerId = Alarm.AlarmItem.ServerId
            };
            if (newItem.ServerId == 0)
            {
                var serverId = await SiteService.SendAlarm(newItem);
                newItem.ServerId = serverId;
            }
            else
            {
                await SiteService.UpdateAlarm(newItem);
            }

            var id = App.Database.SaveAlarmItem(newItem);
            Alarm.AlarmItem.Id = id;
            DependencyService.Get<IAlarmSetter>().SetAlarm(newItem);
            //pop the page
            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
            Navigation?.PopAsync();
        }

        protected bool ValidateFields()
        {
            bool validation = DaysOfWeek.GetHasADayBeenSelected(Alarm.Days);

            if (string.IsNullOrWhiteSpace(Alarm.Name) || SelectedTrainingItem == null)
            {
                validation = false;
            }

            return validation;
        }

        private void LoadTrainingItems()
        {
            var items = App.Database.GetTrainingItems();
            foreach (var training in items)
            {
                TrainingItems.Add(new TrainingViewModel(training));
            }

            OnPropertyChanged(nameof(TrainingItems));
        }
    }

    public interface IAlarmSetter
    {
        void SetAlarm(Alarm alarm);

        void SetRepeatingAlarm(Alarm alarm);

        void DeleteAlarm(Alarm alarm);

        void DeleteAllAlarms(List<Alarm> alarms);
    }
}
