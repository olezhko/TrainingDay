using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using TrainingDay.iOS.Services;
using TrainingDay.Views.Controls;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(PopupLoader))]
namespace TrainingDay.iOS.Services
{
    class PopupLoader:IPopupLoader
    {
        public void ShowQuestionPopup(QuestionPopup popup, string ok, string neg)
        {
            UIAlertView alert = new UIAlertView()
            {
                Title = popup.Title,
                Message = popup.Text
            };
            var okIndex = alert.AddButton(ok);
            var negIndex = alert.AddButton(neg);

            alert.Clicked += delegate (object a, UIButtonEventArgs b) {
                Console.WriteLine("Button " + b.ButtonIndex.ToString() + " clicked");

                popup.OnPopupClosed(new PopupClosedArgs
                {
                    Button = okIndex == b.ButtonIndex? ok:neg
                });
            };
            alert.Show();
        }
    }
}