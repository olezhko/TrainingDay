using System;
using System.ComponentModel;
using Android.Views;
using Com.Tomergoldst.Tooltips;
using TrainingDay.Droid.Services;
using TrainingDay.Services;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Com.Tomergoldst.Tooltips.ToolTipsManager;


[assembly: ResolutionGroupName("CrossGeeks")]
[assembly: ExportEffect(typeof(DroidTooltipEffect), nameof(TrainingDay.Services.TooltipEffect))]
namespace TrainingDay.Droid.Services
{
    public class DroidTooltipEffect : PlatformEffect
    {
        ToolTip toolTipView;
        ToolTipsManager _toolTipsManager;
        ITipListener listener;

        public DroidTooltipEffect()
        {
            listener = new TipListener();
            _toolTipsManager = new ToolTipsManager(listener);
        }

        void OnTap(object sender, EventArgs e)
        {
            var control = Control ?? Container;

            var text = TooltipEffect.GetText(Element);

            if (!string.IsNullOrEmpty(text))
            {
                ToolTip.Builder builder;
                var parentContent = control.RootView;

                var position = TooltipEffect.GetPosition(Element);
                switch (position)
                {
                    case TooltipPosition.Bottom:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.PositionBelow);
                        break;
                    case TooltipPosition.AlignCenter:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.AlignCenter);
                        break;
                    case TooltipPosition.AlignLeft:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.AlignLeft);
                        break;
                    case TooltipPosition.AlignRight:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.AlignRight);
                        break;
                    case TooltipPosition.GravityCenter:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.GravityCenter);
                        break;
                    case TooltipPosition.GravityLeft:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.GravityLeft);
                        break;
                    case TooltipPosition.GravityRight:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.GravityRight);
                        break;
                    case TooltipPosition.Top:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.PositionAbove);
                        break;
                    case TooltipPosition.Left:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.PositionLeftTo);
                        break;
                    case TooltipPosition.Right:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.PositionRightTo);
                        break;
                    default:
                        builder = new ToolTip.Builder(control.Context, control, parentContent as ViewGroup, text.PadRight(80, ' '), ToolTip.PositionBelow);
                        break;
                }

                builder.SetAlign(ToolTip.AlignLeft);
                builder.SetBackgroundColor(TooltipEffect.GetBackgroundColor(Element).ToAndroid());
                builder.SetTextColor(TooltipEffect.GetTextColor(Element).ToAndroid());

                toolTipView = builder.Build();
                _toolTipsManager?.Show(toolTipView);
            }
        }
 
        protected override void OnAttached()
        {
            var control = Control ?? Container;
            OnTap(control, null);
            //control.Click += OnTap;
        }

        protected override void OnDetached()
        {
            var control = Control ?? Container;
            //control.Click -= OnTap;
            _toolTipsManager.FindAndDismiss(control);
        }

        class TipListener : Java.Lang.Object, ITipListener
        {
            public void OnTipDismissed(Android.Views.View p0, int p1, bool p2)
            {

            }
        }
    }
}