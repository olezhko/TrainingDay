using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RepsAndWeightControl : ContentView
    {
        public RepsAndWeightControl()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public static readonly BindableProperty ValueProperty = BindableProperty.Create(nameof(Value), typeof(WeightAndReps), typeof(RepsAndWeightControl), new WeightAndReps(5,15), defaultBindingMode: BindingMode.TwoWay);
        public WeightAndReps Value
        {
            get { return (WeightAndReps)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private void DeleteButton_Clicked(object sender, EventArgs e)
        {
            OnDeleteRequire();
        }

        public void OnDeleteRequire()
        {
            DeleteRequestCommand?.Execute(Value);
        }


        public static readonly BindableProperty DeleteRequestCommandProperty =
            BindableProperty.Create("DeleteRequestCommand", typeof(Command), typeof(RepsAndWeightControl), null);

        public ICommand DeleteRequestCommand
        {
            get { return (ICommand)GetValue(DeleteRequestCommandProperty); }
            set { SetValue(DeleteRequestCommandProperty, value); }
        }
    }


    public class WeightAndReps
    {
        public int Repetitions { get; set; }
        public double Weight { get; set; }
        public string WeightString
        {
            get => Weight.ToString(CultureInfo.InvariantCulture);
            set
            {
                var res = double.TryParse(value,NumberStyles.Any,CultureInfo.InvariantCulture, out var weight);
                if (res)
                {
                    Weight = weight;
                }
            }
        }
        
        public WeightAndReps() { }
        public WeightAndReps(double weight, int repetitions)
        {
            Weight = weight;
            Repetitions = repetitions;
        }
    }
}