﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views.ModalPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrainingSettingsPage : ContentPage
    {
        public enum TrainingSettingsActions
        {
            AddAlarm,
            ShareTraining,
            SuperSetAction,
            MoveExercises,
            CopyExercises
        }

        public TrainingSettingsPage()
        {
            InitializeComponent();
        }

        public event EventHandler<TrainingSettingsActions> ActionSelected;
        private async void AddAlarmCommand_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
            ActionSelected?.Invoke(this, TrainingSettingsActions.AddAlarm);
        }

        private async void ShareTrainingCommand_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
            ActionSelected?.Invoke(this, TrainingSettingsActions.ShareTraining);
        }

        private async void SetSuperSetCommand_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
            ActionSelected?.Invoke(this, TrainingSettingsActions.SuperSetAction);
        }

        private async void StartMoveExerciseCommand_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
            ActionSelected?.Invoke(this, TrainingSettingsActions.MoveExercises);
        }

        private async void StartCopyExerciseCommand_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
            ActionSelected?.Invoke(this, TrainingSettingsActions.CopyExercises);
        }

        private async void ClosePage_Click(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}