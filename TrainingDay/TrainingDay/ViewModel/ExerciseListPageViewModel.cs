using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TrainingDay.Annotations;
using TrainingDay.Code;
using TrainingDay.Controls;
using TrainingDay.Helpers;
using TrainingDay.Model;
using Xamarin.Forms;
using XLabs;

namespace TrainingDay.ViewModel
{
    public class ExerciseListPageViewModel : BaseViewModel
    {
        public ObservableCollection<ExerciseSelectViewModel> Items { get; set; }
        public ICommand DeleteExerciseCommand { protected set; get; }
        private AddTrainingViewModel lvm;
        public AddTrainingViewModel ListViewModel
        {
            get { return lvm; }
            set
            {
                if (lvm != value)
                {
                    lvm = value;
                    OnPropertyChanged();
                }
            }
        }

        public INavigation Navigation { get; set; }

        public ExerciseListPageViewModel()
        {
            DeleteExerciseCommand = new Command(DeleteExercise);
        }

        private void DeleteExercise()
        {
            bool isDeleted = false;
            foreach (var exerciseSelectViewModel in Items)
            {
                if (exerciseSelectViewModel.IsSelected)
                {
                    App.Database.DeleteExerciseItem(exerciseSelectViewModel.Id);
                    isDeleted = true;
                }
            }
            if (isDeleted)
            {
                DependencyService.Get<IMessage>().ShortAlert(Resource.DeletedString);
                LoadItems();
            }
        }

        internal void LoadItems(MusclesEnum muscle)
        {
            Items.Clear();
            foreach (var exercise in App.Database.GetExerciseItems())
            {
                var newItem = new ExerciseSelectViewModel(exercise);
                if (newItem.Muscles.Any(a=>a.Id == (int)muscle))
                {
                    Items.Add(newItem);
                }
            }
            OnPropertyChanged(nameof(ExerciseSelectViewModel.Muscles));
            OnPropertyChanged(nameof(Items));
        }

        public void LoadItems()
        {
            if (Items == null)
                Items = new ObservableCollection<ExerciseSelectViewModel>();
            Items.Clear();
            foreach (var exercise in App.Database.GetExerciseItems())
            {
                var newItem = new ExerciseSelectViewModel(exercise);
                Items.Add(newItem);
            }
            OnPropertyChanged(nameof(ExerciseSelectViewModel.Muscles));
            OnPropertyChanged(nameof(Items));
        }

        public List<Exercise> GetSelectedItems()
        {
            var items = Items.Where(a => a.IsSelected).Select(x => new Exercise()
            {
                Muscles = MusclesConverter.ConvertBack(x.Muscles.ToList()),
                ExerciseItemName = x.ExerciseItemName,
                Id = x.Id,
                CountOfApproches = x.CountOfApproches,
                CountOfTimes = x.CountOfTimes,
                ShortDescription = x.ShortDescription
            }).ToList();

            return items;
        }
    }

    public class MuscleViewModel:BaseViewModel
    {
        public int Id { get; set; }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private Color color;
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                OnPropertyChanged();
            }
        }
    }

    public enum MusclesEnum
    {
        [Description("Шея","Neck")] Neck,
        [Description("Плечи", "Shoulders")] Shoulders,
        [Description("Дельтовидная","Deltoid")] Deltoid,
        [Description("Трапеции", "Trapezium")] Trapezium,
        [Description("Бицепс", "Biceps")] Biceps,
        [Description("Грудь", "Chest")] Chest,
        [Description("Предплечье", "Forearm")] Forearm,
        [Description("Пресс", "Abdominal")] Abdominal,
        [Description("Квадрицепс", "Quadriceps")] Quadriceps,
        [Description("Икры", "Caviar")] Caviar,
        [Description("Бедра", "Thighs")] Thighs,
        [Description("Ягодицы", "Buttocks")] Buttocks,
        [Description("Нижняя спина", "LowerBack")] LowerBack,
        [Description("Средняя спина", "MiddleBack")] MiddleBack,
        [Description("Широчайшие спина", "WidestBack")] WidestBack,
        [Description("Трицепс", "Triceps")] Triceps,
    }

    public class MusclesConverter:IValueConverter
    {
        public static int SetMuscles(params MusclesEnum[] muscles)
        {
            BitArray arr = new BitArray(32);

            for (int i = 0; i < muscles.Length; i++)
            {
                arr.Set((int)muscles[i],true);
            }
            int[] array = new int[1];
            ((ICollection) arr).CopyTo(array, 0);
            return array[0];
        }

        public static int SetMuscles(int oldMuscles,params MusclesEnum[] muscles)
        {
            BitArray arr = new BitArray(new []{ oldMuscles });

            for (int i = 0; i < muscles.Length; i++)
            {
                arr.Set((int)muscles[i], true);
            }
            int[] array = new int[1];
            ((ICollection)arr).CopyTo(array, 0);
            return array[0];
        }

        static List<Color> Colors = new List<Color>(){
            Color.Red,Color.Green,Color.Yellow,
            Color.Blue,Color.Teal,Color.Gray,
            Color.Aqua,Color.Orange,Color.Purple,
            Color.Olive,Color.Brown,Color.SeaGreen,
            Color.DarkSlateGray,Color.LawnGreen,Color.Khaki,
        Color.Navy};

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((int) value);
        }

        public static List<MuscleViewModel> Convert(int muscles)
        {
            List<MuscleViewModel> res = new List<MuscleViewModel>();
            BitArray arr = new BitArray(new []{ muscles });
            for (int i = 0; i < 32; i++)
            {
                if (arr.Get(i))
                {
                    res.Add(new MuscleViewModel(){Id = i, Color = Colors[i], Name= EnumTools.GetDecription((MusclesEnum)i, DependencyService.Get<ILocalize>().GetCurrentCultureInfo()) });
                }
            }

            return res;
        }

        public static int ConvertBack(List<MuscleViewModel> list)
        {
            int res = 0;
            foreach (var muscleViewModel in list)
            {
                SetMuscles(res,(MusclesEnum)muscleViewModel.Id);
            }

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return ConvertBack((List<MuscleViewModel>)value);
        }
    }

    public class EnumTools
    {
        internal static string GetDecription(MusclesEnum e, CultureInfo cultureInfo)
        {
            var info = e.GetType().GetRuntimeField(e.ToString());
            if (info != null)
            {
                var attributes = info.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes != null)
                {
                    foreach (Attribute item in attributes)
                    {
                        if (item is DescriptionAttribute)
                            return cultureInfo.Name == "ru-RU"?(item as DescriptionAttribute).InfoRu: (item as DescriptionAttribute).InfoEn;
                    }
                }
            }
            return e.ToString("G");
        }
    }
}
