using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.View;
using TrainingDay.Views;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    class PreparedTrainingsPageViewModel:BaseViewModel
    {
        public ObservableCollection<PreparedTrainingViewModel> PreparedTrainingsCollection { get; set; }
        public INavigation Navigation { get; set; }
        public PreparedTrainingsPageViewModel()
        {
            PreparedTrainingsCollection = new ObservableCollection<PreparedTrainingViewModel>();
            FillTrainings();
            CreateNewTrainingCommand = new Command(AddNewTraining);
            ItemSelectedCommand = new Command<SelectedItemChangedEventArgs>(ShowTrainingExercieses);
        }

        public ICommand ItemSelectedCommand { get; set; }
        private void ShowTrainingExercieses(SelectedItemChangedEventArgs parameter)
        {
            PreparedTrainingViewModel trVm = parameter.SelectedItem as PreparedTrainingViewModel;

            try
            {
                if (trVm != null)
                {
                    TrainingExercisesPageViewModel vm = new TrainingExercisesPageViewModel()
                    {
                        Navigation = Navigation
                    };
                    vm.Load(new TrainingViewModel()
                    {
                        Exercises = trVm.Exercises,
                        Title = trVm.Name,
                    });
                    Navigation.PushAsync(new TrainingExercisesPage() { BindingContext = vm });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public ICommand CreateNewTrainingCommand { get; set; }
        private void AddNewTraining()
        {
            Navigation.PushAsync(new AddTrainingPage());
        }

        private void FillTrainings()
        {
            var exerciseBase = App.Database.GetExerciseItems().ToList();

            var value = MusclesConverter.SetMuscles(MusclesEnum.Chest);
            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.ChestString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.chest.png"),
                MainMuscles = new ObservableCollection<MuscleViewModel>(MusclesConverter.Convert(value)),
                Exercises = GetExerciseByPreferedMuscles(exerciseBase, MusclesConverter.Convert(value).ToArray())
            });

            value = MusclesConverter.SetMuscles(MusclesEnum.ShouldersBack,MusclesEnum.ShouldersFront,MusclesEnum.ShouldersMiddle);
            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.ShouldersString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.shoulders.png"),
                MainMuscles = new ObservableCollection<MuscleViewModel>(MusclesConverter.Convert(value)),
                Exercises = GetExerciseByPreferedMuscles(exerciseBase, MusclesConverter.Convert(value).ToArray())
            });

            value = MusclesConverter.SetMuscles(MusclesEnum.MiddleBack, MusclesEnum.WidestBack, MusclesEnum.ErectorSpinae,MusclesEnum.Trapezium);
            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.BackString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.back.png"),
                MainMuscles = new ObservableCollection<MuscleViewModel>(MusclesConverter.Convert(value)),
                Exercises = GetExerciseByPreferedMuscles(exerciseBase, MusclesConverter.Convert(value).ToArray())
            });

            value = MusclesConverter.SetMuscles(MusclesEnum.Biceps, MusclesEnum.Triceps, MusclesEnum.Forearm);
            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.ArmsString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.arms.png"),
                MainMuscles = new ObservableCollection<MuscleViewModel>(MusclesConverter.Convert(value)),
                Exercises = GetExerciseByPreferedMuscles(exerciseBase, MusclesConverter.Convert(value).ToArray())
            });

            value = MusclesConverter.SetMuscles(MusclesEnum.Buttocks, MusclesEnum.Thighs, MusclesEnum.Caviar, MusclesEnum.ShinAnteriorTibialis, MusclesEnum.ShinCamboloid, MusclesEnum.Quadriceps);
            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.LegsAndGlutesString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.legsAndGlutes.png"),
                MainMuscles = new ObservableCollection<MuscleViewModel>(MusclesConverter.Convert(value)),
                Exercises = GetExerciseByPreferedMuscles(exerciseBase, MusclesConverter.Convert(value).ToArray())
            });

            value = MusclesConverter.SetMuscles(MusclesEnum.Abdominal);
            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.AbdominalString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.press.png"),
                MainMuscles = new ObservableCollection<MuscleViewModel>(MusclesConverter.Convert(value)),
                Exercises = GetExerciseByPreferedMuscles(exerciseBase, MusclesConverter.Convert(value).ToArray())
            });



            OnPropertyChanged(nameof(PreparedTrainingsCollection));
        }

        private ObservableCollection<TrainingExerciseViewModel> GetExerciseByPreferedMuscles(List<Exercise> baseExercises, params MuscleViewModel[] muscles)
        {
            var result = new ObservableCollection<TrainingExerciseViewModel>();
            int order = 0;
            foreach (var baseExercise in baseExercises)
            {
                var exMuscles = MusclesConverter.Convert(baseExercise.Muscles);
                //var sub = exMuscles.Where(a=>muscles.Contains(a));
                var sub = new List<MuscleViewModel>();
                foreach (var muscleViewModel in exMuscles)
                {
                    if (muscles.Any(a=>a.Id == muscleViewModel.Id))
                    {
                        sub.Add(muscleViewModel);
                    }
                }

                if (sub.Any())
                {
                    result.Add(new TrainingExerciseViewModel(baseExercise,new TrainingExerciseComm()
                    {
                        Weight = 5,
                        CountOfTimes = 15,
                        CountOfApproches = 5,
                        OrderNumber = order,
                        ExerciseId = baseExercise.Id,
                    }));
                    order++;
                }
            }
            return result;
        }
    }


    class PreparedTrainingViewModel:BaseViewModel
    {
        public string Name { get; set; }
        public ImageSource TrainingImageUrl { get; set; }
        public ObservableCollection<MuscleViewModel> MainMuscles { get; set; }
        public ObservableCollection<TrainingExerciseViewModel> Exercises { get; set; }
    }
}
