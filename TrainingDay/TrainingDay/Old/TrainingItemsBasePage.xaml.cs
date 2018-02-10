using System.Windows.Input;
using TrainingDay.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TrainingDay.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrainingItemsBasePage : ContentPage
    {
        public TrainingItemsBasePage()
        {
            InitializeComponent();
            BindingContext = new TrainingItemsBasePageViewModel(){Navigation = this.Navigation };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var vm = BindingContext as TrainingItemsBasePageViewModel;
            vm.LoadItems();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            RemoveBinding(ItemSelectedCommandProperty);
            SetBinding(ItemSelectedCommandProperty, new Binding(ItemSelectedCommandPropertyName));
        }

        private void HandleItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }

            var command = ItemSelectedCommand;
            if (command != null && command.CanExecute(e.SelectedItem))
            {
                command.Execute(e.SelectedItem);
            }
        }

        public const string ItemSelectedCommandPropertyName = "ItemSelectedCommand";
        public static BindableProperty ItemSelectedCommandProperty = BindableProperty.Create(
            propertyName: "ItemSelectedCommand",
            returnType: typeof(ICommand),
            declaringType: typeof(TrainingItemsBasePage),
            defaultValue: null);

        public ICommand ItemSelectedCommand
        {
            get { return (ICommand)GetValue(ItemSelectedCommandProperty); }
            set { SetValue(ItemSelectedCommandProperty, value); }
        }
    }
}