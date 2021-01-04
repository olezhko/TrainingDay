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
        private async void AddAlarmCommand_Clicked(object sender, EventArgs e)
        {
            ActionSelected?.BeginInvoke(this, TrainingSettingsActions.AddAlarm, null, null);
            await Navigation.PopModalAsync(Device.RuntimePlatform == Device.Android);
        }

        private async void ShareTrainingCommand_Clicked(object sender, EventArgs e)
        {
            ActionSelected?.BeginInvoke(this, TrainingSettingsActions.ShareTraining, null, null);
            await Navigation.PopModalAsync(Device.RuntimePlatform == Device.Android);
        }

        private async void SetSuperSetCommand_Clicked(object sender, EventArgs e)
        {
            ActionSelected?.BeginInvoke(this, TrainingSettingsActions.SuperSetAction, null, null);
            await Navigation.PopModalAsync(Device.RuntimePlatform == Device.Android);
        }

        private async void StartMoveExerciseCommand_Clicked(object sender, EventArgs e)
        {
            ActionSelected?.BeginInvoke(this, TrainingSettingsActions.MoveExercises, null, null);
            await Navigation.PopModalAsync(Device.RuntimePlatform == Device.Android);
        }

        private async void StartCopyExerciseCommand_Clicked(object sender, EventArgs e)
        {
            ActionSelected?.BeginInvoke(this, TrainingSettingsActions.CopyExercises, null, null);
            await Navigation.PopModalAsync(Device.RuntimePlatform == Device.Android);
        }

        private async void ClosePage_Click(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(Device.RuntimePlatform == Device.Android);
        }
    }
}