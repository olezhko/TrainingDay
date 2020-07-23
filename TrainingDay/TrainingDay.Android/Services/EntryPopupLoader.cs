
using Android.App;
using TrainingDay.Droid.Services;
using TrainingDay.Views.Controls;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(PopupLoader))]
namespace TrainingDay.Droid.Services
{
    public class PopupLoader : IPopupLoader
    {
        public void ShowQuestionPopup(QuestionPopup popup, string positiveText, string negativeText)
        {
            var alert = new AlertDialog.Builder(Forms.Context);

            alert.SetMessage(popup.Text);
            alert.SetTitle(popup.Title);

            alert.SetPositiveButton(positiveText, (senderAlert, args) =>
            {
                popup.OnPopupClosed(new PopupClosedArgs
                {
                    Button = positiveText
                });
            });

            alert.SetNegativeButton(negativeText, (senderAlert, args) =>
            {
                popup.OnPopupClosed(new PopupClosedArgs
                {
                    Button = negativeText
                });
            });

            alert.Show();
        }
    }
}