using System;
using System.Linq;
using System.Reflection;
using TrainingDay.Services;
using Xamarin.Forms;

namespace TrainingDay.Views.Controls
{
    public class EnumBindablePicker<T> : Picker where T : struct
    {
        public EnumBindablePicker()
        {
            SelectedIndexChanged += OnSelectedIndexChanged;
            //Fill the Items from the enum
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                Items.Add(GetEnumDescription(value));
            }
        }

        //public static BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(T), typeof(EnumBindablePicker<T>), default(T), propertyChanged: OnSelectedItemChanged, defaultBindingMode: BindingMode.TwoWay);

        //public T SelectedItem
        //{
        //    get { return (T)GetValue(SelectedItemProperty); }
        //    set
        //    {
        //        SetValue(SelectedItemProperty, value);
        //    }
        //}

        private void OnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (SelectedIndex < 0 || SelectedIndex > Items.Count - 1)
            {
                //SelectedItem = default(T);
            }
            else
            {
                //try parsing, if using description this will fail
                T match;
                if (!Enum.TryParse<T>(Items[SelectedIndex], out match))
                {
                    //find enum by Description
                    match = GetEnumByDescription(Items[SelectedIndex]);
                }
                SelectedItem = (T)Enum.Parse(typeof(T), match.ToString());
            }
        }

        //private static void OnSelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue)
        //{
        //    var picker = bindable as EnumBindablePicker<T>;
        //    if (newvalue != null)
        //    {
        //        picker.SelectedIndex = picker.Items.IndexOf(GetEnumDescription(newvalue));
        //    }
        //}

        public static string GetEnumDescription(object value)
        {
            try
            {
                var enumMember = value.GetType().GetMember(value.ToString()).FirstOrDefault();
                var c2 = enumMember.GetCustomAttributes().First();
                var attr = c2 as TrainingDay.Model.DescriptionAttribute;
                var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();

                string res =  ci.Name == "ru" || ci.Name == "ru-RU" ? 
                    attr.InfoRu : 
                    attr.InfoEn;
                return res;
            }
            catch (Exception e)
            {
                return value.ToString();
            }
        }

        private T GetEnumByDescription(string description)
        {
            return Enum.GetValues(typeof(T)).Cast<T>().FirstOrDefault(x => string.Equals(GetEnumDescription(x), description));
        }
    }
}
