using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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

    public class MobileAlarm
    {
        public MobileAlarm(Alarm alarm)
        {
            this.Days = alarm.Days;
            this.Name = alarm.Name;
            this.Time = alarm.TimeOffset.TimeOfDay.ToString();
            this.Name = alarm.Name;
        }

        public string Token { get; set; }
        public string Name { get; set; }
        public string TrainingName { get; set; }
        public string Time { get; set; }
        public int Days { get; set; }
    }

    public class MobileAlarmAnswer
    {
        public int id { get; set; }
    }

    public class SiteService
    {
        private static string domain = @"https://www.trainingday.tk/api";
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

        
        private static string _alarmEditUrl = domain + @"/MobileAlarms/{0}";
        private static string _alarmCreateUrl = domain + @"/MobileAlarms";
        private static string _alarmDeleteUrl = domain + @"/MobileAlarms/delete/{0}";
        /// <summary>
        /// Create alarm
        /// </summary>
        /// <returns>Server Alarm Id</returns>
        public static async Task<int> SendAlarm(Alarm alarm)
        {
            try
            {
                MobileAlarm item = new MobileAlarm(alarm);
                item.TrainingName = App.Database.GetTrainingItem(alarm.TrainingId).Title;
                item.Token = Settings.Token;

                var client = new RestClient(_alarmCreateUrl);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(item), ParameterType.RequestBody);

                try
                {
                    var result = client.Execute(request);
                    Debug.WriteLine($"Result Status Code: {result.StatusCode} - {result.Content}");
                    var data = JsonConvert.DeserializeObject<MobileAlarmAnswer>(result.Content);
                    return data.id;
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                    return -1;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return -1;
            }
        }

        public static async Task<int> UpdateAlarm(Alarm alarm)
        {
            try
            {
                MobileAlarm item = new MobileAlarm(alarm);
                item.TrainingName = App.Database.GetTrainingItem(alarm.TrainingId).Title;
                item.Token = Settings.Token;
                var client = new RestClient(string.Format(_alarmEditUrl, alarm.ServerId));
                client.Timeout = -1;
                var request = new RestRequest(Method.PUT);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(item), ParameterType.RequestBody);
                try
                {
                    var result = await client.ExecuteAsync(request);
                    Debug.WriteLine($"Result Status Code: {result.StatusCode} - {result.Content}");
                    return Convert.ToInt32(result.Content);
                }
                catch (Exception e)
                {
                    Crashes.TrackError(e);
                    return -1;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return -1;
            }
        }
        
        public static async Task<bool> DeleteAlarm(int serverId)
        {
            if (serverId <= 0)
            {
                return false;
            }
            try
            {
                var client = new RestClient(string.Format(_alarmDeleteUrl,serverId));
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", "", ParameterType.RequestBody);
                IRestResponse response = await client.ExecuteAsync(request);
                Console.WriteLine(response.Content);
                return true;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return false;
            }
        }
    }
}