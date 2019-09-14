using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using TrainingDay.Views.Controls;
using TrainingDay.iOS.Services;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using ViewCellRenderer = Xamarin.Forms.Platform.iOS.ViewCellRenderer;

[assembly: ExportRenderer(typeof(DraggableListView), typeof(DragAndDropListViewRenderer))]
[assembly: ExportRenderer(typeof(DraggableViewCell), typeof(DragAndDropViewCellRenderer))]
namespace TrainingDay.iOS.Services
{
    public class DragAndDropViewCellRenderer : ViewCellRenderer
    {
        private static NSIndexPath sourceIndexPath, destinationIndexPath;
        private static UITableView sourceTableView;

        private class LongPressGestureRecognizer : UILongPressGestureRecognizer
        {
            private static UIView dragAndDropView;

            private WeakReference<UITableView> TableView { get; set; }

            private WeakReference<ViewCell> ViewCell { get; set; }



            public static LongPressGestureRecognizer CreateGesture(UITableView tableView, ViewCell cell)
            {
                return new LongPressGestureRecognizer(OnRecognizing)
                {
                    TableView = new WeakReference<UITableView>(tableView),
                    ViewCell = new WeakReference<ViewCell>(cell),
                };
            }

            private static void OnRecognizing(UILongPressGestureRecognizer r)
            {
                LongPressGestureRecognizer recognizer = r as LongPressGestureRecognizer;
                UITableView tableView;
                recognizer.TableView.TryGetTarget(out tableView);
                ViewCell cell;
                recognizer.ViewCell.TryGetTarget(out cell);
                if (tableView == null || cell == null)
                {
                    return;
                }
                OnRecognizing(recognizer, tableView, cell);
            }

            private static void OnRecognizing(LongPressGestureRecognizer recognizer, UITableView tableView, ViewCell cell)
            {
                NSIndexPath indexPath = tableView.IndexPathForRowAtPoint(recognizer.LocationInView(tableView));
                switch (recognizer.State)
                {
                    case UIGestureRecognizerState.Began:
                        tableView.ScrollEnabled = false;
                        if (indexPath != null)
                        {
                            // Remember the source row
                            sourceIndexPath = indexPath;
                            destinationIndexPath = indexPath;
                            var selectedCell = tableView.CellAt(indexPath);
                            UIGraphics.BeginImageContext(selectedCell.ContentView.Bounds.Size);
                            selectedCell.ContentView.Layer.RenderInContext(UIGraphics.GetCurrentContext());
                            UIImage img = UIGraphics.GetImageFromCurrentImageContext();
                            UIGraphics.EndImageContext();

                            UIImageView iv = new UIImageView(img);
                            dragAndDropView = new UIView();
                            dragAndDropView.Frame = iv.Frame;
                            dragAndDropView.Add(iv);
                            dragAndDropView.BackgroundColor = UIColor.Orange;
                            sourceTableView = tableView;

                            UIApplication.SharedApplication.KeyWindow.Add(dragAndDropView);

                            dragAndDropView.Center = recognizer.LocationInView(UIApplication.SharedApplication.KeyWindow);

                            dragAndDropView.AddGestureRecognizer(selectedCell.GestureRecognizers[0]);
                        }

                        break;
                    case UIGestureRecognizerState.Changed:
                        dragAndDropView.Center = recognizer.LocationInView(UIApplication.SharedApplication.KeyWindow);
                        destinationIndexPath = indexPath;
                        break;
                    case UIGestureRecognizerState.Cancelled:
                    case UIGestureRecognizerState.Failed:
                        sourceIndexPath = null;
                        cell.View.BackgroundColor = Color.Transparent;
                        break;
                    case UIGestureRecognizerState.Ended:

                        if (dragAndDropView == null)
                            return;

                        dragAndDropView.RemoveFromSuperview();
                        dragAndDropView = null;


                        UIView view = UIApplication.SharedApplication.KeyWindow;

                        UIView viewHit = view.HitTest(recognizer.LocationInView(view), null);

                        int removeLocation = (int)sourceIndexPath.Item;
                        int insertLocation = destinationIndexPath != null ? (int)destinationIndexPath.Item : -1;

                        UITableView destinationTableView = viewHit as UITableView;

                        if (viewHit is UITableView)
                        {
                            destinationTableView = viewHit as UITableView;

                        }
                        else
                        {
                            while (!(viewHit is UITableViewCell) && viewHit != null)
                            {
                                viewHit = viewHit.Superview;
                            }

                            UIView tempView = viewHit?.Superview;

                            while (!(tempView is UITableView) && tempView != null)
                            {
                                tempView = tempView.Superview;
                            }

                            if (tempView != null)
                            {
                                destinationTableView = tempView as UITableView;
                                insertLocation = (int)destinationTableView.IndexPathForCell((UITableViewCell)viewHit).Item;
                            }

                        }

                        if (destinationTableView != null)
                        {
                            if (DragAndDropListViewRenderer.ListMap.ContainsKey(tableView.Tag.ToString()) && DragAndDropListViewRenderer.ListMap.ContainsKey(destinationTableView.Tag.ToString()))
                            {
                                var sourceList = (IList)DragAndDropListViewRenderer.ListMap[tableView.Tag.ToString()].Item2;
                                var destinationList = (IList)DragAndDropListViewRenderer.ListMap[destinationTableView.Tag.ToString()].Item2;
                                if (!tableView.Tag.Equals(destinationTableView.Tag) || removeLocation != insertLocation)
                                {
                                    if (sourceList.Contains(cell.BindingContext))
                                    {
                                        sourceList.Remove(cell.BindingContext);

                                        if (insertLocation != -1)
                                            destinationList.Insert(insertLocation, cell.BindingContext);
                                        else
                                            destinationList.Add(cell.BindingContext);
                                    }
                                    tableView.ReloadData();
                                    destinationTableView.ReloadData();
                                }
                            }
                        }


                        tableView.ScrollEnabled = true;

                        break;
                }
            }

