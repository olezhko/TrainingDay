using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using myToolTipSample;
using Newtonsoft.Json;
using TrainingDay.Model;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.View;
using TrainingDay.ViewModels;
using TrainingDay.Views.Controls;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrainingItemsBasePage : ContentPage
    {
        public TrainingItemsBasePage()
        {
            InitializeComponent();
            BindingContext = new TrainingItemsBasePageViewModel(){Navigation = this.Navigation };
        }

        private TrainingItemsBasePageViewModel vm;
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ToolTipEffect.SetIsOpen(AddImage, false);
            ToolTipEffect.SetIsOpen(AddImage, true);
            vm = BindingContext as TrainingItemsBasePageViewModel;
            vm.LoadItems();
            IsStartNotFinishedTraining();
        }

        protected override void OnDisappearing()
        {
            ToolTipEffect.SetIsOpen(AddImage, false);
            base.OnDisappearing();
        }

        private void IsStartNotFinishedTraining()
        {
            if (Settings.IsTrainingNotFinished)
            {
                Settings.IsTrainingNotFinished = false;
                QuestionPopup popup = new QuestionPopup("", Resource.ContinueLastTrainingQuestion);
                popup.PopupClosed += (o, closedArgs) =>
                {
                    if (closedArgs.Button == Resource.OkString)
                    {
                        try
                        {
                            var fn = "NotFinished.trday";
                            var filename = Path.Combine(FileSystem.CacheDirectory, fn);
                            var trainingSerialize = TrainingSerialize.LoadFromFile(File.ReadAllBytes(filename));
                            TrainingViewModel training = new TrainingViewModel();
                            training.Title = trainingSerialize.Title;
                            training.Id = trainingSerialize.Id;
                            foreach (var trainingExerciseSerialize in trainingSerialize.Items)
                            {
                                var item = new TrainingExerciseViewModel()
                                {
                                    TrainingExerciseId = trainingExerciseSerialize.TrainingExerciseId,
                                    ExerciseId = trainingExerciseSerialize.ExerciseId,
                                    ExerciseImageUrl = trainingExerciseSerialize.ExerciseImageUrl,
                                    TrainingId = trainingExerciseSerialize.TrainingId,
                                    IsNotFinished = trainingExerciseSerialize.IsNotFinished,
                                    Muscles = new ObservableCollection<MuscleViewModel>(MusclesConverter.ConvertFromStringToList(trainingExerciseSerialize.Muscles)),
                                    OrderNumber = trainingExerciseSerialize.OrderNumber,
                                    ExerciseItemName = trainingExerciseSerialize.ExerciseItemName,
                                    ShortDescription = trainingExerciseSerialize.ShortDescription,
                                    SuperSetId = trainingExerciseSerialize.SuperSetId,
                                    SuperSetNum = trainingExerciseSerialize.SuperSetNum,

                                    Tags = ExerciseTagExtension.ConvertFromIntToList(trainingExerciseSerialize.TagsValue),
                                };

                                TrainingExerciseViewModel.Description descriptionsStrings = JsonConvert.DeserializeObject<TrainingExerciseViewModel.Description>(trainingExerciseSerialize.ShortDescription);
                                item.AdviceDescription = descriptionsStrings.advice;
                                item.ExecutionDescription = descriptionsStrings.exec;
                                item.StartingPositionDescription = descriptionsStrings.start;

                                ExerciseTagExtension.ConvertJsonBack(item, trainingExerciseSerialize.WeightAndRepsString);
                                training.AddExercise(item);
                            }

                            Navigation.PushAsync(new TrainingImplementPage()
                            {
                                TrainingItem = training,
                                Title = training.Title,
                                StartTime = TimeSpan.Parse(Settings.IsTrainingNotFinishedTime)
                            });
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                };
                popup.Show(Resource.OkString, Resource.CancelString);
            }
        }
    }
}