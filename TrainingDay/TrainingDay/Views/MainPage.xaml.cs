using System;
using TrainingDay.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.iOS)
            {
                MainGrid.Children.Remove(AlarmsStackLayout);
                Grid.SetRow(SettingsStackLayout,4);
            }
        }

        private void ShowTrainingsItemsPage_Click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TrainingItemsBasePage() { Title = Resource.TrainingsBaseString },true);
        }

        private void ShowWeightControlPage_Click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new WeightViewAndSetPage() { Title = Resource.WeightControlString }, true);
        }

        private void ShowSettingsPage_Click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage() { Title = Resource.SettingsString }, true);
        }

        private void ShowHistoryItemsPage_Click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HistoryTrainingPage() { Title = Resource.HistoryTrainings }, true);
        }

        private void ShowNotesPage_Click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new BlogsPage() { Title = Resource.NewsString }, true);
        }

        private void ShowTrainingAlarms_Click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TrainingAlarmListPage() { Title = Resource.TrainingNotifications }, true);
        }
    }
}