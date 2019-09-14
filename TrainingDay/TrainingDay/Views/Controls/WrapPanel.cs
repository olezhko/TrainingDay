using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TrainingDay.Services;
using TrainingDay.ViewModels;
using Xamarin.Forms;

namespace TrainingDay.Views.Controls
{
    public class WrapPanel : Layout<Xamarin.Forms.View>
    {
        private Frame addButtonBrame;
        public WrapPanel()
        {
            _collectionChanged += OnCollectionChanged;
            picker.WidthRequest = 1;
            picker.HeightRequest = 1;
            picker.Items.RemoveAt(picker.Items.Count - 1);// remove last Chouse
            picker.SelectedIndexChanged += Picker_SelectedIndexChanged;
            picker.Unfocused += Picker_Unfocused;

            AddButton = new Label()
            {
                TextColor = Color.Black,
                BackgroundColor = Color.Transparent,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            var tap = new TapGestureRecognizer();
            tap.Tapped += AddButton_Tapped;
            addButtonBrame = new Frame() { CornerRadius = 5, BorderColor = Color.Gray, Padding = new Thickness(2, 5) , Margin = 0};
            addButtonBrame.Content = AddButton;
            addButtonBrame.GestureRecognizers.Add(tap);
        }


        #region Collection
        private static void ItemsSource_OnPropertyChanged(BindableObject bindable, IEnumerable oldvalue, IEnumerable newvalue)
        {
            if (oldvalue != null)
            {
                var coll = (INotifyCollectionChanged)oldvalue;
                coll.CollectionChanged -= ItemsSource_OnItemChanged;
                var co = bindable as WrapPanel;
                co?.RemoveItems();
            }

            if (newvalue != null)
            {
                var coll = newvalue as INotifyCollectionChanged;
                coll.CollectionChanged += ItemsSource_OnItemChanged;
                var co = bindable as WrapPanel;
                co?.AddItems(co.ItemsSource);
            }
        }

        private void RemoveItems()
        {
            Children.Clear();
            if (IsEditableItems)
            {
                Children.Add(addButtonBrame);
                Children.Add(picker);
            }
        }

        private void RemoveItems(IList oldItems)
        {

        }

        private void AddItems(IEnumerable coll)
        {
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!AddItems(IEnumerable coll)");

            if (coll is ObservableCollection<MuscleViewModel> a)
            {
                if (a.Count == 0)
                {
                    if (IsEditableItems)
                        IsVisible = true;
                    else
                        IsVisible = false;
                    return;
                }
            }
            foreach (object item in coll)
            {
                var child = ItemTemplate.CreateContent() as Xamarin.Forms.View;
                if (child == null)
                    return;

                var tap = new TapGestureRecognizer();
                tap.Tapped += Tap_Tapped;
                child.GestureRecognizers.Add(tap);
                child.BindingContext = item;
                Children.Insert(Children.Count - 1, child);
            }
        }


        private static event EventHandler<NotifyCollectionChangedEventArgs> _collectionChanged;
        private static void ItemsSource_OnItemChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _collectionChanged?.Invoke(null, e);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            //if (args.NewItems != null) AddItems(args.NewItems);
            //if (args.OldItems != null) RemoveItems(args.OldItems);
            RemoveItems();
            AddItems(ItemsSource);
        }
        #endregion

        #region DrawItems
        /// <summary>
        /// This is called when the spacing or orientation properties are changed - it forces
        /// the control to go back through a layout pass.
        /// </summary>
        private void OnSizeChanged()
        {
            ForceLayout();
        }

        /// <summary>
        /// This method is called during the measure pass of a layout cycle to get the desired size of an element.
        /// </summary>
        /// <param name="widthConstraint">The available width for the element to use.</param>
        /// <param name="heightConstraint">The available height for the element to use.</param>
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (WidthRequest > 0)
                widthConstraint = Math.Min(widthConstraint, WidthRequest);
            if (HeightRequest > 0)
                heightConstraint = Math.Min(heightConstraint, HeightRequest);

            double internalWidth = double.IsPositiveInfinity(widthConstraint) ? double.PositiveInfinity : Math.Max(0, widthConstraint);
            double internalHeight = double.IsPositiveInfinity(heightConstraint) ? double.PositiveInfinity : Math.Max(0, heightConstraint);

