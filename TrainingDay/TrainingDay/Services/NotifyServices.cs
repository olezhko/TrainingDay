using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TrainingDay.Services
{
    public class MobileToken
    {
        public string Token { get; set; }
        public string Language { get; set; }
        public string Zone { get; set; }
        public int Frequency { get; set; }
    }

    public class NotifyServices
    {
        private static string domain = @"https://www.traningday.tk/api";
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
                    Debug.WriteLine($"Error: {e.Message}");
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }




        //private static string _sendAlarmAddUrl = domain + @"/alarm/add/?token={0}{1}";
        //public static async Task<bool> SendAlarmToServer(string token, Alarm alarm)
        //{
        //    try
        //    {
        //        var days = DaysOfWeek.Parse(alarm.Days).AllDays.Select(item => item ? 1 : 0);
        //        string alarmString = $"&id={alarm.Id}&days={string.Join("", days)}&time={alarm.TimeOffset}";

        //        HttpClient client = new HttpClient();
        //        var uri = new Uri(string.Format(_sendAlarmAddUrl, token, alarm));
        //        var content = new StringContent($"token={token}{alarmString}", 
        //            Encoding.UTF8, "application/json");
        //        var response = await client.PostAsync(uri, content);
        //        var res = await response.Content.ReadAsStringAsync();
        //        return res.Contains(token); // fix to check to success"
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        return false;
        //    }
        //}

        //private static string _sendAlarmDelUrl = domain + @"/alarm/del/?token={0}&alarmId={1}";
        //public static async Task<bool> RemoveAlarmOnServer(string token, int alarmId)
        //{
        //    try
        //    {
        //        HttpClient client = new HttpClient();
        //        var uri = new Uri(string.Format(_sendAlarmDelUrl, token, alarmId));
        //        var content = new StringContent($"token={token}&alarmId={alarmId}",
        //            Encoding.UTF8, "application/json");
        //        var response = await client.PostAsync(uri, content);
        //        var res = await response.Content.ReadAsStringAsync();
        //        return res.Contains(token); // fix to check to success"
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        return false;
        //    }
        //}
    }
}
