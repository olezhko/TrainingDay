using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TrainingDay.Droid.Services.DragListView;
using TrainingDay.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(DraggableViewCell), typeof(DragAndDropViewCellRenderer))]
namespace TrainingDay.Droid.Services.DragListView
{
    public class DragAndDropViewCellRenderer : ViewCellRenderer
    {
        protected override global::Android.Views.View GetCellCore(Xamarin.Forms.Cell item, global::Android.Views.View convertView, global::Android.Views.ViewGroup parent, global::Android.Content.Context context)
        {
            var cell = base.GetCellCore(item, convertView, parent, context) as ViewGroup;

            cell.SetOnDragListener(new ItemDragListener(cell));
            return cell;
        }
    }
}