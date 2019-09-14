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
using TrainingDay.Services;
using Xamarin.Forms;
using Application = Android.App.Application;


[assembly: Xamarin.Forms.Dependency(typeof(MessageAndroid))]
namespace TrainingDay.Droid.Services
{
    class MessageAndroid : IMessage
    {
        public void ShowMessage(string message, string title)
        {
            AlertDialog.Builder dialog = new AlertDialog.Builder(Forms.Context);
            dialog.SetTitle(title);
            dialog.SetMessage(message);
            dialog.SetPositiveButton("OK", (senderAlert, args) => {

            });
            var alertdialog = dialog.Create();
            alertdialog.Show();
        }

        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }

        //public string EnterAlert(string title, string oldValue)
        //{
        //    EditText et = new EditText(Forms.Context);
        //    et.Text = oldValue;
        //    AlertDialog.Builder alert = new AlertDialog.Builder(Forms.Context);
        //    alert.SetTitle(title);
        //    alert.SetView(et);

        //    alert.SetPositiveButton("OK", (senderAlert, args) => {

        //    });

        //    alert.SetNegativeButton("Cancel", (senderAlert, args) => {

        //    });

        //    var alertdialog = alert.Create();
        //    alertdialog.Show();
        //    return et.Text;
        //}
    }
}