            return Orientation == StackOrientation.Vertical
                ? DoVerticalMeasure(internalWidth, internalHeight)
                    : DoHorizontalMeasure(internalWidth, internalHeight);

        }

        /// <summary>
        /// Does the vertical measure.
        /// </summary>
        /// <returns>The vertical measure.</returns>
        /// <param name="widthConstraint">Width constraint.</param>
        /// <param name="heightConstraint">Height constraint.</param>
        private SizeRequest DoVerticalMeasure(double widthConstraint, double heightConstraint)
        {
            int columnCount = 1;

            double width = 0;
            double height = 0;
            double minWidth = 0;
            double minHeight = 0;
            double heightUsed = 0;

            foreach (var item in Children)
            {
                var size = item.Measure(widthConstraint, heightConstraint);
                width = Math.Max(width, size.Request.Width);

                var newHeight = height + size.Request.Height + Spacing;
                if (newHeight > heightConstraint)
                {
                    columnCount++;
                    heightUsed = Math.Max(height, heightUsed);
                    height = size.Request.Height;
                }
                else
                    height = newHeight;

                minHeight = Math.Max(minHeight, size.Minimum.Height);
                minWidth = Math.Max(minWidth, size.Minimum.Width);
            }

            if (columnCount > 1)
            {
                height = Math.Max(height, heightUsed);
                width *= columnCount;  // take max width
            }

            return new SizeRequest(new Size(width, height), new Size(minWidth, minHeight));
        }

        /// <summary>
        /// Does the horizontal measure.
        /// </summary>
        /// <returns>The horizontal measure.</returns>
        /// <param name="widthConstraint">Width constraint.</param>
        /// <param name="heightConstraint">Height constraint.</param>
        private SizeRequest DoHorizontalMeasure(double widthConstraint, double heightConstraint)
        {
            int rowCount = 1;

            double width = 0;
            double height = 0;
            double minWidth = 0;
            double minHeight = 0;
            double widthUsed = 0;

            foreach (var item in Children)
            {
                var size = item.Measure(widthConstraint, heightConstraint);
                if (height == 0)
                {
                    height = Math.Max(height, size.Request.Height);
                }

                var newWidth = width + size.Request.Width + Spacing;
                if (newWidth > widthConstraint)
                {
                    rowCount++;
                    widthUsed = Math.Max(width, widthUsed);
                    width = size.Request.Width;
                }
                else
                    width = newWidth;

                minHeight = Math.Max(minHeight, size.Minimum.Height);
                minWidth = Math.Max(minWidth, size.Minimum.Width);
            }

            if (rowCount > 1)
            {
                width = Math.Max(width, widthUsed);
                height = (height + Spacing) * rowCount - Spacing; // via MitchMilam 
            }
            //if (addButtonBrame!=null)
            //{
            //    addButtonBrame.HeightRequest = height;
            //    addButtonBrame.WidthRequest = height;
            //}

            return new SizeRequest(new Size(width, height), new Size(minWidth, minHeight));
        }

