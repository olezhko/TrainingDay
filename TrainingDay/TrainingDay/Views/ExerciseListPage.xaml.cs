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
        private int _listViewScrollId;
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
            AdMobView.AdUnitId =
                Device.RuntimePlatform == Device.Android ? "ca-app-pub-8728883017081055/2677919170" : "ca-app-pub-8728883017081055/7119310061";
            _viewModel.UpdateItems();
        }

        private async void AddExercisesButton_Clicked(object sender, EventArgs e)
        {
            ExerciseItemPage page = new ExerciseItemPage();
            ExerciseViewModel item = new ExerciseViewModel();
            page.BindingContext = item;
            page.NewExerciseAdded += Page_NewExerciseAdded;
            await Navigation.PushAsync(page);
        }

        private void Page_NewExerciseAdded(object sender, Exercise e)
        {
            _viewModel.Items.Add(new TrainingExerciseViewModel(e,new TrainingExerciseComm()));
        }

        private async void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
           TrainingExerciseViewModel selected = e.Item as TrainingExerciseViewModel;
            ExerciseItemPage page = new ExerciseItemPage();
            ExerciseViewModel vM = new ExerciseViewModel(App.Database.GetExerciseItem(selected.ExerciseId));
            _listViewScrollId = selected.ExerciseId;
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