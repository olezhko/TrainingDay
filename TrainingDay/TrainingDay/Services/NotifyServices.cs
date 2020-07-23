using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TrainingDay.Services
{
    public class NotifyServices
    {
        private static string domain = @"http://trainingday.ladyka.by/api";
        private static string _sendTokenUrl = domain + @"/auth/?token={0}&language={1}&zone={2}";
        public static async Task<bool> SendTokenToServer(string token, string language, TimeSpan zone)
        {
            try
            {
                HttpClient client = new HttpClient();
                var uri = new Uri(string.Format(_sendTokenUrl, token, language, (int)zone.TotalHours));
                var content = new StringContent($"token={token}&language={language}&zone={(int)zone.TotalHours}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);
                var res = await response.Content.ReadAsStringAsync();
                return res.Contains(token); // fix to check to success"
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static bool SendAlarmToServer(string token, DateTime alarmUtc, int id)
        {
            return true;
        }
    }
}
