using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Com.Tooltip;
using Java.Util;
using myToolTipSample;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("Plugin")]
[assembly: ExportEffect(typeof(TrainingDay.Droid.Services.myToolTipImplementation), nameof(myToolTipSample.ToolTipEffect))]
namespace TrainingDay.Droid.Services
{
    public class myToolTipImplementation : PlatformEffect
    {
        Tooltip.Builder builder;

        //void OnTap(object sender, EventArgs e)
        //{
        //   // GetToolTip();
        //}

        private void GetToolTip()
        {
            var control = Control ?? Container;

            var text = ToolTipEffect.GetText(Element);

            if (!string.IsNullOrEmpty(text))
            {

                var position = ToolTipEffect.GetPosition(Element);
                builder = new Tooltip.Builder(control);
                switch (position)
                {
                    case ToolTipPosition.Top:
                        builder.SetGravity((int)GravityFlags.Top);
                        break;
                    case ToolTipPosition.Left:
                        builder.SetGravity((int)GravityFlags.Left);
                        break;
                    case ToolTipPosition.Right:
                        builder.SetGravity((int)GravityFlags.Right);
                        break;
                    case ToolTipPosition.Bottom:
                        builder.SetGravity((int)GravityFlags.Bottom);
                        break;
                    default:
                        builder.SetGravity((int)GravityFlags.Bottom);
                        break;
                }


                builder.SetText(text);
                builder.SetCornerRadius(Convert.ToSingle(ToolTipEffect.GetCornerRadius(Element)));

                builder.SetDismissOnClick(true);
                builder.SetBackgroundColor(ToolTipEffect.GetBackgroundColor(Element).ToAndroid());
                builder.SetTextColor(ToolTipEffect.GetTextColor(Element).ToAndroid());
                var heightArrow = ToolTipEffect.GetArrowHeight(Element);
                if (heightArrow > 0.0)
                    builder.SetArrowHeight(Convert.ToSingle(heightArrow));
                var widthArrow = ToolTipEffect.GetArrowWidth(Element);
                if (widthArrow > 0.0)
                    builder.SetArrowWidth(Convert.ToSingle(widthArrow));

                var textSize = ToolTipEffect.GetTextSize(Element);
                if (textSize > 0)
                    builder.SetTextSize(Convert.ToSingle(textSize));

                builder.SetMargin(Convert.ToSingle(ToolTipEffect.GetMargin(Element)));
                builder.SetPadding(ToolTipEffect.GetPadding(Element));
                builder.SetCancelable(true);

                builder.Build().Show();

                //  _toolTipsManager?.Show(toolTipView);
            }
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;
            //control.Click += OnTap;
        }

        protected override void OnDetached()
        {
            var control = Control ?? Container;
            //control.Click -= OnTap;
            builder?.Dispose();
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
            if (args.PropertyName == "IsOpen")
            {
                if (ToolTipEffect.GetIsOpen(Element))
                {
                    GetToolTip();
                }
                else
                {
                    Hide();
                }
            }
        }

        private void Hide()
        {
            //builder.d
        }
    }
}