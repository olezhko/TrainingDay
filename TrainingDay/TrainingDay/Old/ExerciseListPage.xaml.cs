using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.Model;
using TrainingDay.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExerciseListPage : ContentPage
    {
        public ExerciseListPageViewModel ViewModel;
        public ExerciseListPage(ExerciseListPageViewModel viewmodel)
        {
            InitializeComponent();
            ViewModel = viewmodel;
            this.BindingContext = ViewModel;
        }

        protected override void OnAppearing()
        {
            ViewModel.LoadItems();
            base.OnAppearing();
        }

        private async void AddExercisesButton_Clicked(object sender, EventArgs e)
        {
            ExerciseItemPage page = new ExerciseItemPage();
            Exercise item = new Exercise();
            page.BindingContext = item;
            await Navigation.PushAsync(page);
        }
    }
}