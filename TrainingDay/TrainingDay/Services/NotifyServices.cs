using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TrainingDay.Controls;

namespace TrainingDay.Services
{
    public class NotifyServices
    {
        private static string domain = @"http://trainingday.ladyka.by/api";
        private static string _sendTokenUrl = domain + @"/auth/?token={0}&language={1}&zone={2}";
        private static string _sendAlarmAddUrl = domain + @"/alarm/add/?token={0}{1}";
        private static string _sendAlarmDelUrl = domain + @"/alarm/del/?token={0}&alarmId={1}";
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

        public static async Task<bool> SendAlarmToServer(string token, Alarm alarm)
        {
            try
            {
                var days = DaysOfWeek.Parse(alarm.Days).AllDays.Select(item => item ? 1 : 0);
                string alarmString = $"&id={alarm.Id}&days={string.Join("", days)}&time={alarm.TimeOffset}";

                HttpClient client = new HttpClient();
                var uri = new Uri(string.Format(_sendAlarmAddUrl, token, alarm));
                var content = new StringContent($"token={token}{alarmString}", 
                    Encoding.UTF8, "application/json");
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

        public static async Task<bool> RemoveAlarmOnServer(string token, int alarmId)
        {
            try
            {
                HttpClient client = new HttpClient();
                var uri = new Uri(string.Format(_sendAlarmDelUrl, token, alarmId));
                var content = new StringContent($"token={token}&alarmId={alarmId}",
                    Encoding.UTF8, "application/json");
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
    }
}
