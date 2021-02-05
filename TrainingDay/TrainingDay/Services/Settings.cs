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
            get => AppSettings.GetValueOrDefault(nameof(IsFirstTime), SettingsIsFirstTime);
            set => AppSettings.AddOrUpdateValue(nameof(IsFirstTime), value);
        }

        public static double WeightGoal
        {
            get => AppSettings.GetValueOrDefault(nameof(WeightGoal), SettingsWeightGoal);
            set => AppSettings.AddOrUpdateValue(nameof(WeightGoal), value);
        }

        private static readonly bool SettingsIsLightTheme = true;
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

        private static readonly bool SettingsIsDisplayOnImplement = true;
        public static bool IsDisplayOnImplement
        {
            get => AppSettings.GetValueOrDefault(nameof(IsDisplayOnImplement), SettingsIsDisplayOnImplement);
            set => AppSettings.AddOrUpdateValue(nameof(IsDisplayOnImplement), value);
        }

        private static readonly bool SettingsIsTrainingNotFinished = false;
        public static bool IsTrainingNotFinished
        {
            get => AppSettings.GetValueOrDefault(nameof(IsTrainingNotFinished), SettingsIsTrainingNotFinished);
            set => AppSettings.AddOrUpdateValue(nameof(IsTrainingNotFinished), value);
        }

        private static readonly string SettingsIsTrainingNotFinishedTime = "";
        public static string IsTrainingNotFinishedTime
        {
            get => AppSettings.GetValueOrDefault(nameof(IsTrainingNotFinishedTime), SettingsIsTrainingNotFinishedTime);
            set => AppSettings.AddOrUpdateValue(nameof(IsTrainingNotFinishedTime), value);
        }


        private static readonly bool SettingsIsExpandedMainGroup = true;
        public static bool IsExpandedMainGroup
        {
            get => AppSettings.GetValueOrDefault(nameof(IsExpandedMainGroup), SettingsIsExpandedMainGroup);
            set => AppSettings.AddOrUpdateValue(nameof(IsExpandedMainGroup), value);
        }

        private static readonly string SettingsToken = "";
        public static string Token
        {
            get => AppSettings.GetValueOrDefault(nameof(Token), SettingsToken);
            set => AppSettings.AddOrUpdateValue(nameof(Token), value);
        }

        private static readonly bool SettingsIsTokenSavedOnServer = false;
        public static bool IsTokenSavedOnServer
        {
            get => AppSettings.GetValueOrDefault(nameof(IsTokenSavedOnServer), SettingsIsTokenSavedOnServer);
            set => AppSettings.AddOrUpdateValue(nameof(IsTokenSavedOnServer), value);
        }

        private static readonly double SettingsWaistGoal = 0.0; 
        public static double WaistGoal
        {
            get => AppSettings.GetValueOrDefault(nameof(WaistGoal), SettingsWaistGoal);
            set => AppSettings.AddOrUpdateValue(nameof(WaistGoal), value);
        }

        private static readonly double SettingsHipGoal = 0.0;
        public static double HipGoal
        {
            get => AppSettings.GetValueOrDefault(nameof(HipGoal), SettingsHipGoal);
            set => AppSettings.AddOrUpdateValue(nameof(HipGoal), value);
        }



        private static readonly string SettingsGoogleToken = "";
        public static string GoogleToken
        {
            get => AppSettings.GetValueOrDefault(nameof(GoogleToken), SettingsGoogleToken);
            set => AppSettings.AddOrUpdateValue(nameof(GoogleToken), value);
        }
    }
}
