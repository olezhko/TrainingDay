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
using TrainingDay.Droid.Services;
using TrainingDay.Views.Controls;

[assembly: Xamarin.Forms.Dependency(typeof(EntryPopupLoader))]
namespace TrainingDay.Droid.Services
{
    public class EntryPopupLoader : IEntryPopupLoader
    {
        public void ShowPopup(EntryPopup popup)
        {
            var alert = new AlertDialog.Builder(Xamarin.Forms.Forms.Context);

            var edit = new EditText(Xamarin.Forms.Forms.Context) { Text = popup.Text };
            alert.SetView(edit);

            alert.SetTitle(popup.Title);

            alert.SetPositiveButton("OK", (senderAlert, args) =>
            {
                popup.OnPopupClosed(new EntryPopupClosedArgs
                {
                    Button = "OK",
                    Text = edit.Text
                });
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                popup.OnPopupClosed(new EntryPopupClosedArgs
                {
                    Button = "Cancel",
                    Text = edit.Text
                });
            });
            alert.Show();
        }

        public void ShowPopup(QuestionPopup popup)
        {
            var alert = new AlertDialog.Builder(Xamarin.Forms.Forms.Context);

            alert.SetMessage(popup.Text);
            alert.SetTitle(popup.Title);

            alert.SetPositiveButton("OK", (senderAlert, args) =>
            {
                popup.OnPopupClosed(new EntryPopupClosedArgs
                {
                    Button = "OK"
                });
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                popup.OnPopupClosed(new EntryPopupClosedArgs
                {
                    Button = "Cancel"
                });
            });
            alert.Show();
        }

        public void ShowPopup(QuestionPopup popup, string positiveText, string negativeText, string neitralText)
        {
            var alert = new AlertDialog.Builder(Xamarin.Forms.Forms.Context);

            alert.SetMessage(popup.Text);
            alert.SetTitle(popup.Title);

            alert.SetPositiveButton(positiveText, (senderAlert, args) =>
            {
                popup.OnPopupClosed(new EntryPopupClosedArgs
                {
                    Button = positiveText
                });
            });

            alert.SetNegativeButton(negativeText, (senderAlert, args) =>
            {
                popup.OnPopupClosed(new EntryPopupClosedArgs
                {
                    Button = negativeText
                });
            });

            alert.SetNeutralButton(neitralText, (senderAlert, args) =>
            {
                popup.OnPopupClosed(new EntryPopupClosedArgs
                {
                    Button = neitralText
                });
            });
            alert.Show();
        }
        public void ShowPopup(QuestionPopup popup, string positiveText, string negativeText)
        {
            var alert = new AlertDialog.Builder(Xamarin.Forms.Forms.Context);

            alert.SetMessage(popup.Text);
            alert.SetTitle(popup.Title);

            alert.SetPositiveButton(positiveText, (senderAlert, args) =>
            {
                popup.OnPopupClosed(new EntryPopupClosedArgs
                {
                    Button = positiveText
                });
            });

            alert.SetNegativeButton(negativeText, (senderAlert, args) =>
            {
                popup.OnPopupClosed(new EntryPopupClosedArgs
                {
                    Button = negativeText
                });
            });

            alert.Show();
        }
    }
}