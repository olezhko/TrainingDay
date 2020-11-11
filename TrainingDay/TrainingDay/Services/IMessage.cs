using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TrainingDay.Services
{
    public interface IMessage
    {
        void ShowMessage(string message, string title);
        void LongAlert(string message);
        void ShortAlert(string message);
        void ShowNotification(int id, string title, string message, bool isUpdateCurrent, bool isSilent);
        void CancelNotification(int id);
    }
}
