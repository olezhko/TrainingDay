using System;
using TrainingDay.View;
using TrainingDay.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExerciseListPage : ContentPage
    {
        public ExerciseListPage()
        {
            InitializeComponent();
            _viewModel = new ExerciseListPageViewModel(){Navigation = this.Navigation};
            this.BindingContext = _viewModel;
            this.ToolbarItems.Remove(AcceptChouseMenu);
        }

        ExerciseListPageViewModel _viewModel;
        public ExerciseListPage(ExerciseListPageViewModel viewmodel)
        {
            InitializeComponent();
            _viewModel = viewmodel;
            BindingContext = _viewModel;
            ToolbarItems.Remove(RemoveFromBaseMenu);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.UpdateItems();
        }

        private async void AddExercisesButton_Clicked(object sender, EventArgs e)
        {
            ExerciseItemPage page = new ExerciseItemPage();
            ExerciseViewModel item = new ExerciseViewModel();
            page.BindingContext = item;
            await Navigation.PushAsync(page);
        }

        private async void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            TrainingExerciseViewModel selected = e.Item as TrainingExerciseViewModel;
            ExerciseItemPage page = new ExerciseItemPage();
            ExerciseViewModel vM = new ExerciseViewModel(selected.GetExercise());

            page.BindingContext = vM;
            await Navigation.PushAsync(page);
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            //_viewModel.SearchExercisesByName(SearchBar.Text);
            _viewModel.UpdateItems();
        }
    }
}