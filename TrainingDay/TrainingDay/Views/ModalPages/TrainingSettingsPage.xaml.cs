using System;
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
        private void AddAlarmCommand_Clicked(object sender, EventArgs e)
        {
            ActionSelected?.BeginInvoke(this, TrainingSettingsActions.AddAlarm, null, null);
            Navigation.PopModalAsync(false);
        }

        private void ShareTrainingCommand_Clicked(object sender, EventArgs e)
        {
            ActionSelected?.BeginInvoke(this, TrainingSettingsActions.ShareTraining, null, null);
            Navigation.PopModalAsync(false);
        }

        private void SetSuperSetCommand_Clicked(object sender, EventArgs e)
        {
            ActionSelected?.BeginInvoke(this, TrainingSettingsActions.SuperSetAction, null, null);
            Navigation.PopModalAsync(false);
        }

        private void StartMoveExerciseCommand_Clicked(object sender, EventArgs e)
        {
            ActionSelected?.BeginInvoke(this, TrainingSettingsActions.MoveExercises, null, null);
            Navigation.PopModalAsync(false);
        }

        private void StartCopyExerciseCommand_Clicked(object sender, EventArgs e)
        {
            ActionSelected?.BeginInvoke(this, TrainingSettingsActions.CopyExercises, null, null);
            Navigation.PopModalAsync(false);
        }
    }
}