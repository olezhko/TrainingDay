using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Crashes;
using TrainingDay.Model;
using TrainingDay.Resources;
using TrainingDay.Services;
using TrainingDay.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views.ModalPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrainingsSetGroupPage : ContentPage
    {
        public TrainingsSetGroupPage()
        {
            InitializeComponent();
            var trainingsGroups = App.Database.GetTrainingsGroups();
            ItemsListView.ItemsSource = new ObservableCollection<TrainingUnion>(trainingsGroups);
        }

        public TrainingViewModel trainingMoveToGroup { get; set; }
        private async void ShowNewGroupWnd_Click(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("", Resource.GroupingEnterNameofGroup, Resource.OkString, Resource.CancelString);
            if (result != null)
            {
                try
                {
                    var name = result;
                    if (!string.IsNullOrEmpty(name))
                    {
                        var unions = App.Database.GetTrainingsGroups();
                        DeleteGroup(new List<TrainingUnion>(unions));
                        var union = unions.FirstOrDefault(un => un.Name == name);
                        if (union != null) // если группа с таким именем уже существует
                        {
                            var viewModel = new TrainingUnionViewModel(union);
                            if (!viewModel.TrainingIDs.Contains(trainingMoveToGroup.Id))
                            {
                                viewModel.TrainingIDs.Add(trainingMoveToGroup.Id);// добавляем в список тренировок у группы выбранную тренировку
                                trainingMoveToGroup.GroupName = viewModel;
                                App.Database.SaveTrainingGroup(viewModel.Model);
                            }
                        }
                        else
                        {
                            var viewModel = new TrainingUnionViewModel();//новая группа
                            viewModel.Name = name;
                            viewModel.TrainingIDs.Add(trainingMoveToGroup.Id);
                            trainingMoveToGroup.GroupName = viewModel;
                            App.Database.SaveTrainingGroup(viewModel.Model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex);
                }
            }

            DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
            await Navigation.PopAsync(Device.RuntimePlatform == Device.Android);
        }

        private void DeleteGroup(List<TrainingUnion> unions)
        {
            try
            {
                if (trainingMoveToGroup.GroupName != null && trainingMoveToGroup.GroupName.Id != 0)
                {
                    var unionToEdit = new TrainingUnionViewModel(unions.First(u => u.Id == trainingMoveToGroup.GroupName.Id));
                    unionToEdit.TrainingIDs.Remove(trainingMoveToGroup.Id);
                    if (unionToEdit.TrainingIDs.Count != 0)
                        App.Database.SaveTrainingGroup(unionToEdit.Model);
                    else
                        App.Database.DeleteTrainingGroup(unionToEdit.Id);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        public void GroupSelected(int id)
        {
            try
            {
                //  если тренировка уже в группе и группа такая же, как и выбрали
                if (trainingMoveToGroup.GroupName != null && id == trainingMoveToGroup.GroupName.Id)
                {
                    Navigation.PopAsync(Device.RuntimePlatform == Device.Android);
                    return;
                }

                var unions = App.Database.GetTrainingsGroups();
                // если тренировка в группе и группа не "Основные"
                if (trainingMoveToGroup.GroupName != null && trainingMoveToGroup.GroupName.Id != 0)
                {
                    var unionToEdit = unions.First(u => u.Id == trainingMoveToGroup.GroupName.Id);
                    var vm = new TrainingUnionViewModel(unionToEdit);
                    vm.TrainingIDs.Remove(trainingMoveToGroup.Id);
                    if (vm.TrainingIDs.Count != 0)
                        App.Database.SaveTrainingGroup(vm.Model);
                    else
                        App.Database.DeleteTrainingGroup(vm.Id);
                }

                if (id != 0)
                {
                    var union = unions.FirstOrDefault(un => un.Id == id);
                    if (union != null)
                    {
                        var viewModel = new TrainingUnionViewModel(union);
                        viewModel.TrainingIDs.Add(trainingMoveToGroup.Id);
                        trainingMoveToGroup.GroupName = viewModel;
                        App.Database.SaveTrainingGroup(viewModel.Model);
                    }
                }


                DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
            
            Navigation.PopAsync(Device.RuntimePlatform == Device.Android);
        }

        private void ItemsListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedGroupToTraining = e.Item as TrainingUnion;
            GroupSelected(selectedGroupToTraining.Id);
        }
    }
}
