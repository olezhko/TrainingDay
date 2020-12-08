using Android.Content;
using Android.Support.V4.Content.Res;

using System.ComponentModel;
using Android.Graphics;
using TrainingDay.Controls;
using TrainingDay.Droid.Render;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(DayOfWeekButton), typeof(AndroidDayOfTheWeekButtonRenderer))]
namespace TrainingDay.Droid.Render
{
    public class AndroidDayOfTheWeekButtonRenderer : Xamarin.Forms.Platform.Android.ButtonRenderer
    {
        DayOfWeekButton _dowButton;
        Android.Graphics.Color _deselectedTextColor = Android.Graphics.Color.Bisque;
        Android.Graphics.Color _selectedTextColor = new Android.Graphics.Color(29, 33, 48);
        private Android.Graphics.Color _deselectedColor = Android.Graphics.Color.DimGray;
        private Android.Graphics.Color _selectedColor = Android.Graphics.Color.White;

        public AndroidDayOfTheWeekButtonRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control == null) return;

            if (Element != null)
                _dowButton = (DayOfWeekButton)Element;

            if (Control.Width > Control.Height)
                Control.SetHeight(Control.Width);
            else
                Control.SetWidth(Control.Height);

            _dowButton.Clicked += Element_Clicked;

            Control.SetPaddingRelative(0, 0, 0, 0);
            Control.TextSize = 20;
            SetPadding(0, 0, 0, 0);
            ButtonDeselected();
            if (_dowButton.IsSelected)
                ButtonSelected();
            else
                ButtonDeselected();
        }

        private void Element_Clicked(object sender, System.EventArgs e)
        {
            //_dowButton.IsSelected = !_dowButton.IsSelected;
        }

        protected override void Dispose(bool disposing)
        {
            _dowButton.Clicked -= Element_Clicked;
            base.Dispose(disposing);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == DayOfWeekButton.IsSelectedProperty.PropertyName)
            {
                if (_dowButton.IsSelected)
                {
                    ButtonSelected();
                }
                else
                {
                    ButtonDeselected();
                }
            }
        }

        void ButtonSelected()
        {
            Control.SetTextColor(_selectedTextColor);
            //Control.SetBackgroundColor(_selectedColor);
        }

        void ButtonDeselected()
        {
            Control.SetTextColor(_deselectedTextColor);
            //Control.SetBackgroundColor(_deselectedColor);
        }
    }
}