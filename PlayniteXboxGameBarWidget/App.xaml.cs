using System;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Gaming.XboxGameBar;

namespace PlayNiteWidget
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        internal static string uriToLaunch = @"playnitefullscreen://";
        private XboxGameBarWidget playniteWidget = null;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            XboxGameBarWidgetActivatedEventArgs widgetArgs = null;
            if (args.Kind == ActivationKind.Protocol)
            {
                var protocolArgs = args as IProtocolActivatedEventArgs;
                string scheme = protocolArgs.Uri.Scheme;
                if (scheme.Equals("ms-gamebarwidget"))
                {
                    widgetArgs = args as XboxGameBarWidgetActivatedEventArgs;
                }
            }
            if (widgetArgs != null)
            {
                if (widgetArgs.IsLaunchActivation)
                {
                    Frame rootFrame = new Frame();
                    Window.Current.Content = rootFrame;

                    // Create Game Bar widget object which bootstraps the connection with Game Bar
                    playniteWidget = new XboxGameBarWidget(
                        widgetArgs,
                        Window.Current.CoreWindow,
                        rootFrame);
                    playniteWidget.WindowStateChanged += Widget1_WindowStateChanged;
                }
            }

            OpenPlayniteAsync();
        }

        private void Widget1_WindowStateChanged(XboxGameBarWidget sender, object args)
        {
            if (playniteWidget.WindowState == XboxGameBarWidgetWindowState.Restored)
            {
                playniteWidget.Close();
            }
        }


        /// <summary>
        /// The OpenPlaynite.
        /// </summary>
        /// <returns>The <see cref="IAsyncOperation{bool}"/>.</returns>
        public async void OpenPlayniteAsync()
        {
            Uri uri = new Uri(uriToLaunch);
            await Windows.System.Launcher.LaunchUriAsync(uri);

        }
    }
}
