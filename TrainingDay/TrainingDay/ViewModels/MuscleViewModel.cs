using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TrainingDay.Services;
using TrainingDay.Views.Controls;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    public class MuscleViewModel : BaseViewModel
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
        [Description("Шея", "Neck")] Neck,
        [Description("Трапеции", "Trapezium")] Trapezium,
        [Description("Плечи. Передняя дельта", "Shoulders. Front")] ShouldersFront,
        [Description("Плечи. Задняя дельта", "Shoulders. Back")] ShouldersBack,
        [Description("Плечи. Средняя дельта", "Shoulders. Middle")] ShouldersMiddle,
        [Description("Широчайшие спина", "WidestBack")] WidestBack,
        [Description("Средняя спина", "MiddleBack")] MiddleBack,
        [Description("Разгибатель позвоночника", "Erector Spinae")] ErectorSpinae,
        [Description("Грудь", "Chest")] Chest,
        [Description("Пресс", "Abdominal")] Abdominal,
        [Description("Трицепс", "Triceps")] Triceps,
        [Description("Бицепс", "Biceps")] Biceps,
        [Description("Предплечье", "Forearm")] Forearm,
        [Description("Квадрицепс", "Quadriceps")] Quadriceps,
        [Description("Голень. Икры", "Caviar")] Caviar,
        [Description("Голень. Камболовидная", "Shin, Camboloid")] ShinCamboloid,
        [Description("Голень. Передняя большеберцовая", "Shin, Anterior tibialis")] ShinAnteriorTibialis,
        [Description("Бедра", "Thighs")] Thighs,
        [Description("Ягодицы", "Buttocks")] Buttocks,


        [Description("Выберите", "Chouse")] None
    }

    public class MusclesConverter : IValueConverter
    {
        public static int SetMuscles(params MusclesEnum[] muscles)
        {
            BitArray arr = new BitArray(32);

            for (int i = 0; i < muscles.Length; i++)
            {
                arr.Set((int)muscles[i], true);
            }
            int[] array = new int[1];
            ((ICollection)arr).CopyTo(array, 0);
            return array[0];
        }

        public static int SetMuscles(int oldMuscles, params MusclesEnum[] muscles)
        {
            BitArray arr = new BitArray(new[] { oldMuscles });

            for (int i = 0; i < muscles.Length; i++)
            {
                arr.Set((int)muscles[i], true);
            }
            int[] array = new int[1];
            ((ICollection)arr).CopyTo(array, 0);
            return array[0];
        }

        public static List<Color> Colors = new List<Color>(){
            Color.Red,Color.Green,Color.Gold,
            Color.SlateBlue,Color.Teal,Color.Gray,
            Color.DodgerBlue,Color.Orange,Color.Purple,
            Color.Olive,Color.SandyBrown,Color.SeaGreen,
            Color.DarkSlateGray,Color.MediumTurquoise,Color.Sienna,
            Color.CornflowerBlue,Color.Crimson,Color.DarkRed,
            Color.MediumPurple};


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((int)value);
        }

        public static List<MuscleViewModel> Convert(int muscles)
        {
            List<MuscleViewModel> res = new List<MuscleViewModel>();
            BitArray arr = new BitArray(new[] { muscles });
            for (int i = 0; i < 32; i++)
            {
                if (arr.Get(i))
                {
                    res.Add(new MuscleViewModel() { Id = i, Color = Colors[i],
                        Name = EnumBindablePicker<MusclesEnum>.GetEnumDescription((MusclesEnum)i)});
                }
            }

            return res;
        }

        public static int ConvertBack(List<MuscleViewModel> list)
        {
            int res = 0;
            foreach (var muscleViewModel in list)
            {
                res = SetMuscles(res, (MusclesEnum)muscleViewModel.Id);
            }

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((List<MuscleViewModel>)value);
        }

        internal static int Convert(string value)
        {
            int muscle = 0;
            string[] enums = value.Split(',');
            for (int i = 0; i < enums.Length; i++)
            {
                var res = Enum.Parse(typeof(MusclesEnum), enums[i]);
                muscle = SetMuscles(muscle, (MusclesEnum)res);
            }
            return muscle;
        }
    }

    public static class StringExtensions
    {
        public static bool Contains(this String str, String substring,
            StringComparison comp)
        {
            if (substring == null)
                throw new ArgumentNullException("substring",
                    "substring cannot be null.");
            else if (!Enum.IsDefined(typeof(StringComparison), comp))
                throw new ArgumentException("comp is not a member of StringComparison",
                    "comp");

            return str.IndexOf(substring, comp) >= 0;
        }
    }

    internal class DescriptionAttribute : Attribute
    {
        public string InfoRu;
        public string InfoEn;
        public DescriptionAttribute(string infoRu, string infoEn)
        {
            InfoEn = infoEn;
            InfoRu = infoRu;
        }
    }
}
