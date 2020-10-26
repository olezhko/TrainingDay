using Syncfusion.DataSource.Extensions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TrainingDay.Model;
using TrainingDay.Services;
using TrainingDay.Views;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    public class BlogsPageViewModel:BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public ObservableCollection<BlogViewModel> BlogsCollection { get; set; }
        public BlogsPageViewModel()
        {
            BlogsCollection = new ObservableCollection<BlogViewModel>();
            LoadItems();
        }

        public ICommand RefreshCommand => new Command(LoadItems);

        public bool IsLoadingItems { get; set; }
        private async void LoadItems()
        {
            IsLoadingItems = true;
            OnPropertyChanged(nameof(IsLoadingItems));

            var res = await SiteService.GetBlogsFromServer();
            if (res!=null)
            {
                BlogsCollection = res.Select(item => new BlogViewModel(item)).OrderByDescending(item=>item.DateTime).ToObservableCollection();
                OnPropertyChanged(nameof(BlogsCollection));

                IsLoadingItems = false;
                OnPropertyChanged(nameof(IsLoadingItems));
            }
        }

        public ICommand OpenBlogCommand=>new Command<BlogViewModel>(OpenBlog);
        private async void OpenBlog(BlogViewModel obj)
        {
            await Navigation.PushModalAsync(new BlogItemPage(){BindingContext = obj});
        }
    }
}
