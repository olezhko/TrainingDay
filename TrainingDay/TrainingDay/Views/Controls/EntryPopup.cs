using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace TrainingDay.Views.Controls
{
    public class EntryPopup
    {
        public string Text { get; set; }
        public string Title { get; set; }
        public List<string> Buttons { get; set; }

        public EntryPopup(string title, string text, params string[] buttons)
        {
            Title = title;
            Text = text;
            Buttons = buttons.ToList();
        }

        public EntryPopup(string title, string text) : this(title, text, "OK", Resources.Resource.CancelString)
        {
        }

        public event EventHandler<EntryPopupClosedArgs> PopupClosed;
        public void OnPopupClosed(EntryPopupClosedArgs e)
        {
            var handler = PopupClosed;
            handler?.Invoke(this, e);
        }

        public void Show()
        {
            DependencyService.Get<IEntryPopupLoader>().ShowPopup(this);
        }
    }

    public class EntryPopupClosedArgs : EventArgs
    {
        public string Text { get; set; }
        public string Button { get; set; }
    }

    public interface IEntryPopupLoader
    {
        void ShowPopup(EntryPopup reference);
        void ShowPopup(QuestionPopup reference,string ok, string neg, string neut);
        void ShowPopup(QuestionPopup reference, string ok, string neg);
        void ShowPopup(QuestionPopup reference);
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

        public event EventHandler<EntryPopupClosedArgs> PopupClosed;
        public void OnPopupClosed(EntryPopupClosedArgs e)
        {
            var handler = PopupClosed;
            handler?.Invoke(this, e);
        }

        public void Show()
        {
            DependencyService.Get<IEntryPopupLoader>().ShowPopup(this);
        }
        public void Show(string ok, string neg, string neut)
        {
            DependencyService.Get<IEntryPopupLoader>().ShowPopup(this,ok,neg,neut);
        }

        public void Show(string ok, string neg)
        {
            DependencyService.Get<IEntryPopupLoader>().ShowPopup(this, ok, neg);
        }
    }
}
