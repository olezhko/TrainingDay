﻿using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using Xamarin.Forms;
using TrainingDay.Services;

namespace myToolTipSample
{
    /// <summary>
    /// Cross myToolTip
    /// </summary>
    public static class ToolTipEffect
    {

        public static readonly BindableProperty TextProperty = BindableProperty.CreateAttached("Text", typeof(string), typeof(ToolTipEffect), string.Empty);

        public static readonly BindableProperty PositionProperty = BindableProperty.CreateAttached("Position", typeof(ToolTipPosition), typeof(ToolTipEffect), ToolTipPosition.Bottom);

        public static readonly BindableProperty IsTapAttachedProperty = BindableProperty.CreateAttached("IsTapAttached", typeof(bool), typeof(ToolTipEffect), false, propertyChanged: OnIsTapAttachedChanged);

        public static readonly BindableProperty TextColorProperty = BindableProperty.CreateAttached("TextColor", typeof(Color), typeof(ToolTipEffect), Color.Black);

        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.CreateAttached("BackgroundColor", typeof(Color), typeof(ToolTipEffect), Color.Silver);

        public static readonly BindableProperty IsOpenProperty = BindableProperty.CreateAttached("IsOpen", typeof(bool), typeof(ToolTipEffect), false, propertyChanged: IsOpenChanged);

        private static void IsOpenChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view == null)
            {
                return;
            }

