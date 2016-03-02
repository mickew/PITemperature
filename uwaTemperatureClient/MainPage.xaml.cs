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
using Windows.System;
using Windows.Devices.Gpio;
using Windows.System.Threading;
using Windows.UI.Popups;

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
        private bool statusBarIsShownByScriptNotify = false;
        private bool statusBarVisible = false;
        private const int BUTTON_PIN = 5;
        private GpioPin buttonPin;
        private ThreadPoolTimer buttonShutDownTimer;



        public MainPage()
        {
            this.InitializeComponent();
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            settings = new PITSettings();
            settings.Load();
            if (settings.CheckOk())
            {
                SetServerLabel(settings.Server);
#pragma warning disable 4014
                Navigate(new Uri(settings.Server));
#pragma warning restore 4014
            }
            InitGPIO();
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();
            if (gpio != null)
            {
                buttonPin = gpio.OpenPin(BUTTON_PIN);
                // Check if input pull-up resistors are supported
                if (buttonPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                    buttonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
                else
                    buttonPin.SetDriveMode(GpioPinDriveMode.Input);

                // Set a debounce timeout to filter out switch bounce noise from a button press
                buttonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);
                buttonPin.ValueChanged += ButtonPin_ValueChanged;
            }
        }

        private void ButtonPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.FallingEdge)
            {
                if (statusBarVisible)
                {
                    ShowStatusBar(false);
                    statusBarIsShownByScriptNotify = false;
                }
                else
                {
                    ShowStatusBar(true);
                    statusBarIsShownByScriptNotify = true;
                }
                buttonShutDownTimer = ThreadPoolTimer.CreateTimer(buttonShutDownTimerElapsedHandler, TimeSpan.FromSeconds(3));
            }
            else
            {
                if (buttonShutDownTimer != null)
                {
                    buttonShutDownTimer.Cancel();
                    buttonShutDownTimer = null;
                }
            }
        }

        public void buttonShutDownTimerElapsedHandler(ThreadPoolTimer timer)
        {
            if (buttonPin.Read() == GpioPinValue.Low)
            {
                buttonShutDownTimer = null;
                ShutDown();
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
                await WebView.ClearTemporaryWebDataAsync();
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
            {
                statusBarVisible = true;
                gridLength = new GridLength();
            }
            else
            {
                statusBarVisible = false;
                gridLength = new GridLength(0);
            }
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
                    statusBarIsShownByScriptNotify = false;
                }
            }
            if (statusBarIsShownByScriptNotify)
            {
                ShowStatusBar(false);
                statusBarIsShownByScriptNotify = false;
            }
        }

        private void browser_ScriptNotify(object sender, NotifyEventArgs e)
        {
            try
            {
                string data = e.Value;
                if (data.StartsWith("Clicked:"))
                {
                    if (statusBarVisible)
                    {
                        ShowStatusBar(false);
                        statusBarIsShownByScriptNotify = false;
                    }
                    else
                    {
                        ShowStatusBar(true);
                        statusBarIsShownByScriptNotify = true;
                    }
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

        private void btnTurnOff_Click(object sender, RoutedEventArgs e)
        {
            ShutDown();
        }

        private void ShutDown()
        {

#pragma warning disable 4014
            _dispatcher.RunAsync(CoreDispatcherPriority.Normal,() =>
            {
                var dialog = new ContentDialog()
                {
                    Title = "PI Temperature Shutdown",
                    MaxWidth = this.ActualWidth // Required for Mobile!
                };

                // Setup Content
                var panel = new StackPanel();
                panel.Children.Add(new TextBlock
                {
                    Text = "System is shutting down!.",
                    TextWrapping = TextWrapping.Wrap,
                });

                dialog.Content = panel;

                //// Add Buttons
                //dialog.PrimaryButtonText = "OK";
                //dialog.SecondaryButtonText = "Cancel";

                // Show Dialog
                dialog.ShowAsync();
            });
#pragma warning restore 4014
            ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, new TimeSpan(0,0,2));
        }
    }
}
