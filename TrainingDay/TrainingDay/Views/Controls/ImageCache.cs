using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;

namespace TrainingDay.Views.Controls
{
    public class ImageCache:Image
    {
        public ImageCache()
        {
            App.ImageDownloader.Downloaded += ImageDownloaderOnDownloaded;
            BackgroundColor = Color.Black;
            Source = "main.png";
        }

        public static readonly BindableProperty ImageUrlProperty = BindableProperty.Create(nameof(ImageUrl), typeof(string), typeof(ImageCache), null, propertyChanged: (bindable, oldvalue, newvalue) => ((ImageCache)bindable).OnImageUrlChanged(), defaultBindingMode: BindingMode.TwoWay);
        public string ImageUrl
        {
            get { return (string)this.GetValue(ImageUrlProperty); }
            set { this.SetValue(ImageUrlProperty, value); }
        }
        public void OnImageUrlChanged()
        {
            LoadImage();
        }
        private void LoadImage()
        {
            try
            {
                if (!string.IsNullOrEmpty(ImageUrl))
                {
                    BackgroundColor = Color.Transparent;
                    var imageSource = App.Database.GetImage(ImageUrl);
                    if (imageSource == null)
                    {
                        var uriSource = new UriImageSource()
                        {
                            Uri = new Uri(ImageUrl),
                            CachingEnabled = false,
                        };
                        Source = uriSource;

                        App.ImageDownloader.AddUrl(ImageUrl);
                    }
                    else
                    {
                        Source = ImageSource.FromStream(() => Stream(imageSource));
                    }
                }
                else
                {
                    Source = "main.png";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ImageDownloaderOnDownloaded(object sender, ImageData e)
        {
            Console.WriteLine($"ImageCache ImageDownloaderOnDownloaded {ImageUrl}");
            App.ImageDownloader.Downloaded -= ImageDownloaderOnDownloaded;
            var item = App.Database.GetImage(ImageUrl);
            if (item!=null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Source = ImageSource.FromStream(() => Stream(item));
                });
            }
        }

        private Stream Stream(ImageData data)
        {
            return new MemoryStream(data.Data);
        }
    }


    public class ImageQueueCacheDownloader
    {
        public ImageQueueCacheDownloader()
        {
            Start();
        }

        public event EventHandler<ImageData> Downloaded;
        private static readonly Queue<string> _items = new Queue<string>();
        public void AddUrl(string url)
        {
            if (!_items.Contains(url))
            {
                Console.WriteLine($"ImageCache ImageQueueCacheDownloader AddUrl {url}");
                _items.Enqueue(url);
            }
        }

        public void Start()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        if (_items.Count != 0)
                        {
                            using (var client = new WebClient())
                            {
                                string url = _items.Dequeue();
                                Console.WriteLine($"ImageCache TryDownload {url}");
                                var data = client.DownloadData(new Uri(url));

                                var item = new ImageData()
                                {
                                    Data = data,
                                    Url = url,
                                };
                                App.Database.SaveImage(item);
                                OnDownloaded(item);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    Thread.Sleep(400);
                }
            });
        }

        protected virtual void OnDownloaded(ImageData e)
        {
            Downloaded?.Invoke(this, e);
        }
    }

    public class ImageData
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        public string Url { get; set; }
        public byte[] Data { get; set; }
    }
}
