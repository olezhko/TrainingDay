using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using TrainingDay.Model;

namespace TrainingDay.Services
{
    public class MobileToken
    {
        public string Token { get; set; }
        public string Language { get; set; }
        public string Zone { get; set; }
        public int Frequency { get; set; }
    }

    public class SiteService
    {
        private static string domain = @"https://www.trainingday.tk/api";
        //private static string domain = @"http://localhost:59734/api";
        private static string _sendTokenUrl = domain + @"/MobileTokens";
        public static async Task<bool> SendTokenToServer(string tokenString, string language, TimeSpan zone, int freq)
        {
            try
            {
                MobileToken token = new MobileToken();
                token.Frequency = freq;
                token.Language = language;
                token.Token = tokenString;
                token.Zone = zone.ToString();
                var client = new RestClient(_sendTokenUrl);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Cookie", ".AspNetCore.Antiforgery.F1R49Bn-sHQ=CfDJ8DSuB5KQHH1HjhVjYb_UYNkYnk6PjUp68qgFq-vApTTa3RgeXB8Harisq5G6j8Vkg6qU734I-rQc_UzJBGUzGqj4LHrD22C_xEO4y-103zt7fOxt933aKV0B4TjmCyornURtfXjI-FjElAJo_qrtN_c");
                request.AddParameter("application/json", JsonConvert.SerializeObject(token), ParameterType.RequestBody);
                try
                {
                    var result = await client.ExecuteAsync(request);
                    Debug.WriteLine($"Result Status Code: {result.StatusCode} - {result.Content}");
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                }
                return true;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return false;
            }
        }


        private static string _getBlogsUrl = domain + @"/MobileBlogs";
        public static async Task<ObservableCollection<MobileBlog>> GetBlogsFromServer()
        {
            var client = new RestClient(_getBlogsUrl);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "", ParameterType.RequestBody);
            try
            {
                var result = await client.ExecuteAsync(request);
                Debug.WriteLine($"Result Status Code: {result.StatusCode} - {result.Content}");
                return JsonConvert.DeserializeObject<ObservableCollection<MobileBlog>>(result.Content);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                Debug.WriteLine($"Error: {e.Message}");
                return new ObservableCollection<MobileBlog>();
            }
        }
    }
}
