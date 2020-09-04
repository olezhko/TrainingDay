using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExerciseDescriptionControl : ContentView
    {
        public ExerciseDescriptionControl()
        {
            InitializeComponent();
            BindingContext = this;
        }

        #region Dep Propeties
        public static readonly BindableProperty StartingDescriptionProperty = BindableProperty.Create(nameof(StartingDescription), typeof(string), typeof(ExerciseDescriptionControl), null, BindingMode.TwoWay);
        public string StartingDescription
        {
            get { return (string)this.GetValue(StartingDescriptionProperty); }
            set { this.SetValue(StartingDescriptionProperty, value); }
        }

        public static readonly BindableProperty ExecutingDescriptionProperty = BindableProperty.Create(nameof(ExecutingDescription), typeof(string), typeof(ExerciseDescriptionControl), null, BindingMode.TwoWay);
        public string ExecutingDescription
        {
            get { return (string)this.GetValue(ExecutingDescriptionProperty); }
            set { this.SetValue(ExecutingDescriptionProperty, value); }
        }

        public static readonly BindableProperty AdviceDescriptionProperty = BindableProperty.Create(nameof(AdviceDescription), typeof(string), typeof(ExerciseDescriptionControl), null, BindingMode.TwoWay);
        public string AdviceDescription
        {
            get { return (string)this.GetValue(AdviceDescriptionProperty); }
            set { this.SetValue(AdviceDescriptionProperty, value); }
        }


        public static readonly BindableProperty IsReadOnlyProperty = BindableProperty.Create(nameof(IsReadOnly), typeof(bool), typeof(ExerciseDescriptionControl), false, BindingMode.TwoWay);
        public bool IsReadOnly
        {
            get { return (bool)this.GetValue(IsReadOnlyProperty); }
            set { this.SetValue(IsReadOnlyProperty, value); }
        }
        #endregion
    }
}