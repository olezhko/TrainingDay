using System;
using System.Collections.Generic;
using System.Text;
using TrainingDay.Resources;
using Xamarin.Forms;

namespace TrainingDay.Views.Controls
{
    //https://github.com/johnshardman/XF_DragAndDrop
    public class DraggableListView : ListView
    {
        public static readonly BindableProperty IsDragEnabledProperty = BindableProperty.Create("IsDragEnabled", typeof(bool), typeof(DraggableListView), false, BindingMode.TwoWay);

        public bool IsDragEnabled
        {
            get => (bool)GetValue(IsDragEnabledProperty);
            set => SetValue(IsDragEnabledProperty, value);
        }

        protected override void SetupContent(Cell content, int index)
        {
            base.SetupContent(content, index);

            var viewCell = content as ViewCell;
            viewCell.View.BackgroundColor = (Color)Application.Current.Resources["ContentPageBackgroundColor"];
        }
    }

    public class DraggableViewCell : ViewCell
    {

    }
}