            bool isOpen = (bool)newValue;
            if (isOpen)
            {
                view.Effects.Add(new ControlTooltipEffect());
            }
            else
            {
                var toRemove = view.Effects.FirstOrDefault(e => e is ControlTooltipEffect);
                if (toRemove != null)
                {
                    view.Effects.Remove(toRemove);
                }
            }
        }

        public static bool GetIsOpen(BindableObject view)
        {
            return (bool)view.GetValue(IsOpenProperty);
        }

        public static void SetIsOpen(BindableObject view, bool value)
        {
            if (!Settings.IsToolTipShow)
                return;
            view.SetValue(IsOpenProperty, value);
        }


        #region New Logic

        //public static readonly BindableProperty IsShownProperty = BindableProperty.CreateAttached("IsShown", typeof(bool), typeof(ToolTipEffect), false, propertyChanged: IsShownChanged);


        #endregion




        #region AndroidIOSOnly

        /// <summary>
        /// Android Only
        /// </summary>
        public static readonly BindableProperty PaddingProperty = BindableProperty.CreateAttached("Padding", typeof(int), typeof(ToolTipEffect), default);

        /// <summary>
        /// Android, IOS
        /// </summary>
        public static readonly BindableProperty ArrowHeightProperty = BindableProperty.CreateAttached("ArrowHeight", typeof(double), typeof(ToolTipEffect), default);

        /// <summary>
        /// Android, IOS
        /// </summary>
        public static readonly BindableProperty ArrowWidthProperty = BindableProperty.CreateAttached("ArrowWidth", typeof(double), typeof(ToolTipEffect), default);
        #endregion

        #region AndroidOnly

        /// <summary>
        /// Android Only
        /// </summary>
        public static readonly BindableProperty TextSizeProperty = BindableProperty.CreateAttached("TextSize", typeof(double), typeof(ToolTipEffect), default);

        /// <summary>
        /// Android Only
        /// </summary>
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.CreateAttached("CornerRadius", typeof(double), typeof(ToolTipEffect), 5.0);

        /// <summary>
        /// Android Only
        /// </summary>
        public static readonly BindableProperty MarginProperty = BindableProperty.CreateAttached("Margin", typeof(double), typeof(ToolTipEffect), 10.0);
        #endregion

        #region UWPOnly
        /// <summary>
        /// Overrides TextProperty when it is set. UWP only, IOS and Android not implemented yet
        /// </summary>
        public static readonly BindableProperty ContentProperty = BindableProperty.Create("Content", typeof(View), typeof(ToolTipEffect), (object)null, (BindingMode)0, (BindableProperty.ValidateValueDelegate)null, (BindableProperty.BindingPropertyChangedDelegate)null, (BindableProperty.BindingPropertyChangingDelegate)null, (BindableProperty.CoerceValueDelegate)null, (BindableProperty.CreateDefaultValueDelegate)null);


        /// <summary>
        /// Sets the Height of the toolTip. UWP only, IOS and Android not implemented yet
        /// </summary>
        public static readonly BindableProperty HeightProperty = BindableProperty.CreateAttached("Height", typeof(double), typeof(ToolTipEffect), default);

        /// <summary>
        /// Sets the Width of the toolTip. UWP only, IOS and Android not implemented yet
        /// </summary>
        public static readonly BindableProperty WidthProperty = BindableProperty.CreateAttached("Width", typeof(double), typeof(ToolTipEffect), default);

        #endregion

        public static double GetHeight(BindableObject view)
        {
            return (double)view.GetValue(HeightProperty);
        }
        public static double GetWidth(BindableObject view)
        {
            return (double)view.GetValue(WidthProperty);
        }

        public static void SetHeight(BindableObject view, double value)
        {
            view.SetValue(HeightProperty, value);
        }

        public static void SetWidth(BindableObject view, double value)
        {
            view.SetValue(WidthProperty, value);
        }

        public static double GetTextSize(BindableObject view)
        {
            return (double)view.GetValue(TextSizeProperty);
        }

        public static void SetTextSize(BindableObject view, double value)
        {
            view.SetValue(TextSizeProperty, value);
        }

        public static double GetArrowHeight(BindableObject view)
        {
            return (double)view.GetValue(ArrowHeightProperty);
        }

        public static void SetArrowHeight(BindableObject view, double value)
        {
            view.SetValue(ArrowHeightProperty, value);
        }

        public static double GetArrowWidth(BindableObject view)
        {
            return (double)view.GetValue(ArrowWidthProperty);
        }

        public static void SetArrowWidth(BindableObject view, double value)
        {
            view.SetValue(ArrowWidthProperty, value);
        }


        //public static readonly BindableProperty ActionProperty = BindableProperty.CreateAttached("Action", typeof(Action), typeof(ToolTipEffect), Action.OnHover);

        /// <summary>
        /// Gets or sets the value of the Content. This property can be used to change the content in a tab header.
        /// </summary>

        public static View GetContent(BindableObject view)
        {
            return (View)view.GetValue(ContentProperty);
        }

        public static double GetCornerRadius(BindableObject view)
        {
            return (double)view.GetValue(CornerRadiusProperty);
        }

        public static double GetMargin(BindableObject view)
        {
            return (double)view.GetValue(MarginProperty);
        }

        public static void SetMargin(BindableObject view, int value)
        {
            view.SetValue(MarginProperty, value);
        }

        public static int GetPadding(BindableObject view)
        {
            return (int)view.GetValue(PaddingProperty);
        }

        public static void SetPadding(BindableObject view, int value)
        {
            view.SetValue(PaddingProperty, value);
        }

        internal static float GetLineSpacing(Element element)
        {
            throw new NotImplementedException();
        }

        public static void SetCornerRadius(BindableObject view, float value)
        {
            view.SetValue(CornerRadiusProperty, value);
        }

        public static void SetContent(BindableObject view, View value)
        {
            view.SetValue(ContentProperty, value);
        }

        public static bool GetIsTapAttached(BindableObject view)
        {
            return (bool)view.GetValue(IsTapAttachedProperty);
        }

        public static void SetIsTapAttached(BindableObject view, bool value)
        {
            view.SetValue(IsTapAttachedProperty, value);
        }

        public static string GetText(BindableObject view)
        {
            return (string)view.GetValue(TextProperty);
        }

        public static Color GetTextColor(BindableObject view)
        {
            return (Color)view.GetValue(TextColorProperty);
        }

        public static void SetTextColor(BindableObject view, Color value)
        {
            view.SetValue(TextColorProperty, value);
        }

        public static Color GetBackgroundColor(BindableObject view)
        {
            return (Color)view.GetValue(BackgroundColorProperty);
        }

        public static void SetBackgroundColor(BindableObject view, Color value)
        {
            view.SetValue(BackgroundColorProperty, value);
        }


        public static void SetText(BindableObject view, string value)
        {
            view.SetValue(TextProperty, value);
        }

        public static ToolTipPosition GetPosition(BindableObject view)
        {
            return (ToolTipPosition)view.GetValue(PositionProperty);
        }

        public static void SetPosition(BindableObject view, ToolTipPosition value)
        {
            view.SetValue(PositionProperty, value);
        }

        //public static Action GetAction(BindableObject view)
        //{
        //    return (Action)view.GetValue(ActionProperty);
        //}

        //public static void SetAction(BindableObject view, Action value)
        //{
        //    view.SetValue(ActionProperty, value);
        //}

        static void OnIsTapAttachedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as View;
            if (view == null)
            {
                return;
            }

            bool isVisible = (bool)newValue;
            if (isVisible)
            {
                view.Effects.Add(new ControlTooltipEffect());
            }
            else
            {
                var toRemove = view.Effects.FirstOrDefault(e => e is ControlTooltipEffect);
                if (toRemove != null)
                {
                    view.Effects.Remove(toRemove);
                }
            }
        }
    }

    public enum ToolTipPosition
    {
        Bottom,

        Right,

        Left,

        Top
    }

    public enum Action
    {
        OnHover,

        OnClick,
    }

    class ControlTooltipEffect : RoutingEffect
    {
        public ControlTooltipEffect() : base($"Plugin.{nameof(ToolTipEffect)}")
        {

        }
    }
}

