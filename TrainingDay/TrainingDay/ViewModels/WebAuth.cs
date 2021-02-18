using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TrainingDay.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TrainingDay.ViewModels
{
    public class WebAuthenticatorViewModel : BaseViewModel
    {
        const string authenticationUrl = "https://trainingday.tk/mobileauth/";

        public WebAuthenticatorViewModel()
        {
            GoogleEmail = Settings.GoogleEmail;
            GoogleCommand = new Command(async () => await OnAuthenticate("Google"));
            FacebookCommand = new Command(async () => await OnAuthenticate("Facebook"));
            AppleCommand = new Command(async () => await OnAuthenticate("Apple"));
        }
        public string GoogleEmail { get; set; }
        public ICommand GoogleCommand { get; }

        public ICommand FacebookCommand { get; }

        public ICommand AppleCommand { get; }

        string accessToken = string.Empty;

        public string AuthToken
        {
            get => accessToken;
            set => SetProperty(ref accessToken, value);
        }

        async Task OnAuthenticate(string scheme)
        {
            try
            {
                WebAuthenticatorResult r = null;

                if (scheme.Equals("Apple")
                    && DeviceInfo.Platform == DevicePlatform.iOS
                    && DeviceInfo.Version.Major >= 13)
                {
                    r = await AppleSignInAuthenticator.AuthenticateAsync();
                }
                else
                {
                    var authUrl = new Uri(authenticationUrl + scheme);
                    var callbackUrl = new Uri("trainingday://");

                    r = await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl);
                }

                AuthToken = r?.AccessToken ?? r?.IdToken;
                if (scheme == "Google")
                {
                    Settings.GoogleToken = AuthToken;
                    //if (r.Properties.TryGetValue("name", out var name) && !string.IsNullOrEmpty(name))
                    //    googleName = $"Name: {name}{Environment.NewLine}";
                    if (r.Properties.TryGetValue("email", out var email) && !string.IsNullOrEmpty(email))
                        Settings.GoogleEmail = $"Email: {email}{Environment.NewLine}";
                }
            }
            catch (Exception ex)
            {
                AuthToken = string.Empty;
                //await DisplayAlertAsync($"Failed: {ex.Message}");
            }
        }
    }
}
