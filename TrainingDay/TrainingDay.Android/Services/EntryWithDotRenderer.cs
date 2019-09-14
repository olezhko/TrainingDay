using Android.Content;
using Android.Text.Method;
using TrainingDay.Controls;
using TrainingDay.Droid.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(EntryWithDot), typeof(EntryWithDotRenderer))]
namespace TrainingDay.Droid.Services
{
    class EntryWithDotRenderer : EntryRenderer
    {
        public EntryWithDotRenderer(Context cont) : base(cont)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                //this.Control.KeyListener = DigitsKeyListener.GetInstance(true, true); // I know this is deprecated, but haven't had time to test the code without this line, I assume it will work without
                this.Control.InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagDecimal;
            }
        }
    }
}