        /// <summary>
        /// Positions and sizes the children of a Layout.
        /// </summary>
        /// <param name="x">A value representing the x coordinate of the child region bounding box.</param>
        /// <param name="y">A value representing the y coordinate of the child region bounding box.</param>
        /// <param name="width">A value representing the width of the child region bounding box.</param>
        /// <param name="height">A value representing the height of the child region bounding box.</param>
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            if (Orientation == StackOrientation.Vertical)
            {
                double colWidth = 0;
                double yPos = y, xPos = x;

                foreach (var child in Children.Where(c => c.IsVisible))
                {
                    var request = child.Measure(width, height);

                    double childWidth = request.Request.Width;
                    double childHeight = request.Request.Height;
                    colWidth = Math.Max(colWidth, childWidth);

                    if (yPos + childHeight > height)
                    {
                        yPos = y;
                        xPos += colWidth + Spacing;
                        colWidth = 0;
                    }

                    var region = new Rectangle(xPos, yPos, childWidth, childHeight);
                    LayoutChildIntoBoundingRegion(child, region);
                    yPos += region.Height + Spacing;
                }
            }
            else
            {
                double rowHeight = 0;
                double yPos = y, xPos = x;

                foreach (var child in Children.Where(c => c.IsVisible))
                {
                    //var request = child.GetSizeRequest(width, height);
                    var request = child.Measure(width, height);

                    double childWidth = request.Request.Width;
                    double childHeight = request.Request.Height;
                    rowHeight = Math.Max(rowHeight, childHeight);
                    if (xPos + childWidth > width)
                    {
                        xPos = x;
                        yPos += rowHeight + Spacing;
                        rowHeight = 0;
                    }

                    var region = new Rectangle(xPos, yPos, childWidth, childHeight);
                    LayoutChildIntoBoundingRegion(child, region);
                    xPos += region.Width + Spacing;
                }
            }
        }


        #endregion

        private Label AddButton;
        private EnumBindablePicker<MusclesEnum> picker = new EnumBindablePicker<MusclesEnum>();
        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (picker.SelectedIndex >= 0)
            {
                var newMuscle = new MuscleViewModel()
                {
                    Id = picker.SelectedIndex,
                    Color = MusclesConverter.Colors[picker.SelectedIndex],
                    Name = EnumBindablePicker<MusclesEnum>.GetEnumDescription(picker.SelectedItem)
                };

                var items = (ObservableCollection<MuscleViewModel>)(ItemsSource as IList);
                if (!items.Any(a => a.Id == newMuscle.Id))
                {
                    items.Add(newMuscle);
                }
            }
        }

        private void Picker_Unfocused(object sender, FocusEventArgs e)
        {
            picker.IsVisible = false;
        }

        private void AddButton_Tapped(object sender, EventArgs e)
        {
            picker.IsVisible = true;
            picker.Focus();
        }

        private void Tap_Tapped(object sender, EventArgs e)
        {
            if (IsEditableItems)
            {
                try
                {
                    var elem = (Xamarin.Forms.View)sender;
                    var index = Children.IndexOf(elem);
                    Children.Remove(elem);
                    (ItemsSource as IList).RemoveAt(index - 1);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        public string AddContent
        {
            get => AddButton.Text;
            set => AddButton.Text = value;
        }

        #region DepProp CallBacks
        private void OnIsEditableItemsChanged()
        {
            if (IsEditableItems)
            {
                try
                {
                    Children.Add(addButtonBrame);
                    Children.Add(picker);
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
            else
            {
                try
                {
                    Children.Remove(addButtonBrame);
                    Children.Remove(picker);
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }


        #endregion

        #region Dep

        public static BindableProperty IsEditableItemsProperty = BindableProperty.Create(nameof(IsEditableItems), typeof(bool), typeof(WrapPanel), default(bool), propertyChanged: (bindable, oldvalue, newvalue) => ((WrapPanel)bindable).OnIsEditableItemsChanged(), defaultBindingMode: BindingMode.TwoWay);
        public bool IsEditableItems
        {
            get { return (bool)GetValue(IsEditableItemsProperty); }
            set { SetValue(IsEditableItemsProperty, value); }
        }


        public static BindableProperty OrientationProperty = BindableProperty.Create(nameof(Orientation), typeof(StackOrientation), typeof(WrapPanel), StackOrientation.Vertical, propertyChanged: (bindable, oldvalue, newvalue) => ((WrapPanel)bindable).OnSizeChanged(), defaultBindingMode: BindingMode.TwoWay);
        /// <summary>
        /// Orientation (Horizontal or Vertical)
        /// </summary>
        public StackOrientation Orientation
        {
            get { return (StackOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static BindableProperty SpacingProperty = BindableProperty.Create(nameof(Spacing), typeof(double), typeof(WrapPanel), 6.0, propertyChanged: (bindable, oldvalue, newvalue) => ((WrapPanel)bindable).OnSizeChanged(), defaultBindingMode: BindingMode.TwoWay);
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(WrapPanel), null, propertyChanged: (bindable, oldvalue, newvalue) => ((WrapPanel)bindable).OnSizeChanged(), defaultBindingMode: BindingMode.TwoWay);
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(WrapPanel), null, propertyChanged: (bindable, oldvalue, newvalue) => ItemsSource_OnPropertyChanged(bindable, (IEnumerable)oldvalue, (IEnumerable)newvalue), defaultBindingMode: BindingMode.TwoWay);
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        #endregion
    }
}
