using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.Code;
using TrainingDay.Model;

namespace TrainingDay.ViewModel
{
    public class TrainingsHistoryViewModel:BaseViewModel
    {
        public ObservableCollection<LastTrainingViewModel> LastTrainings { get; set; }

        public TrainingsHistoryViewModel()
        {
            LastTrainings = new ObservableCollection<LastTrainingViewModel>();
            LoadItems();
        }

        private void LoadItems()
        {
            //var baseItems = App.database.GetLastTrainingItems();
            //var trainingItems = App.database.GetTrainingItems();

            //foreach (var lastTraining in baseItems)
            //{
            //    var training = trainingItems.First(item => item.Id == lastTraining.TrainingId);
            //    if (training!=null)
            //    {
            //        LastTrainings.Add(new LastTrainingViewModel()
            //        {
            //            ImplementeDateTime = lastTraining.Time,
            //            Description = training.Description,
            //            Title = training.Title
            //        });
            //    }
            //}

            LastTrainings.Add(new LastTrainingViewModel() {ImplementeDateTime = DateTime.Now, Description = "ПРИСЕДАНИЯ СО ШТАНГОЙ НА ПЛЕЧАХ", Title = "Тренеровка ног" });
            LastTrainings.Add(new LastTrainingViewModel() { ImplementeDateTime = DateTime.Now, Description = "ПРИСЕДАНИЯ СО ШТАНГОЙ НА ПЛЕЧАХ", Title = "Тренеровка ног" });
            LastTrainings.Add(new LastTrainingViewModel() { ImplementeDateTime = DateTime.Now, Description = "ПРИСЕДАНИЯ СО ШТАНГОЙ НА ПЛЕЧАХ", Title = "Тренеровка ног" });
            LastTrainings.Add(new LastTrainingViewModel() { ImplementeDateTime = DateTime.Now, Description = "ПРИСЕДАНИЯ СО ШТАНГОЙ НА ПЛЕЧАХ", Title = "Тренеровка ног" });
        }
    }

    public class LastTrainingViewModel:Training
    {
        public DateTime ImplementeDateTime { get; set; }
    }
}
