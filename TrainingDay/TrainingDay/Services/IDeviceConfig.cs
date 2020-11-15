using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TrainingDay.Services
{
    public class SettingsLocal
    {
        public bool IsLightTheme { get; set; }
    }

    public class FileLoader
    {
        public static async Task<string> ReadData()
        {
            try
            {

                var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SettingData.txt");

                if (backingFile == null || !File.Exists(backingFile))
                {
                    return null;
                }
                string FileData = string.Empty;
                using (var reader = new StreamReader(backingFile, true))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        FileData = line;
                    }
                }

                return FileData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static async Task SaveData(string Data)
        {
            try
            {
                var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "SettingData.txt");
                using (var writer = File.CreateText(backingFile))
                {
                    await writer.WriteLineAsync(Data);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
    public interface IDeviceConfig
    {
        void SetBrightness(float percent);
        void SetTheme(bool isLightTheme);
    }
}
