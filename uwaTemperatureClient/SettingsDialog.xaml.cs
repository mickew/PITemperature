using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace uwaTemperatureClient
{
    public enum SettingsResult
    {
        OK,
        Fail,
        Cancel,
        Nothing
    }

    public sealed partial class SettingsDialog : ContentDialog
    {
        private PITSettings _settings;
        public SettingsResult Result { get; private set; }
        public PITSettings Settings { get { return _settings; } set { _settings = value; SetGui(value); } }
        public SettingsDialog()
        {
            this.InitializeComponent();

            OSK_TextBox.RegisterEditControl(serverTextBox);
        }

        private void SetGui(PITSettings settings)
        {
            //serverTextBox.Text = settings.Server;
            OSK_TextBox.OutputString = settings.Server;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Result = SettingsResult.Cancel;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            SetSettings();
            Result = SettingsResult.OK;
        }

        private void SetSettings()
        {
            _settings.Server = serverTextBox.Text;
        }

        private void OSK_Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (OSK_TextBox.Visibility == Visibility.Collapsed)
            {
                OSK_TextBox.Visibility = Visibility.Visible;
            }
            else
            {
                OSK_TextBox.Visibility = Visibility.Collapsed;
            }

            /* Need to set focus so the on screen keyboard's content buffer gets updated */
            serverTextBox.Focus(FocusState.Programmatic);
        }
    }
}
