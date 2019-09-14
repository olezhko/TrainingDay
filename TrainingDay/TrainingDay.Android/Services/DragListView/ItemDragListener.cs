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

namespace TrainingDay.Droid.Services.DragListView
{
    public class ItemDragListener : Java.Lang.Object, Android.Views.View.IOnDragListener
    {
        Android.Views.View associatedView;

        public ItemDragListener(Android.Views.View view)
        {
            associatedView = view;
        }

        #region IOnDragListener implementation
        public bool OnDrag(Android.Views.View v, Android.Views.DragEvent e)
        {
            switch (e.Action)
            {
                case Android.Views.DragAction.Started:
                    break;
                case Android.Views.DragAction.Entered:
                    v.SetBackgroundColor(Android.Graphics.Color.Orange);
                    break;
                case Android.Views.DragAction.Exited:
                    v.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    break;
                case Android.Views.DragAction.Drop:
                    v.SetBackgroundColor(Android.Graphics.Color.Transparent);
                    TemporalObject tmpObj = (TemporalObject)e.LocalState;
                    Android.Views.View view = tmpObj.View;

                    object passedItem = tmpObj.Item;
                    ListView oldParent = (ListView)view.Parent;
                    //BaseAdapter sourceAdapter = (oldParent.Adapter is IWrapperListAdapter) ? ((IWrapperListAdapter)oldParent.Adapter).WrappedAdapter as BaseAdapter : ((BaseAdapter)oldParent.Adapter);

                    try
                    {
                        ListView newParent = (ListView)v.Parent.Parent;
                        //BaseAdapter destinationAdapter = oldParent.Adapter is IWrapperListAdapter ?
                        //    (BaseAdapter) ((IWrapperListAdapter)newParent.Adapter).WrappedAdapter :
                        //    (BaseAdapter)oldParent.Adapter;

                        int removeLocation = oldParent.GetPositionForView(view);
                        int insertLocation = newParent.GetPositionForView(associatedView);
                        if (DragAndDropListViewRenderer.ListMap.ContainsKey(oldParent.Id.ToString()) && DragAndDropListViewRenderer.ListMap.ContainsKey(newParent.Id.ToString()))
                        {
                            var sourceList = DragAndDropListViewRenderer.ListMap[oldParent.Id.ToString()];
                            var destinationList = DragAndDropListViewRenderer.ListMap[newParent.Id.ToString()];
                            if (!oldParent.Equals(newParent) || removeLocation != insertLocation)
                            {
                                if (sourceList.Contains(passedItem))
                                {
                                    sourceList.Remove(passedItem);

                                    insertLocation -= 1;
                                    if (insertLocation < 0)
                                        insertLocation = 0;
                                    destinationList.Insert(insertLocation, passedItem);
                                }
                                //destinationAdapter.NotifyDataSetChanged();
                                //sourceAdapter.NotifyDataSetChanged();
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        ShowException(oldParent.Adapter.GetType().ToString());
                    }

                    break;
                case Android.Views.DragAction.Ended:
                    break;
            }

            return true;
        }

        private void ShowException(string text)
        {
            var alert = new AlertDialog.Builder(Xamarin.Forms.Forms.Context);

            var edit = new EditText(Xamarin.Forms.Forms.Context) { Text = text };
            alert.SetView(edit);

            alert.SetTitle("Exception");
            alert.SetPositiveButton("OK", (senderAlert, args) =>
            {

            });
            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {

            });
            alert.Show();
        }

        #endregion
    }
}