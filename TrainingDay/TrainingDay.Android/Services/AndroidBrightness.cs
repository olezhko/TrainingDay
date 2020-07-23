using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using TrainingDay.Droid.Services;
using TrainingDay.Services;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidBrightnessService))]
namespace TrainingDay.Droid.Services
{
    public class AndroidBrightnessService : IDeviceConfig
    {
        public void SetBrightness(float brightness)
        {
            var window = ((Activity)Forms.Context).Window;
            //var window = CrossCurrentActivity.Current.Activity.Window;
            var attributesWindow = new WindowManagerLayoutParams();

            attributesWindow.CopyFrom(window.Attributes);
            attributesWindow.ScreenBrightness = brightness;

            window.Attributes = attributesWindow;
        }

        public void SetTheme(bool isLightTheme)
        {
            SettingsLocal local = new SettingsLocal();
            local.IsLightTheme = isLightTheme;
            FileLoader.SaveData(JsonConvert.SerializeObject(local));
        }
    }
}