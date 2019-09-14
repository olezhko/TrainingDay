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
    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
            var navigationPage = new NavigationPage(new TrainingItemsBasePage()) {Icon = "main.png"};
            //navigationPage.PopRequested += NavigationPageOnPopRequested;
            Children.Add(navigationPage);
            Children.Add(new NavigationPage(new HistoryTrainingPage()){ Icon = "train_hist.png" });
            Children.Add(new NavigationPage(new ExerciseListPage()) { Icon = "exercise.png" });
            Children.Add(new NavigationPage(new TrainingAlarmListPage()) { Icon = "alarm.png" });
            Children.Add(new NavigationPage(new WeightViewAndSetPage()) { Icon = "weight.png" });
            Children.Add(new NavigationPage(new SettingsPage()) { Icon = "settings.png" });
        }

        //private void NavigationPageOnPopRequested(object sender, NavigationRequestedEventArgs e)
        //{
        //    var nav = sender as NavigationPage;
        //    var implementPage = nav.CurrentPage as TrainingImplementPage;
        //    if (implementPage != null)
        //    {
        //        if (implementPage.Items.All(a => a.IsNotFinished))
        //        {
        //            QuestionPopup popup = new QuestionPopup(Resource.DeleteExercises, Resource.AreYouSerious, new[] { "Yes", "No", "Cancel" });
        //            popup.PopupClosed += (o, closedArgs) =>
        //            {
        //                if (closedArgs.Button == "Yes")
        //                {
        //                    implementPage.FinishIfLast();
        //                }
        //                if (closedArgs.Button == "No")
        //                {
        //                    return;
        //                }
        //                if (closedArgs.Button == "Cancel")
        //                {
        //                    var task = e.Task;
        //                    // to do: cancel popup
        //                }
        //            };
        //            popup.Show();
        //        }
        //    }
        //}
    }
}