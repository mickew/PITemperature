using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace uwaTemperatureClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly CoreDispatcher _dispatcher;
        private PITSettings settings;

        public MainPage()
        {
            this.InitializeComponent();
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            settings = new PITSettings();
            settings.Load();
            if (settings.CheckOk())
            {
                SetServerLabel(settings.Server);
                Navigate(new Uri(settings.Server));
            }
        }

        public async Task<string> MakeWebRequest(Uri uri)
        {
            HttpClient http = new System.Net.Http.HttpClient();
            HttpResponseMessage response = await http.GetAsync(uri);
            return await response.Content.ReadAsStringAsync();
        }

        private async Task Navigate(Uri uri)
        {
            try
            {
                //string s = await MakeWebRequest(uri);
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => browser.Navigate(uri));
            }
            catch
            {
            }
        }

        private async void ShowStatusBar(bool show)
        {
            GridLength gridLength;
            if (show)
                gridLength = new GridLength();
            else
                gridLength = new GridLength(0);
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => grdMain.RowDefinitions[1].Height = gridLength);
        }

        private async void SetServerLabel(string label)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => lblServer.Text = label);
        }

        private async void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsDialog settingsDialog = new SettingsDialog();
            settingsDialog.Settings = settings;
            await settingsDialog.ShowAsync();
            if (settingsDialog.Result == SettingsResult.OK)
            {
                settings.Save();
                if (settings.CheckOk())
                {
                    SetServerLabel(settings.Server);
                    await Navigate(new Uri(settings.Server));
                }
            }
        }

        private void browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            try
            {
                string data = e.Value;
                if (data.StartsWith("Clicked:"))
                {
                    ShowStatusBar(true);
                }
            }
            catch (Exception)
            {
                // Could not build a proper Uri. Abandon.
            }
        }

        private void browser_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            ShowStatusBar(false);
        }

        private void browser_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            ShowStatusBar(true);
        }
    }
}
