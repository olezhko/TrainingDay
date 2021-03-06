﻿using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xamarin.Forms;
using System.Linq;
using TrainingDay.Resources;
using TrainingDay.ViewModels;

namespace TrainingDay.Views.Controls
{
    public class StepProgressBar : StackLayout
    {
        #region Dep
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(StepProgressBar), null, propertyChanged: ItemsSource_OnPropertyChanged);

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(StepProgressBar), null);

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly BindableProperty UnselectedIndicatorProperty = BindableProperty.Create(nameof(DeselectElementImage), typeof(string), typeof(StepProgressBar), "", BindingMode.OneWay);
        public string DeselectElementImage
        {
            get { return (string)this.GetValue(UnselectedIndicatorProperty); }
            set { this.SetValue(UnselectedIndicatorProperty, value); }
        }

        public static readonly BindableProperty StepSelectedProperty = BindableProperty.Create(nameof(StepSelected), typeof(int), typeof(StepProgressBar), 0, defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty StepColorProperty = BindableProperty.Create(nameof(StepColor), typeof(Xamarin.Forms.Color), typeof(StepProgressBar), Color.Black, defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty SteppedColorProperty = BindableProperty.Create(nameof(SteppedColor), typeof(Xamarin.Forms.Color), typeof(StepProgressBar), Color.Black, defaultBindingMode: BindingMode.TwoWay);
        public static readonly BindableProperty StepCanTouchProperty = BindableProperty.Create(nameof(StepCanTouch), typeof(bool), typeof(StepProgressBar), true);

        public Color StepColor
        {
            get { return (Color)GetValue(StepColorProperty); }
            set { SetValue(StepColorProperty, value); }
        }

        public Color SteppedColor
        {
            get { return (Color)GetValue(SteppedColorProperty); }
            set { SetValue(SteppedColorProperty, value); }
        }

        public int StepSelected
        {
            get { return (int)GetValue(StepSelectedProperty); }
            set { SetValue(StepSelectedProperty, value); }
        }

        public bool StepCanTouch
        {
            get { return (bool)GetValue(StepCanTouchProperty); }
            set { SetValue(StepCanTouchProperty, value); }
        }


        #endregion

        public StepProgressBar()
        {
            _collectionChanged += OnCollectionChanged;
            Orientation = StackOrientation.Vertical;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            Spacing = 0;
            AddStyles();
            headersStackLayout = new StackLayout();
            headersStackLayout.Orientation = StackOrientation.Horizontal;
            headersStackLayout.VerticalOptions = LayoutOptions.Start;
            headersStackLayout.Padding = new Thickness(0,10);

            var scroll = new ScrollView();
            scroll.Orientation = ScrollOrientation.Horizontal;
            scroll.HorizontalOptions = LayoutOptions.FillAndExpand;
            scroll.Content = headersStackLayout;
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Always;
            Children.Add(scroll);
        }

        #region Draw
        private StackLayout headersStackLayout;
        private Xamarin.Forms.View element;
        private void SetTemplateElement()
        {
            element = ItemTemplate.CreateContent() as Xamarin.Forms.View;
            Children.Add(element);
        }

        void AddStyles()
        {
            var unselectedCircleStyle = new Style(typeof(Button))
            {
                Setters = {
                    new Setter { Property = BackgroundColorProperty,   Value = Color.Transparent },
                    new Setter { Property = Button.BorderColorProperty,   Value = StepColor },
                    new Setter { Property = Button.TextColorProperty,   Value = StepColor },
                    new Setter { Property = Button.BorderWidthProperty,   Value = 0.5 },
                    new Setter { Property = HeightRequestProperty,   Value = 45 },
                    new Setter { Property = WidthRequestProperty,   Value = 45 },
                    new Setter { Property = PaddingProperty,   Value = new Thickness(0) },
            }
            };

            var selectedCircleStyle = new Style(typeof(Button))
            {
                Setters = {
                    new Setter { Property = BackgroundColorProperty, Value = SteppedColor },
                    new Setter { Property = Button.FontAttributesProperty,   Value = FontAttributes.Bold },

                    new Setter { Property = Button.TextColorProperty, Value = Color.White },
                    new Setter { Property = Button.BorderColorProperty, Value = StepColor },
                    new Setter { Property = Button.BorderWidthProperty,   Value = 0.5 },
                    new Setter { Property = HeightRequestProperty,   Value = 45 },
                    new Setter { Property = WidthRequestProperty,   Value = 45 },
                    new Setter { Property = PaddingProperty,   Value = new Thickness(0) },
            }
            };

            Resources = new ResourceDictionary();
            Resources.Add("unselectedCircleStyle", unselectedCircleStyle);
            Resources.Add("selectedCircleStyle", selectedCircleStyle);
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == ItemsSourceProperty.PropertyName)
            {
                var list = ItemsSource as IList;
                for (int i = 0; i < list.Count; i++)
                {
                    var button = new Button()
                    {
                        Text = $"{i + 1}",
                        ClassId = $"{i + 1}",
                        Style = Resources["unselectedCircleStyle"] as Style,
                        CornerRadius = 20
                    };

                    button.Clicked -= Handle_Clicked;
                    button.Clicked += Handle_Clicked;

                    headersStackLayout.Children.Add(button);

                    if (i < list.Count - 1)
                    {
                        var separatorLine = new BoxView()
                        {
                            BackgroundColor = Color.Silver,
                            HeightRequest = 1,
                            WidthRequest = 5,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            Margin = new Thickness(0)
                        };
                        headersStackLayout.Children.Add(separatorLine);
                    }
                }
            }

            //if (propertyName == StepSelectedProperty.PropertyName)
            //{
            //    var children = headersStackLayout.Children.FirstOrDefault(p => (!string.IsNullOrEmpty(p.ClassId) && Convert.ToInt32(p.ClassId) == StepSelected));
            //    if (children != null) SelectElement(children as Button);
            //}

            if (propertyName == StepColorProperty.PropertyName)
            {
                AddStyles();
            }
        }

        #endregion

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (StepCanTouch)
                SelectElement(sender as Button);
        }

        Button _lastStepSelected;
        void SelectElement(Button elementSelected)
        {
            var list = ItemsSource as IList;

            if (_lastStepSelected != null)
                _lastStepSelected.Style = Resources["unselectedCircleStyle"] as Style;

            elementSelected.Style = Resources["selectedCircleStyle"] as Style;

            var index = Convert.ToInt32(elementSelected.Text) - 1;

            StepSelected = index;
            if (StepSelected >= 0)
                element.BindingContext = list[StepSelected];

            _lastStepSelected = elementSelected;
        }

        public void SelectElement(int index)
        {
            var children = headersStackLayout.Children.FirstOrDefault(p => (Convert.ToInt32(p.ClassId) == index + 1)) as Button;
            if (children != null)
            {
                SelectElement(children);
            }
        }

        public void DeselectElement()
        {
            var children = headersStackLayout.Children.FirstOrDefault(p => (Convert.ToInt32(p.ClassId) == StepSelected + 1)) as Button;
            if (children != null)
            {
                children.BackgroundColor = Color.LightGreen;
            }
        }
        public void DeSkipElement()
        {
            var children = headersStackLayout.Children.FirstOrDefault(p => (Convert.ToInt32(p.ClassId) == StepSelected + 1)) as Button;
            if (children != null)
            {
                children.BackgroundColor = Color.Transparent;
            }
        }
        public void SkipElement()
        {
            var children = headersStackLayout.Children.FirstOrDefault(p => (Convert.ToInt32(p.ClassId) == StepSelected + 1)) as Button;
            if (children != null)
            {
                children.BackgroundColor = Color.DimGray;
            }
        }

        private static void ItemsSource_OnPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (newvalue != null)
            {
                var co = bindable as StepProgressBar;
                if (co != null)
                {
                    co.SetTemplateElement();
                    co.SelectElement(co.StepSelected);
                }

                var coll = newvalue as INotifyCollectionChanged;
                // Subscribe to CollectionChanged on the new collection
                coll.CollectionChanged += ItemsSource_OnItemChanged;
            }

            if (oldvalue!=null)
            {
                var coll = (INotifyCollectionChanged)oldvalue;
                // Unsubscribe from CollectionChanged on the old collection
                coll.CollectionChanged -= ItemsSource_OnItemChanged;
            }
        }

        public void NextElement()
        {
            SelectElement(StepSelected + 1);
        }

        public void NextElement(int index)
        {
            SelectElement(index);
        }


        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null) AddItems(args.NewItems);
            //if (args.OldItems != null) RemoveItems(args.OldItems);
        }

        private void AddItems(IList argsNewItems)
        {
            var count = (ItemsSource as IList).Count;

            if (count - argsNewItems.Count !=0)
            {
                AddSeparatorLine();
            }

            for (int i = 0; i < argsNewItems.Count; i++)
            {
                var button = new Button()
                {
                    Text = $"{i + count}",
                    ClassId = $"{i + count}",
                    Style = Resources["unselectedCircleStyle"] as Style,
                    CornerRadius = 20
                };

                button.Clicked -= Handle_Clicked;
                button.Clicked += Handle_Clicked;

                headersStackLayout.Children.Add(button);
                if (i < argsNewItems.Count - 1)
                {
                    var separatorLine = new BoxView()
                    {
                        BackgroundColor = Color.Silver,
                        HeightRequest = 1,
                        WidthRequest = 5,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Margin = new Thickness(0)
                    };
                    headersStackLayout.Children.Add(separatorLine);
                }
            }
        }

        private void AddSeparatorLine()
        {
            var separatorLine = new BoxView()
            {
                BackgroundColor = Color.Silver,
                HeightRequest = 1,
                WidthRequest = 5,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(0)
            };
            headersStackLayout.Children.Add(separatorLine);
        }

        private static event EventHandler<NotifyCollectionChangedEventArgs> _collectionChanged;
        private static void ItemsSource_OnItemChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _collectionChanged?.Invoke(null, e);
        }
    }
}
