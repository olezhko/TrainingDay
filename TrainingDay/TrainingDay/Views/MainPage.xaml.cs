using System;
using System.Linq;
using TrainingDay.Resources;
using TrainingDay.View;
using TrainingDay.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            //var navigationPage = new NavigationPage(new TrainingItemsBasePage()) { IconImageSource = "main.png"};
            ////navigationPage.PopRequested += NavigationPageOnPopRequested;
            //Children.Add(navigationPage);
            //Children.Add(new NavigationPage(new HistoryTrainingPage()){ IconImageSource = "train_hist.png" });
            //Children.Add(new NavigationPage(new ExerciseListPage()) { IconImageSource = "exercise.png" });
            //Children.Add(new NavigationPage(new WeightViewAndSetPage()) { IconImageSource = "weight.png" });
            //Children.Add(new NavigationPage(new TrainingAlarmListPage()) { IconImageSource = "alarm.png" });
            //Children.Add(new NavigationPage(new SettingsPage()) { IconImageSource = "settings.png" });
        }

        private void ShowTrainingsItemsPage_Click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TrainingItemsBasePage() { Title = Resource.TrainingsBaseString },true);
        }

        private void ShowWeightControlPage_Click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new WeightViewAndSetPage() { Title = Resource.WeightString }, true);
        }

        private void ShowSettingsPage_Click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SettingsPage() { Title = Resource.SettingsString }, true);
        }

        private void ShowHistoryItemsPage_Click(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HistoryTrainingPage() { Title = Resource.HistoryTrainings }, true);
        }
    }
}