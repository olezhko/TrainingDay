using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrainingDay.Views.Controls;
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
            int querypages = 1;
            VideoSearch videos = new VideoSearch();
            var items = await videos.GetVideos(search, querypages);

            Console.OutputEncoding = Encoding.Unicode;
            foreach (var item in items)
            {
                Debug.WriteLine("Title: " + item.getTitle());
                Debug.WriteLine("Author: " + item.GetAuthor());
                Debug.WriteLine("Description: " + item.getDescription());
                Debug.WriteLine("Duration: " + item.getDuration());
                Debug.WriteLine("Url: " + item.getUrl());
                Debug.WriteLine("Thumbnail: " + item.getThumbnail());
                Debug.WriteLine("ViewCount: " + item.getViewCount());
                Debug.WriteLine("");

                if (result.Count == maxCount)
                {
                    break;
                }
                result.Add(new YoutubeVideoItem()
                {
                    VideoTitle = item.getTitle(),
                    VideoUrl = item.getUrl(),
                    VideoAuthor = item.GetAuthor()
                });
            }

            return result;
        }
    }

    public class VideoSearchComponents
    {
        private String Title;
        private String Author;
        private String Description;
        private String Duration;
        private String Url;
        private String Thumbnail;
        private String ViewCount;

        public VideoSearchComponents(String Title, String Author, String Description, String Duration, String Url, String Thumbnail, String ViewCount)
        {
            this.setTitle(Title);
            this.SetAuthor(Author);
            this.setDescription(Description);
            this.setDuration(Duration);
            this.setUrl(Url);
            this.setThumbnail(Thumbnail);
            this.setViewCount(ViewCount);
        }

        public String getTitle()
        {
            return Title;
        }

        public void setTitle(String title)
        {
            Title = title;
        }

        public String GetAuthor()
        {
            return Author;
        }

        public void SetAuthor(String author)
        {
            Author = author;
        }

        public String getDescription()
        {
            return Description;
        }

        public void setDescription(String description)
        {
            Description = description;
        }

        public String getDuration()
        {
            return Duration;
        }

        public void setDuration(String duration)
        {
            Duration = duration;
        }

        public String getUrl()
        {
            return Url;
        }

        public void setUrl(String url)
        {
            Url = url;
        }

        public String getThumbnail()
        {
            return Thumbnail;
        }

        public void setThumbnail(String thumbnail)
        {
            Thumbnail = thumbnail;
        }

        public String getViewCount()
        {
            return ViewCount;
        }

        public void setViewCount(String viewcount)
        {
            ViewCount = viewcount;
        }
    }

    class Web
    {
        static WebClient webclient;

        public static async Task<String> getContentFromUrl(String Url)
        {
            try
            {
                webclient = new WebClient();
                webclient.Encoding = Encoding.UTF8;

                Task<string> downloadStringTask = webclient.DownloadStringTaskAsync(new Uri(Url));
                var content = await downloadStringTask;

                webclient.DownloadStringAsync(new Uri(Url));

                return content.Replace('\r', ' ').Replace('\n', ' ');
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }

    public class Helper
    {
        public static string Folder { get; set; }

        public static String ExtractValue(String Source, String Start, String End)
        {
            int start, end;

            try
            {
                if (Source.Contains(Start) && Source.Contains(End))
                {
                    start = Source.IndexOf(Start, 0) + Start.Length;
                    end = Source.IndexOf(End, start);

                    return Source.Substring(start, end - start);
                }
                else
                    return printZero();
            }
            catch (Exception ex)
            {
                return printZero();
            }
        }

        public static String printZero()
        {
            return " ";
        }

        public static string makeFilenameValid(string file)
        {
            char replacementChar = '_';

            var blacklist = new HashSet<char>(Path.GetInvalidFileNameChars());

            var output = file.ToCharArray();

            for (int i = 0, ln = output.Length; i < ln; i++)
            {
                if (blacklist.Contains(output[i]))
                    output[i] = replacementChar;
            }

            return new string(output);
        }
    }

    public class VideoSearch
    {
        static List<VideoSearchComponents> items;

        static string title;
        static string author;
        static string description;
        static string duration;
        static string url;
        static string thumbnail;
        static string viewcount;

        /// <summary>
        /// Search videos
        /// </summary>
        /// <param name="querystring"></param>
        /// <param name="querypages"></param>
        /// <returns></returns>
        public async Task<List<VideoSearchComponents>> GetVideos(string querystring, int querypages)
        {
            items = new List<VideoSearchComponents>();

            // Do search
            for (int i = 1; i <= querypages; i++)
            {
                // Search address
                string content = await Web.getContentFromUrl("https://www.youtube.com/results?search_query=" + querystring + "&page=" + i);

                // Search string
                string pattern = "<div class=\"yt-lockup-content\">.*?title=\"(?<NAME>.*?)\".*?</div></div></div></li>";
                MatchCollection result = Regex.Matches(content, pattern, RegexOptions.Singleline);

                for (int ctr = 0; ctr <= result.Count - 1; ctr++)
                {
                    // Title
                    title = result[ctr].Groups[1].Value;

                    // Author
                    author = Helper.ExtractValue(result[ctr].Value, "/user/", "class").Replace('"', ' ').TrimStart().TrimEnd();

                    if (string.IsNullOrEmpty(author))
                        author = Helper.ExtractValue(result[ctr].Value, " >", "</a>");

                    // Description
                    description = Helper.ExtractValue(result[ctr].Value, "dir=\"ltr\" class=\"yt-uix-redirect-link\">", "</div>");

                    if (string.IsNullOrEmpty(description))
                        description = Helper.ExtractValue(result[ctr].Value, "<div class=\"yt-lockup-description yt-ui-ellipsis yt-ui-ellipsis-2\" dir=\"ltr\">", "</div>");


                    // Duration
                    duration = Helper.ExtractValue(Helper.ExtractValue(result[ctr].Value, "id=\"description-id-", "span"), ": ", "<").Replace(".", "");

                    // Url
                    url = string.Concat("http://www.youtube.com/watch?v=", Helper.ExtractValue(result[ctr].Value, "watch?v=", "\""));

                    // Thumbnail
                    thumbnail = "https://i.ytimg.com/vi/" + Helper.ExtractValue(result[ctr].Value, "watch?v=", "\"") + "/mqdefault.jpg";

                    // View count
                    {
                        string strView = Helper.ExtractValue(result[ctr].Value, "</li><li>", "</li></ul></div>");
                        if (!string.IsNullOrEmpty(strView) && !string.IsNullOrWhiteSpace(strView))
                        {
                            string[] strParsedArr =
                                strView.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                            string parsedText = strParsedArr[0];
                            parsedText = parsedText.Trim().Replace(",", ".");

                            viewcount = parsedText;
                        }
                    }

                    // Remove playlists
                    if (title != "__title__" && title != " ")
                    {
                        if (duration != "" && duration != " ")
                        {
                            // Add item to list
                            items.Add(new VideoSearchComponents(Utilities.HtmlDecode(title),
                                Utilities.HtmlDecode(author), Utilities.HtmlDecode(description), duration, url, thumbnail, viewcount));
                        }
                    }
                }
            }

            return items;
        }
    }

    class Utilities
    {
        public static string HtmlDecode(string value)
        {
            try
            {
                return WebUtility.HtmlDecode(value);
            }
            catch { return value; }
        }
    }
}
