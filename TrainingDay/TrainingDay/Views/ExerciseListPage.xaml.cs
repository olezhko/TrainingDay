using System;
using System.Linq;
using TrainingDay.Services;
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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.UpdateItems();
            if (newExerciseAdded)
            { 
                newExerciseAdded = false;
                ScrollToEnd();
            }
        }

        private void ScrollToEnd()
        {
            Device.BeginInvokeOnMainThread(() => {
                ExercisesListView.ScrollTo(_viewModel.Items[_viewModel.Items.Count - 1], ScrollToPosition.End, false);
            });
        }

        private async void AddExercisesButton_Clicked(object sender, EventArgs e)
        {
            ExerciseItemPage page = new ExerciseItemPage();
            ExerciseViewModel item = new ExerciseViewModel();
            page.BindingContext = item;
            page.NewExerciseAdded += Page_NewExerciseAdded;
            await Navigation.PushAsync(page);
        }

        public bool newExerciseAdded = false;
        private void Page_NewExerciseAdded(object sender, Exercise e)
        {
            newExerciseAdded = true;
            _viewModel.Items.Add(new TrainingExerciseViewModel(e,new TrainingExerciseComm()));
        }

        private async void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            TrainingExerciseViewModel selected = e.Item as TrainingExerciseViewModel;
            ExerciseItemPage page = new ExerciseItemPage();
            ExerciseViewModel vM = new ExerciseViewModel(App.Database.GetExerciseItem(selected.ExerciseId));
            page.BindingContext = vM;
            page.ExerciseChanged += Page_ExerciseChanged;
            await Navigation.PushAsync(page);
        }

        private void Page_ExerciseChanged(object sender, Exercise e)
        {
            try
            {
                var delete = _viewModel.Items.First(item => item.ExerciseId == e.Id);
                var index = _viewModel.Items.IndexOf(delete);
                _viewModel.Items.Remove(delete);
                _viewModel.Items.Insert(index, new TrainingExerciseViewModel(e, new TrainingExerciseComm()));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.UpdateItems();
        }
    }
}