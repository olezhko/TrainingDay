using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TrainingDay.Services
{
    public class YoutubeVideoItem
    {
        public string VideoAuthor{ get; set; }
        public string VideoUrl { get; set; }
        public string VideoTitle { get; set; }

        public WebViewSource WebViewData
        {
            get
            {
                var htmlSource = new HtmlWebViewSource();

                //htmlSource.Html = $@"<html><body style=""background-color:{backColor} ; color: {textColor};"">
                htmlSource.Html = $@"<html><body>
                  <iframe width=""300"" height=""300"" src=""{ConvertUrl()}"" frameborder=""0"" allow=""accelerometer; autoplay; encrypted - media; gyroscope; picture -in-picture"" allowfullscreen ></iframe>
                     </body></html>";
                return htmlSource;
                //<iframe width="560" height="315" src="https://www.youtube.com/embed/Cb8kB41eAiA" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
            }
        }

        private string ConvertUrl()
        {
            var sb = new StringBuilder();
            sb.Append(@"https://www.youtube.com/embed/");

            sb.Append(VideoUrl.Replace("http://www.youtube.com/watch?v=", "").Replace("https://www.youtube.com/watch?v=", ""));
            return sb.ToString();
        }
    }

    public class YoutubeService
    {
        public static async Task<List<YoutubeVideoItem>> GetVideoItemsAsync(string search, int maxCount = 10)
        {
            var result = new List<YoutubeVideoItem>();

            Console.OutputEncoding = Encoding.Unicode;
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyCDxiDJf9BcygNESd7skxprJsCR9YiJmuA",
                ApplicationName = "TrainingDay"
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = search; // Replace with your search term.
            searchListRequest.MaxResults = maxCount;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<string> videos = new List<string>();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(String.Format("{0} ({1}) {2}", searchResult.Snippet.Title, searchResult.Id.VideoId, searchResult.Snippet.ChannelTitle));
                        result.Add(new YoutubeVideoItem()
                        {
                            VideoAuthor = searchResult.Snippet.ChannelTitle,
                            VideoTitle = searchResult.Snippet.Title,
                            VideoUrl = searchResult.Id.VideoId
                        });
                        break;
                }
                if (result.Count == maxCount)
                {
                    break;
                }
            }

            Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));

            return result;
        }
    }
}
