using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace TrainingDay.Views.Controls
{
    public class PopupClosedArgs : EventArgs
    {
        public string Text { get; set; }
        public string Button { get; set; }
    }

    public interface IPopupLoader
    {
        void ShowQuestionPopup(QuestionPopup reference, string ok, string neg);
    }

    public class QuestionPopup
    {
        public string Text { get; set; }
        public string Title { get; set; }

        public QuestionPopup(string title, string text)
        {
            Title = title;
            Text = text;
        }

        public event EventHandler<PopupClosedArgs> PopupClosed;
        public void OnPopupClosed(PopupClosedArgs e)
        {
            var handler = PopupClosed;
            handler?.Invoke(this, e);
        }

        public void Show(string ok, string neg)
        {
            DependencyService.Get<IPopupLoader>().ShowQuestionPopup(this, ok, neg);
        }
    }
}
