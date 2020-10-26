using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TrainingDay.Services;
using TrainingDay.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExerciseView : ContentView,INotifyPropertyChanged
    {
        private Picker dataPicker;
        public ExerciseView()
        {
            InitializeComponent();
            dataPicker = new Picker();
            dataPicker.ItemsSource = Enumerable.Range(0, 60).Select(min => min.ToString("D2")).ToList();
            dataPicker.SelectedIndexChanged += DataPickerOnSelectedIndexChanged;
            dataPicker.IsVisible = false;
            dataPicker.Unfocused += DataPicker_Unfocused;
            BindingContextChanged += ExerciseView_BindingContextChanged;
            MainGrid.Children.Add(dataPicker);
        }

        private void ExerciseView_BindingContextChanged(object sender, EventArgs e)
        {
            if (BindingContext != null)
            {
                var item = ((TrainingExerciseViewModel) BindingContext);
                item.WeightAndRepsItems.CollectionChanged += WeightAndRepsItems_CollectionChanged;
                CollectionView.HeightRequest = DivideRoundingUp(item.WeightAndRepsItems.Count, 2) * (50 + 4);
                MusclesWrapPanel.ItemsSource = item.Muscles;
                if (item.ExerciseImageUrl != null)
                {
                    AdviceDescEditor.IsReadOnly = true;
                    StartingDescEditor.IsReadOnly = true;
                    ExecDescEditor.IsReadOnly = true;
                    NameEditor.IsReadOnly = true;
                    MusclesWrapPanel.IsEditableItems = false;
                }
                NameEditor.IsVisible = item.ExerciseImageUrl == null;
                NameLabel.IsVisible = item.ExerciseImageUrl != null;
            }
        }

        private void WeightAndRepsItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var item = ((TrainingExerciseViewModel) BindingContext);
            if (item.WeightAndRepsItems.Count != 0)
            {
                CollectionView.HeightRequest = DivideRoundingUp(item.WeightAndRepsItems.Count, 2) * (50 + 4);
            }
        }

        public static int DivideRoundingUp(int x, int y)
        {
            int quotient = Math.DivRem(x, y, out var remainder);
            return remainder == 0 ? quotient : quotient + 1;
        }

        public TrainingExerciseViewModel CurrentExercise
        {
            get
            {
                var ex = (TrainingExerciseViewModel) BindingContext;
                return ex;
            }
        }

        public event EventHandler<ImageSource> ImageTappedEvent;
        private void ImageTapped(object sender, EventArgs e)
        {
            ImageTappedEvent?.Invoke(this, ImageControl.Source);
        }

        private void AddWeightAndRepsItem_Clicked(object sender, EventArgs e)
        {
            var item = ((TrainingExerciseViewModel)BindingContext);
            item.WeightAndRepsItems.Add(item.WeightAndRepsItems.Count == 0
                ? new WeightAndReps(0, 15)
                : new WeightAndReps(item.WeightAndRepsItems.Last().Weight, item.WeightAndRepsItems.Last().Repetitions));
        }

        private void DataPickerOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!dataPicker.IsVisible)
            {
                return;
            }
            var item = ((TrainingExerciseViewModel)BindingContext);
            switch (mode)
            {
                case PickerMode.Hour:
                    item.TimeHours = dataPicker.SelectedIndex;
                    break;

                case PickerMode.Minute:
                    item.TimeMinutes = dataPicker.SelectedIndex;
                    break;

                case PickerMode.Second:
                    item.TimeSeconds = dataPicker.SelectedIndex;
                    break;
            }
        }


        private void DataPicker_Unfocused(object sender, FocusEventArgs e)
        {
            dataPicker.IsVisible = false;
        }

        private void HourGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            dataPicker.SelectedIndex = -1;
            dataPicker.IsVisible = true;
            dataPicker.Focus();

            mode = PickerMode.Hour;
        }

        private void SecondGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            dataPicker.SelectedIndex = -1;
            dataPicker.IsVisible = true;
            dataPicker.Focus();
            mode = PickerMode.Second;
        }

        private void MinuteGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            dataPicker.SelectedIndex = -1;
            dataPicker.IsVisible = true;
            dataPicker.Focus();
            mode = PickerMode.Minute;
        }

        private PickerMode mode;
        public enum PickerMode
        {
            None,
            Hour,
            Minute,
            Second
        }

        private void Description_Click(object sender, EventArgs e)
        {
            DesctiptionLabel.BackgroundColor = Color.Green;
            VideoLabel.BackgroundColor = Color.DarkGray;

            VideoCollectionView.IsVisible = false;
            DescriptionGrid.IsVisible = true;
            VideoActivityIndicatorGrid.IsVisible = false;
        }

        private async void Video_Click(object sender, EventArgs e)
        {
            DesctiptionLabel.BackgroundColor = Color.DarkGray;
            VideoLabel.BackgroundColor = Color.Green;
            DescriptionGrid.IsVisible = false;

            VideoActivityIndicatorGrid.IsVisible = true;
            VideoActivityIndicator.IsRunning = true;

            await LoadVideoItems();

            VideoActivityIndicator.IsRunning = false;
            VideoCollectionView.IsVisible = true;
            VideoActivityIndicatorGrid.IsVisible = false;
        }

        public ObservableCollection<YoutubeVideoItem> VideoItems { get; set; } = new ObservableCollection<YoutubeVideoItem>();
        public async Task LoadVideoItems()
        {
            try
            {
                VideoItems.Clear();
                if (CurrentExercise != null)
                {
                    var items = await YoutubeService.GetVideoItemsAsync(CurrentExercise.ExerciseItemName);
                    foreach (var item in items)
                    {
                        VideoItems.Add(item);
                    }
                }

                VideoCollectionView.ItemsSource = VideoItems;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }


        public ICommand DeleteRequestCommand => new Command<WeightAndReps>(DeleteRequestWeightAndReps);

        public bool IsViewImage
        {
            get { return ImageControl.IsVisible;}
            set { ImageControl.IsVisible = false; }
        }

        private void DeleteRequestWeightAndReps(WeightAndReps sender)
        {
            var item = ((TrainingExerciseViewModel)BindingContext);
            item.WeightAndRepsItems.Remove(sender);
        }


        private void StartCalculateTime_Clicked(object sender, EventArgs e)
        {
            var item = ((TrainingExerciseViewModel)BindingContext);

            item.StartCalculateDateTime = DateTime.Now;
            item.IsTimeCalculating = true;
        }
    }
}