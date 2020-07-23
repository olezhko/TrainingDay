using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
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
        [Description("Передняя дельта", "Front Delta")] ShouldersFront,
        [Description("Задняя дельта", "Back Delta")] ShouldersBack,
        [Description("Средняя дельта", "Middle Delta")] ShouldersMiddle,
        [Description("Широчайшие спина", "WidestBack")] WidestBack,
        [Description("Средняя спина", "MiddleBack")] MiddleBack,
        [Description("Разгибатель позвоночника", "Erector Spinae")] ErectorSpinae,
        [Description("Грудь", "Chest")] Chest,
        [Description("Пресс", "Abdominal")] Abdominal,
        [Description("Трицепс", "Triceps")] Triceps,
        [Description("Бицепс", "Biceps")] Biceps,
        [Description("Предплечье", "Forearm")] Forearm,
        [Description("Квадрицепс", "Quadriceps")] Quadriceps,
        [Description("Икры", "Caviar")] Caviar,
        [Description("Камболовидная", "Camboloid")] ShinCamboloid,
        [Description("Передняя большеберцовая", "Anterior tibialis")] ShinAnteriorTibialis,
        [Description("Бедра", "Thighs")] Thighs,
        [Description("Ягодицы", "Glute")] Buttocks,
        [Description("Кардио", "Cardio")] Cardio,


        [Description("Выберите", "Chouse")] None
    }

    public class MusclesConverter
    {
        public static List<Color> Colors = new List<Color>(){
            Color.Red,Color.Green,Color.Gold,
            Color.SlateBlue,Color.Teal,Color.Gray,
            Color.DodgerBlue,Color.Orange,Color.Purple,
            Color.Olive,Color.SandyBrown,Color.SeaGreen,
            Color.DarkSlateGray,Color.MediumTurquoise,Color.Sienna,
            Color.CornflowerBlue,Color.Crimson,Color.DarkRed,
            Color.MediumPurple,Color.Firebrick};

        public static List<MuscleViewModel> SetMuscles(params MusclesEnum[] muscles)
        {
            List<MuscleViewModel> result = new List<MuscleViewModel>();
            for (int i = 0; i < muscles.Length; i++)
            {
                result.Add(ConvertFromEnumToViewModel(muscles[i]));
            }

            return result;
        }

        private static MuscleViewModel ConvertFromEnumToViewModel(MusclesEnum muscle)
        {
            var index = (int) muscle;
            return new MuscleViewModel()
            {
                Id = index,
                Color = Colors[index],
                Name = EnumBindablePicker<MusclesEnum>.GetEnumDescription(muscle)
            };
        }

        public static string ConvertToString(int muscles)
        {
            if (muscles == 0)
            {
                return "";
            }
            StringBuilder res = new StringBuilder();
            BitArray arr = new BitArray(new[] { muscles });
            for (int i = 0; i < 32; i++)
            {
                if (arr.Get(i))
                {
                    res.Append((MusclesEnum)i);
                    res.Append(",");
                }
            }

            if (res.Length == 0)
            {
                return "";
            }
            res.Remove(res.Length - 1, 1);
            return res.ToString();
        }

        public static List<MuscleViewModel> ConvertFromStringToList(string value, int oldvalue = 0) // separator
        {
            List<MuscleViewModel> muscle = new List<MuscleViewModel>();

            if (!string.IsNullOrEmpty(value))
            {
                string[] enums = value.Split(',');
                try
                {
                    for (int i = 0; i < enums.Length; i++)
                    {
                        var res = Enum.Parse(typeof(MusclesEnum), enums[i]);
                        muscle.Add(ConvertFromEnumToViewModel((MusclesEnum)res));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                var res = ConvertToString(oldvalue);
                if (!string.IsNullOrEmpty(res))
                    muscle = ConvertFromStringToList(res);
            }
            
            return muscle;
        }

        public static string ConvertFromListToString(List<MuscleViewModel> array)
        {
            try
            {
                if (array.Count == 0)
                {
                    return "";
                }
                StringBuilder res = new StringBuilder();
                foreach (var muscleViewModel in array)
                {
                    res.Append((MusclesEnum)muscleViewModel.Id);
                    res.Append(",");
                }

                res.Remove(res.Length - 1, 1);
                return res.ToString();
            }
            catch (Exception e)
            {
                return "";
            }
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
