using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.Model;
using Xamarin.Forms;

namespace TrainingDay.Controls
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

        public static BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(T), typeof(EnumBindablePicker<T>), default(T), propertyChanged: OnSelectedItemChanged, defaultBindingMode: BindingMode.TwoWay);

        public T SelectedItem
        {
            get { return (T) GetValue(SelectedItemProperty); }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

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

        private static void OnSelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as EnumBindablePicker<T>;
            if (newvalue != null)
            {
                picker.SelectedIndex = picker.Items.IndexOf(GetEnumDescription(newvalue));
            }
        }

        private static string GetEnumDescription(object value)
        {
            string result = value.ToString();
            DescriptionAttribute attribute = typeof(T).GetRuntimeField(value.ToString()).GetCustomAttributes<DescriptionAttribute>(false).SingleOrDefault();
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            if (attribute != null)
                result = ci.Name=="ru-RU"? attribute.InfoRu: attribute.InfoEn;

            return result;
        }

        private T GetEnumByDescription(string description)
        {
            return Enum.GetValues(typeof(T)).Cast<T>().FirstOrDefault(x => string.Equals(GetEnumDescription(x), description));
        }
    }
}
