using TrainingDay.ViewModels;
using Xamarin.Forms;

namespace TrainingDay.Views.Controls
{
    public partial class AlarmListCell : ViewCell
    {
        AlarmViewModel _alarm;
        public AlarmListCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext == null) return;
            _alarm = (AlarmViewModel)BindingContext;

            if (string.IsNullOrWhiteSpace(_alarm.Name))
                NameLabel.IsVisible = false;
            else
            {
                NameLabel.Text = _alarm.Name;
                NameLabel.IsVisible = true;
                TrainingNameLabel.Text = App.Database.GetTrainingItem(_alarm.AlarmItem.TrainingId).Title;
            }

            StartSpan.Text = _alarm.Time.ToString(@"hh\:mm");
            SetDynamicResources(_alarm.IsActive);
            //IsActiveSwitch.IsToggled = _alarm.IsActive;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ActiveSwitch.Toggled += ActiveSwitch_Toggled;
        }

        protected override void OnDisappearing()
        {
            ActiveSwitch.Toggled -= ActiveSwitch_Toggled;
            base.OnDisappearing();
        }

        void ActiveSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            //switch resource
            DaysOfWeekView.IsEnabled = e.Value;
            SetDynamicResources(e.Value);
        }

        void SetDynamicResources(bool state)
        {
            if (state)
            {
                //if is active
                NameLabel.Style = (Style)App.Current.Resources["AlarmNameHeading"];
                TimeLabel.Style = (Style)App.Current.Resources["AlarmTimeHeading"];
                TrainingNameLabel.Style = (Style)App.Current.Resources["AlarmNameHeading"];
                FrequencyLabel.Style = (Style)App.Current.Resources["AlarmExtrasHeading"];
            }
            else
            {
                //if is not active
                TrainingNameLabel.Style = (Style)App.Current.Resources["AlarmNameDisabledHeading"];
                NameLabel.Style = (Style)App.Current.Resources["AlarmNameDisabledHeading"];
                TimeLabel.Style = (Style)App.Current.Resources["AlarmTimeDisabledHeading"];
                FrequencyLabel.Style = (Style)App.Current.Resources["AlarmExtrasDisabledHeading"];
            }
        }
    }
}