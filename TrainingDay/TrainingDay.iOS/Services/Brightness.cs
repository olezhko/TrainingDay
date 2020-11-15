using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using Newtonsoft.Json;
using TrainingDay.iOS.Services;
using TrainingDay.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceConfig))]
namespace TrainingDay.iOS.Services
{
    class DeviceConfig : IDeviceConfig
    {
        public void SetBrightness(float brightness)
        {
            UIScreen.MainScreen.Brightness = brightness;
        }

        public void SetTheme(bool isLightTheme)
        {
            SettingsLocal local = new SettingsLocal();
            local.IsLightTheme = isLightTheme;
            FileLoader.SaveData(JsonConvert.SerializeObject(local));
        }
    }
}