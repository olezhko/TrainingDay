using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BlogItemPage : ContentPage
    {
        public BlogItemPage()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.iOS)
            {
                //iOS stuff
            }
            else if (Device.RuntimePlatform == Device.Android)
            {

            }
        }
    }
}