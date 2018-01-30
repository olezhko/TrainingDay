using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.Model;
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
            var friend = (Exercise)BindingContext;
            if (!String.IsNullOrEmpty(friend.ExerciseItemName))
            {
                DependencyService.Get<IMessage>().ShortAlert("Сохранено");
                int id = App.Database.SaveExerciseItem(friend);
                Debug.WriteLine("Сохранено упражнение с id = " + id);
            }
            this.Navigation.PopAsync();
        }

        public void LoadExercise(int id)
        {
            var item = App.Database.GetExerciseItem(id);
            BindingContext = item;
        }
    }
}