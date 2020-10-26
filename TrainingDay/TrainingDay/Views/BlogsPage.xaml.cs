using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BlogsPage : ContentPage
    {
        public BlogsPage()
        {
            InitializeComponent();
            BindingContext = new BlogsPageViewModel() { Navigation = this.Navigation };
        }
    }
}