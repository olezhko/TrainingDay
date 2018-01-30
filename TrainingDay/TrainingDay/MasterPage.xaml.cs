using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.View;
using TrainingDay.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage : ContentPage
    {
        public ListView ListView { get { return listView; } }
        public MasterPage()
        {
            InitializeComponent();

            var masterPageItems = new List<MasterPageItem>();
            masterPageItems.Add(new MasterPageItem
            {
                Args = true, // 
                Title = "Выполнить тренировку",
                IconSource = "make_gym.png",
                TargetType = typeof(MakeTrainingPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "История тренировок",
                IconSource = "train_hist.png",
                TargetType = typeof(HistoryTrainingPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Добавить тренировку",
                IconSource = "add_train.png",
                TargetType = typeof(AddTrainingPage)
            });

            masterPageItems.Add(new MasterPageItem
            {
                Args = false,
                Title = "Доступные тренировки",
                IconSource = "main.png",
                TargetType = typeof(TrainingItemsBasePage)
            });

            masterPageItems.Add(new MasterPageItem
            {
                Title = "Контроль веса",
                IconSource = "weight.png",
                TargetType = typeof(WeightViewAndSetPage)
            });

            listView.ItemsSource = masterPageItems;
        }
    }

    public class MasterPageItem
    {
        public string Title { get; set; }

        public string IconSource { get; set; }

        public Type TargetType { get; set; }

        public object Args { get; set; }
    }
}