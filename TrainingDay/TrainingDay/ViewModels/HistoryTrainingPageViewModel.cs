using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json;
using TrainingDay.Model;
using TrainingDay.Services;
using TrainingDay.View;
using TrainingDay.Views;
using TrainingDay.Views.Controls;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    public class HistoryTrainingPageViewModel: BaseViewModel
    {
        public States State { get; set; } = States.View1;
        public ObservableCollection<LastTrainingViewModelList> LastTrainings { get; set; }
        public ObservableCollection<TrainingExerciseViewModel> TrainingItems { get; set; }
        public HistoryTrainingPageViewModel()
        {
            LastTrainings = new ObservableCollection<LastTrainingViewModelList>();
            TrainingItems = new ObservableCollection<TrainingExerciseViewModel>();
            ItemSelectedCommand = new Command(ViewLastTrainingExercisesPage);
        }
        public INavigation Navigation { get; set; }
        // просто меняем вид по State, и из выбранной тренировки выдираем упражнения и фигачим их в коллекцию
        public ICommand ItemSelectedCommand { get; set; }
        private void ViewLastTrainingExercisesPage(object sender)
        {
            State = States.View2;
            OnPropertyChanged(nameof(State));
            var args = sender as ItemTappedEventArgs;
            LastTrainingViewModel item = args.Item as LastTrainingViewModel;
            TrainingItems.Clear();
            TrainingItems = new ObservableCollection<TrainingExerciseViewModel>(item.Items);
            OnPropertyChanged(nameof(TrainingItems));
        }

        public void LoadItems()
        {
            LastTrainings.Clear();
            var baseItems = App.Database.GetLastTrainingItems();
            var trainingExerciseItems = App.Database.GetLastTrainingExerciseItems();

            baseItems = baseItems.OrderByDescending(item => item.Time);
            foreach (var lastTraining in baseItems)
            {
                try
                {
                    var newItem = new LastTrainingViewModel()
                    {
                        ElapsedTime = lastTraining.ElapsedTime,
                        ImplementDateTime = lastTraining.Time,
                        Title = lastTraining.Title,
                        Id = lastTraining.Id,
                        TrainingId = lastTraining.TrainingId
                    };

                    foreach (var trainingExercise in trainingExerciseItems)
                    {
                        if (trainingExercise.LastTrainingId == lastTraining.Id)
                        {
                            var ex = new TrainingExerciseViewModel()
                            {
                                ExerciseItemName = trainingExercise.ExerciseName,
                                Muscles = new ObservableCollection<MuscleViewModel>(!string.IsNullOrWhiteSpace(trainingExercise.MusclesString)
                                        ? MusclesConverter.ConvertFromStringToList(trainingExercise.MusclesString)
                                        : MusclesConverter.ConvertFromStringToList(MusclesConverter.ConvertToString(trainingExercise.Muscles))),
                                ExerciseImageUrl = trainingExercise.ExerciseImageUrl,
                                OrderNumber = trainingExercise.OrderNumber,
                                ShortDescription = trainingExercise.Description,
                                TrainingExerciseId = trainingExercise.Id,
                                SuperSetId = trainingExercise.SuperSetId,
                            };

                            ex.Tags = ExerciseTagExtension.ConvertFromIntToList(trainingExercise.TagsValue);

                            if (trainingExercise.WeightAndRepsString != null && ex.Tags.Contains(ExerciseTags.ExerciseByRepsAndWeight))
                                ex.WeightAndRepsItems = JsonConvert.DeserializeObject<ObservableCollection<WeightAndReps>>(trainingExercise.WeightAndRepsString);

                            if (ex.Tags.Contains(ExerciseTags.ExerciseByTime) || ex.Tags.Contains(ExerciseTags.ExerciseByDistance))
                            {
                                var obj = JsonConvert.DeserializeObject<(TimeSpan, double)>(trainingExercise.WeightAndRepsString);
                                ex.Distance = obj.Item2;
                                ex.Time = obj.Item1;
                            }

                            newItem.Items.Add(ex);
                        }
                    }

                    PutItemToListByDate(newItem);
                }
                catch{}
            }

            OnPropertyChanged(nameof(LastTrainings));
            if (LastTrainings.Count == 0)
            {
                ChangeState(States.NoData);
            }
        }

        private void PutItemToListByDate(LastTrainingViewModel newItem)
        {
            AddLastTraining(newItem, Resources.Resource.WeekString, 7, out var addResult);
            if (addResult)
            {
                return;
            }

            AddLastTraining(newItem, Resources.Resource.OneMounthString, 31, out addResult);
            if (addResult)
            {
                return;
            }

            AddLastTraining(newItem, Resources.Resource.ThreeMounthString, 91, out addResult);
            if (addResult)
            {
                return;
            }

            AddLastTraining(newItem, Resources.Resource.HalfYearString, 182, out addResult);
            if (addResult)
            {
                return;
            }

            AddLastTraining(newItem, Resources.Resource.YearString, 365, out addResult);
            if (addResult)
            {
                return;
            }
            AddLastTraining(newItem, Resources.Resource.MoreThanYearString, -1, out addResult);
        }

        private void AddLastTraining(LastTrainingViewModel newItem, string daysSectorString, int daysSector, out bool addResult)
        {
            addResult = false;
            if (daysSector == -1 || DateTime.Now - newItem.ImplementDateTime < TimeSpan.FromDays(daysSector))
            {
                try
                {
                    var index = LastTrainings.First(a => a.Heading == daysSectorString);
                    if (index != null)
                    {
                        index.Add(newItem);
                        addResult = true;
                    }
                    else
                    {
                        LastTrainings.Add(new LastTrainingViewModelList(){ Heading = daysSectorString });
                        LastTrainings.Last().Add(newItem);
                        addResult = true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    LastTrainings.Add(new LastTrainingViewModelList() { Heading = daysSectorString });
                    LastTrainings.Last().Add(newItem);
                    addResult = true;
                }
            }
        }

        public void ChangeState(States view1)
        {
            State = view1;
            OnPropertyChanged(nameof(State));
        }

        public ICommand StartAgainCommand =>new Command<LastTrainingViewModel>(StartAgainTraining);
        private void StartAgainTraining(LastTrainingViewModel last)
        {
            int trId = last.TrainingId;
            TrainingViewModel training = new TrainingViewModel();
            if (trId == 0)
            {
                training.Title = last.Title;
                foreach (var trainingExerciseViewModel in last.Items)
                {
                    training.AddExercise(trainingExerciseViewModel);
                }
            }
            else
            {
                training = new TrainingViewModel(App.Database.GetTrainingItem(trId));
                TrainingItemsBasePageViewModel.PrepareTrainingViewModel(training);
            }
            Navigation.PushAsync(new TrainingImplementPage() { TrainingItem = training, Title = training.Title });
        }


        public ICommand ShowStatCommand => new Command(ShowStatPage);
        private async void ShowStatPage()
        {
            //await Navigation.PushAsync(new TrainingsStatisticsPage());
        }
    }

    public class LastTrainingViewModel : Training
    {
        public DateTime ImplementDateTime { get; set; }
        public TimeSpan ElapsedTime { get; set; }
        public int TrainingId { get; set; }
        public ObservableCollection<TrainingExerciseViewModel> Items { get; set; }

        public LastTrainingViewModel()
        {
            Items = new ObservableCollection<TrainingExerciseViewModel>(); 
        }
    }

    public class LastTrainingViewModelList : ObservableCollection<LastTrainingViewModel>
    {
        private string heading;
        public string Heading
        {
            get => heading;
            set
            {
                heading = value;
            }
        } 
    }
}
