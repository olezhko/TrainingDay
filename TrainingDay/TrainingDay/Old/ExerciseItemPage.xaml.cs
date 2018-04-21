using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.Controls;
using TrainingDay.Helpers;
using TrainingDay.Model;
using TrainingDay.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExerciseItemPage : ContentPage
    {
        public ExerciseItemPage()
        {
            InitializeComponent();
        }

        private void Save_clicked(object sender, EventArgs e)
        {        
            if (!String.IsNullOrEmpty(ExerciseView.CurrentExercise.ExerciseItemName))
            {
                DependencyService.Get<IMessage>().ShortAlert(Resource.SavedString);
                App.Database.SaveExerciseItem(ExerciseView.CurrentExercise.GetExercise());
            }
            this.Navigation.PopAsync();
        }

        public void LoadExercise(int id)
        {
            var item = App.Database.GetExerciseItem(id);
            //BindingContext = item;
            ExerciseView.BindingContext = new ExerciseSelectViewModel(item);
        }
    }
}