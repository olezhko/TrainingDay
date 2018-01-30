using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.View;
using TrainingDay.ViewModel;
using Xamarin.Forms;

namespace TrainingDay
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            InitializeComponent();

            masterPage.ListView.ItemSelected += OnItemSelected;

            if (Device.RuntimePlatform == Device.UWP)
            {
                MasterBehavior = MasterBehavior.Popover;
            }
        }

        void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MasterPageItem;
            if (item != null)
            {
                var newDetail = (Page) Activator.CreateInstance(item.TargetType);
                Detail = new NavigationPage(newDetail);
                masterPage.ListView.SelectedItem = null;
                IsPresented = false; 
            }
        }
    }
}
