using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views.Controls
{
    public class PopupClosedArgs : EventArgs
    {
        public string Text { get; set; }
        public string Button { get; set; }
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
        public async void Show(string ok, string neg)
        {
            var task = await App.Current.MainPage.DisplayAlert(this.Title, this.Text, ok, neg);
            var handler = PopupClosed;
            handler?.Invoke(this, new PopupClosedArgs()
            {
                Button = task ? ok : neg
            });
        }
    }
}
