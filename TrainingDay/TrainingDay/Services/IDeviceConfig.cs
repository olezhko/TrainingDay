using System;
using System.Collections.Generic;
using System.Text;

namespace TrainingDay.Services
{
    public interface IDeviceConfig
    {
        void SetBrightness(float percent);
        void SetTheme(bool isLightTheme);
    }
}