            private LongPressGestureRecognizer(Action<UILongPressGestureRecognizer> action) : base(action)
            {
            }
        }

        public override UITableViewCell GetCell(Cell item, UITableViewCell reusedCell, UITableView tableView)
        {
            UITableViewCell tableViewCell = base.GetCell(item, reusedCell, tableView);
            AddGestures(item as ViewCell, tableViewCell, tableView);
            return tableViewCell;
        }

        private void AddGestures(ViewCell cell, UITableViewCell tableViewCell, UITableView tableView)
        {
            tableViewCell.AddGestureRecognizer(LongPressGestureRecognizer.CreateGesture(tableView, cell));
        }

    }

    public class DragAndDropListViewRenderer : ListViewRenderer, IUIGestureRecognizerDelegate
    {

        internal static Dictionary<string, Tuple<UITableView, IList>> ListMap = new Dictionary<string, Tuple<UITableView, IList>>();

        private class ReorderableTableViewSource : UITableViewSource
        {
            public WeakReference<ListView> View { get; set; }

            public UITableViewSource Source { get; set; }

            #region A replacement UITableViewSource which enables drag and drop to reorder rows

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                return Source.GetCell(tableView, indexPath);
            }

            public override nfloat GetHeightForHeader(UITableView tableView, nint section)
            {
                return Source.GetHeightForHeader(tableView, section);
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                return Source.GetViewForHeader(tableView, section);
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return Source.NumberOfSections(tableView);
            }

            public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
            {
                Source.RowDeselected(tableView, indexPath);
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                Source.RowSelected(tableView, indexPath);
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return Source.RowsInSection(tableview, section);
            }

            public override string[] SectionIndexTitles(UITableView tableView)
            {
                return Source.SectionIndexTitles(tableView);
            }

            public override string TitleForHeader(UITableView tableView, nint section)
            {
                return Source.TitleForHeader(tableView, section);
            }

            #endregion

            public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
            {
                // Don't call the base method, which is the key: the same method in
                // ListViewRenderer.ListViewDataSource throws which prevents the rows to become moveable

                // TODO: do things to actually reorder in data model, such as raise a reorder event, etc.
            }
        }

        private new ListView Element => base.Element as ListView;
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {

            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                // Make row reorderable
                Control.Tag = (nint)Control.GetHashCode();

                ListMap.Add(Control.Tag.ToString(), new Tuple<UITableView, IList>(Control, (IList)this.Element.ItemsSource));

                Control.Source = new ReorderableTableViewSource { View = new WeakReference<ListView>(Element), Source = Control.Source };
            }


        }

        protected override void Dispose(bool disposing)
        {
            if (Control != null)
                ListMap.Remove(Control.Tag.ToString());

            base.Dispose(disposing);
        }

    }
}