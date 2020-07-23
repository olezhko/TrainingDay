using Android.Content;
using Android.Support.V4.Content.Res;
using System.ComponentModel;
using TrainingDay.Controls;
using TrainingDay.Droid.Render;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomPicker), typeof(AndroidPickerRenderer))]
namespace TrainingDay.Droid.Render
{
    public class AndroidPickerRenderer : Xamarin.Forms.Platform.Android.AppCompat.PickerRenderer
    {
        CustomPicker _customPicker;

        public AndroidPickerRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control == null) return;

            _customPicker = (CustomPicker)Element;
            if (_customPicker.Hint != null)
                Control.Hint = _customPicker.Hint;

            _customPicker.IsValidChanged += OnIsValidChanged;
            Control.SetCompoundDrawablesRelativeWithIntrinsicBounds(null, null, ResourcesCompat.GetDrawable(Resources, Resource.Drawable.picker_icon, null), null);
            Control.CompoundDrawablePadding = 20;
            Control.TextSize = (float)_customPicker.FontSize;

            Control.SetPaddingRelative(5, 5, 5, 5);
            //Control.SetHintTextColor(Color.FromHex("#FFFFFF").ToAndroid());
            Control.Background = ResourcesCompat.GetDrawable(Resources, Resource.Drawable.control_selector, null);
        }

        protected override void Dispose(bool disposing)
        {
            _customPicker.IsValidChanged -= OnIsValidChanged;
            base.Dispose(disposing);
        }

        void OnIsValidChanged(object sender, System.EventArgs e)
        {
            if (_customPicker.IsValid != true)
            {
                Control.Background = ResourcesCompat.GetDrawable(Resources, Resource.Drawable.control_background_invalid, null);
            }
            else
            {
                Control.Background = ResourcesCompat.GetDrawable(Resources, Resource.Drawable.control_selector, null);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            //if (e.PropertyName == Picker.SelectedIndexProperty.PropertyName)
            //{
            //	if(_customPicker.SelectedIndex >= 0)
            //	{
            //		OnIsValidChanged = true;
            //	}
            //}

            if (e.PropertyName == Picker.IsFocusedProperty.PropertyName)
            {
                if (!_customPicker.IsFocused)
                {
                    if (_customPicker.SelectedIndex >= 0)
                    {
                        _customPicker.IsValid = true;
                    }
                    else
                    {
                        _customPicker.IsValid = false;
                    }
                }
            }
        }
    }
}