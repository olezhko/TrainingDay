using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TrainingDay.Services
{
    public interface IMessage
    {
        void ShowMessage(string message, string title);
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}
