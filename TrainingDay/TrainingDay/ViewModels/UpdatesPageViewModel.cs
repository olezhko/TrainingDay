using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using TrainingDay.Services;
using TrainingDay.Views;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    public class UpdatesPageViewModel:BaseViewModel
    {
        public ObservableCollection<UpdateViewModel> UpdatesCollection { get; set; }
        public UpdatesPageViewModel()
        {
            UpdatesCollection = new ObservableCollection<UpdateViewModel>();
            LoadAllUpdates();

            VersionChanged();// to show last version
        }

        ObservableCollection<UpdateViewModel> _baseUpdatesCollection = new ObservableCollection<UpdateViewModel>();
        string text;

        private void LoadAllUpdates()
        {
            try
            {
                var ci = DependencyService.Get<ILocalize>().GetCurrentLanguage();
                string filename = $"TrainingDay.Resources.updates.updates_{ci}.xml";

                var assembly = typeof(Repository).GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream(filename);

                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream(@"TrainingDay.Resources.updates.updates_en.xml");
                }

                if (stream == null)
                {
                    return;
                }

                string result = "";
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }

                var updatesBase = JsonConvert.DeserializeObject<List<UpdateItem>>(result);
                ConvertFromResource(updatesBase);
                _baseUpdatesCollection = new ObservableCollection<UpdateViewModel>(updatesBase.Select(item => new UpdateViewModel(item)));
            }
            catch (Exception e)
            {
                DependencyService.Get<IMessage>().ShortAlert("Database Exception Init");
            }
        }

        private void ConvertFromResource(List<UpdateItem> items)
        {
            foreach (var updateItem in items)
            {
                try
                {
                    text = updateItem.Text;
                    ReplaceParams(ref text, "width=", "width=\"");
                    ReplaceParams(ref text, "height=", "height=\"");
                    ReplaceParams(ref text, "align=", "align=\"");
                    ReplaceParams(ref text, "src=", "src=\"");
                    updateItem.Text = text;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private void ReplaceParams(ref string value, string param,string newParam)
        {
            try
            {
                value = value.Replace(param, newParam);
                int index = value.IndexOf(param);
                if (index == -1)
                {
                    return;
                }
                for (int i = index; i < value.Length; i++)
                {
                    if (value[i] == ' ')
                    {
                        value = value.Insert(i, "\"");
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public ICommand VersionChangedCommand => new Command(VersionChanged);
        private void VersionChanged()
        {
            UpdatesCollection = new ObservableCollection<UpdateViewModel>(_baseUpdatesCollection.Where(item => item.Model.Version == App.Version));
            if (UpdatesCollection.Count == 0)
            {
                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
            OnPropertyChanged(nameof(UpdatesCollection));
        }

        public ICommand CloseCommand => new Command(Close);
        private void Close()
        {
            Application.Current.MainPage = new NavigationPage(new MainPage());

            App.Database.SaveUpdate(App.Version);
        }
    }

    public class UpdateViewModel : BaseViewModel
    {
        private UpdateItem model;

        public UpdateItem Model
        {
            get
            {
                if (model == null)
                {
                    model = new UpdateItem();
                }

                return model;
            }
            set
            {
                model = value;
                OnPropertyChanged();
            }
        }

        public string Version
        {
            get => Model.Version;
            set
            {
                Model.Version = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get => Model.Text;
            set
            {
                Model.Text = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => Model.Title;
            set
            {
                Model.Title = value;
                OnPropertyChanged();
            }
        }

        public WebViewSource WebViewData
        {
            get
            {
                var textColor = Settings.IsLightTheme ? "#000000" : "#FFFFFF";
                var backColor = Settings.IsLightTheme ? "#e0ddcf" : "#1F2230";
                var htmlSource = new HtmlWebViewSource();
                        
                htmlSource.Html = $@"<html><body style=""background-color:{backColor} ; color: {textColor};"">
                  <h2>{Title}</h2>
                  {Text}
                  </body></html>";
                //htmlSource.Html = @"<html><body><h2>Группировка Тренировок</h2><p>Добавление тренировки в группу.</p> <ol><li>Коснитесь и удерживайте тренировку, чтобы появилось меню.</li><li>Нажмите кнопку <img width=""30"" height =""30"" src =""union_add.png""/>, создайте новую группу или выберите существующую группу.</li></ol></body></html>";
                return htmlSource;
            }
        }

        public UpdateViewModel(UpdateItem model)
        {
            Model = model;
        }
    }

    public class UpdateItem
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public string Version { get; set; }
        public string Title { get; set; }
        public string Text{ get; set; }
    }
}
