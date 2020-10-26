using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using TrainingDay.ViewModels;
using Xamarin.Forms;

namespace TrainingDay.Model
{
    public class MobileBlog
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string ShortText { get; set; } // text with web urls and etc
        public string Text { get; set; } // text with web urls and etc
        public DateTime DateTime { get; set; } // text with web urls and etc
    }


    public class BlogViewModel:BaseViewModel
    {
        private int id;
        private string title;
        private string shortText;
        private string text;
        private DateTime dateTime;
        private MobileBlog item;

        public BlogViewModel()
        {
            item = new MobileBlog();
        }

        public BlogViewModel(MobileBlog item)
        {
            this.item = item;
            Title = item.Title;
            Text = item.Text;
            ShortText = item.ShortText;
            DateTime = item.DateTime;
        }

        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        public string ShortText
        {
            get => shortText;
            set
            {
                shortText = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }

        public DateTime DateTime
        {
            get => dateTime;
            set
            {
                dateTime = value;
                OnPropertyChanged();
            }
        }

        public WebViewSource WebViewDataText
        {
            get
            {
                var backstring = "{background-color: #FFFFFFFF}";
                var htmlSource = new HtmlWebViewSource();
                htmlSource.Html = $@"<html><body> 
<head>  <style>   body{    backstring   }  </style> </head>{Text}  </body></html>";
                return htmlSource;
                //<iframe width="560" height="315" src="https://www.youtube.com/embed/Cb8kB41eAiA" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
            }
        }

        public WebViewSource WebViewDataShortText
        {
            get
            {

                var backstring = "{background-color: #FFFFFFFF}";
                var htmlSource = new HtmlWebViewSource();
                htmlSource.Html = $@"<html><body> 
<head>  <style>   body{    backstring   }  </style> </head>{ShortText}  </body></html>";
                return htmlSource;
                //<iframe width="560" height="315" src="https://www.youtube.com/embed/Cb8kB41eAiA" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
            }
        }
    }
}
