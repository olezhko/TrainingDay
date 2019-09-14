using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace TrainingDay.Services
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;
        private static readonly bool SettingsIsFirstTime = true;
        private static readonly double SettingsWeightGoal = 0.0;


        #endregion


        public static string GeneralSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SettingsKey, value);
            }
        }

        public static bool IsFirstTime
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(IsFirstTime), SettingsIsFirstTime);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(IsFirstTime), value);
            }
        }

        public static double WeightGoal
        {
            get
            {
                return AppSettings.GetValueOrDefault(nameof(WeightGoal), SettingsWeightGoal);
            }
            set
            {
                AppSettings.AddOrUpdateValue(nameof(WeightGoal), value);
            }
        }

        private static readonly bool SettingsIsLightTheme = false;
        public static bool IsLightTheme
        {
            get => AppSettings.GetValueOrDefault(nameof(IsLightTheme), SettingsIsLightTheme);
            set => AppSettings.AddOrUpdateValue(nameof(IsLightTheme), value);
        }

        private static readonly string SettingsLastSyncVersion = "1.0.0.0";
        public static string LastSyncVersion
        {
            get => AppSettings.GetValueOrDefault(nameof(LastSyncVersion), SettingsLastSyncVersion);
            set => AppSettings.AddOrUpdateValue(nameof(LastSyncVersion), value);
        }

        private static readonly bool SettingsIsWeightNotify = true;
        public static bool IsWeightNotify
        {
            get => AppSettings.GetValueOrDefault(nameof(IsWeightNotify), SettingsIsWeightNotify);
            set => AppSettings.AddOrUpdateValue(nameof(IsWeightNotify), value);
        }

        private static readonly int SettingsWeightNotifyFreq = 1;
        public static int WeightNotifyFreq
        {
            get => AppSettings.GetValueOrDefault(nameof(WeightNotifyFreq), SettingsWeightNotifyFreq);
            set => AppSettings.AddOrUpdateValue(nameof(WeightNotifyFreq), value);
        }
    }
}
