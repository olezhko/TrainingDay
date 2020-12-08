using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TrainingDay.Model;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.View;
using TrainingDay.Views;
using TrainingDay.Views.Controls;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    class PreparedTrainingsPageViewModel:BaseViewModel
    {
        public ObservableCollection<PreparedTrainingViewModel> PreparedTrainingsCollection { get; set; }
        public INavigation Navigation { get; set; }
        public PreparedTrainingsPageViewModel()
        {
            FillTrainings();
            CreateNewTrainingCommand = new Command(AddNewTraining);
            ItemSelectedCommand = new Command<ItemTappedEventArgs>(ShowTrainingExercises);
        }

        public ICommand ItemSelectedCommand { get; set; }
        private void ShowTrainingExercises(ItemTappedEventArgs parameter)
        {
            PreparedTrainingViewModel trVm = parameter.Item as PreparedTrainingViewModel;

            try
            {
                if (trVm != null)
                {
                    TrainingExercisesPageViewModel vm = new TrainingExercisesPageViewModel()
                    {
                        Navigation = Navigation
                    };
                    TrainingViewModel tr = new TrainingViewModel();
                    foreach (var trainingExerciseViewModel in trVm.Exercises)
                    {
                        tr.AddExercise(trainingExerciseViewModel);
                    }
                    tr.Title = trVm.Name;
                    vm.Load(tr);
                    Navigation.PushAsync(new TrainingExercisesPage() { BindingContext = vm, IsShowDeleteUnusefulExercisesHelp = true});
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

        public void FillTrainings()
        {
            PreparedTrainingsCollection = new ObservableCollection<PreparedTrainingViewModel>();
            var exerciseBase = App.Database.GetExerciseItems().ToList();

            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.HomeString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.home.png"),
                Exercises = new ObservableCollection<TrainingExerciseViewModel>(exerciseBase.Where(ex => ExerciseTagExtension.ConvertFromIntToList(ex.TagsValue).Contains(ExerciseTags.CanDoAtHome)).Select(item => new TrainingExerciseViewModel(item, new TrainingExerciseComm())))
            });

            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.FitnessString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.fitness.png"),
                Exercises = new ObservableCollection<TrainingExerciseViewModel>(GetExercisesByCodeNum(exerciseBase, 113,
                    109, 114, 115, 116, 117, 118, 119, 108, 111, 84, 103, 102, 110))
            });

            var value = MusclesConverter.SetMuscles(MusclesEnum.Chest);
            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.ChestString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.chest.png"),
                MainMuscles = new ObservableCollection<MuscleViewModel>(value),
                Exercises = GetExerciseByPreferedMuscles(exerciseBase, value.ToArray())
            });

            value = MusclesConverter.SetMuscles(MusclesEnum.ShouldersBack,MusclesEnum.ShouldersFront,MusclesEnum.ShouldersMiddle);
            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.ShouldersString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.shoulders.png"),
                MainMuscles = new ObservableCollection<MuscleViewModel>(value),
                Exercises = GetExerciseByPreferedMuscles(exerciseBase, value.ToArray())
            });

            value = MusclesConverter.SetMuscles(MusclesEnum.MiddleBack, MusclesEnum.WidestBack, MusclesEnum.ErectorSpinae,MusclesEnum.Trapezium);
            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.BackString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.back.png"),
                MainMuscles = new ObservableCollection<MuscleViewModel>(value),
                Exercises = GetExerciseByPreferedMuscles(exerciseBase, value.ToArray())
            });

            value = MusclesConverter.SetMuscles(MusclesEnum.Biceps, MusclesEnum.Triceps, MusclesEnum.Forearm);
            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.ArmsString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.arms.png"),
                MainMuscles = new ObservableCollection<MuscleViewModel>(value),
                Exercises = GetExerciseByPreferedMuscles(exerciseBase, value.ToArray())
            });

            value = MusclesConverter.SetMuscles(MusclesEnum.Buttocks, MusclesEnum.Thighs, MusclesEnum.Caviar, MusclesEnum.ShinAnteriorTibialis, MusclesEnum.ShinCamboloid, MusclesEnum.Quadriceps);
            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.LegsAndGlutesString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.legsAndGlutes.png"),
                MainMuscles = new ObservableCollection<MuscleViewModel>(value),
                Exercises = GetExerciseByPreferedMuscles(exerciseBase, value.ToArray())
            });

            value = MusclesConverter.SetMuscles(MusclesEnum.Abdominal);
            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.AbdominalString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.press.png"),
                MainMuscles = new ObservableCollection<MuscleViewModel>(value),
                Exercises = GetExerciseByPreferedMuscles(exerciseBase, value.ToArray())
            });


            value = MusclesConverter.SetMuscles(MusclesEnum.Cardio);
            PreparedTrainingsCollection.Add(new PreparedTrainingViewModel()
            {
                Name = Resource.CardioString,
                TrainingImageUrl = ImageSource.FromResource("TrainingDay.Resources.prepared.cardio.png"),
                MainMuscles = new ObservableCollection<MuscleViewModel>(value),
                Exercises = GetExerciseByPreferedMuscles(exerciseBase, value.ToArray())
            });

            OnPropertyChanged(nameof(PreparedTrainingsCollection));
        }

        private ObservableCollection<TrainingExerciseViewModel> GetExerciseByPreferedMuscles(List<Exercise> baseExercises, params MuscleViewModel[] muscles)
        {
            var result = new ObservableCollection<TrainingExerciseViewModel>();
            int order = 0;
            foreach (var baseExercise in baseExercises)
            {
                try
                {
                    var exMuscles = MusclesConverter.ConvertFromStringToList(baseExercise.MusclesString);
                    //var sub = exMuscles.Where(a=>muscles.Contains(a));
                    var sub = new List<MuscleViewModel>();
                    foreach (var muscleViewModel in exMuscles)
                    {
                        if (muscles.Any(a => a.Id == muscleViewModel.Id))
                        {
                            sub.Add(muscleViewModel);
                        }
                    }

                    if (sub.Any())
                    {
                        result.Add(new TrainingExerciseViewModel(baseExercise, new TrainingExerciseComm()
                        {
                            OrderNumber = order,
                            ExerciseId = baseExercise.Id,
                        })
                        {
                            WeightAndRepsItems = new ObservableCollection<WeightAndReps>()
                        });
                        order++;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return result;
        }

        private ObservableCollection<TrainingExerciseViewModel> GetExercisesByCodeNum(List<Exercise> baseExercises, params int[] codeNums)
        {
            var result = new ObservableCollection<TrainingExerciseViewModel>();
            int i = 0;
            foreach (var codeNum in codeNums)
            {
                try
                {
                    var exercise = baseExercises.FirstOrDefault(item => item.CodeNum == codeNum);
                    if (exercise != null)
                    {
                        result.Add(new TrainingExerciseViewModel(exercise, new TrainingExerciseComm()
                        {
                            OrderNumber = i,
                            ExerciseId = exercise.Id,
                        })
                        {
                            WeightAndRepsItems = new ObservableCollection<WeightAndReps>(new WeightAndReps[3])
                        });
                    }

                    i++;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
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
