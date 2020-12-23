namespace PlayNiteWidget
{
    using Microsoft.Gaming.XboxGameBar;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.ApplicationModel.Activation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    internal sealed partial class App : Application
    {
        /// <summary>
        /// Defines the uriToLaunch.
        /// </summary>
        internal static string uriToLaunch = @"playnitefullscreen://";

        /// <summary>
        /// Defines the appLaunched.
        /// </summary>
        private bool appLaunched = false;

        /// <summary>
        /// Defines the playniteWidget.
        /// </summary>
        private XboxGameBarWidget playniteWidget = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The OnActivated.
        /// </summary>
        /// <param name="args">The args<see cref="IActivatedEventArgs"/>.</param>
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

            // Open Playnite straight away
            OpenPlaynite();
        }

        /// <summary>
        /// This is triggered on a window state change and auto-closes the widget when it gets restored.
        /// </summary>
        /// <param name="sender">The sender<see cref="XboxGameBarWidget"/>.</param>
        /// <param name="args">The args<see cref="object"/>.</param>
        private void Widget1_WindowStateChanged(XboxGameBarWidget sender, object args)
        {
            if (playniteWidget.WindowState == XboxGameBarWidgetWindowState.Restored)
            {
                QueueForWidgetClose();
            }
        }

        /// <summary>
        /// Queues the widget for closing once playnite has opened.
        /// </summary>
        private void QueueForWidgetClose()
        {
            // If app isn't launched, try again in 1 second
            // Otherwise close the widget
            if (!appLaunched)
            {
                Task.Delay(new TimeSpan(0, 0, 1)).ContinueWith(o => {
                    QueueForWidgetClose();
                });
            } else
            {
                appLaunched = false;
                playniteWidget.Close();
            }
        }

        /// <summary>
        /// Opens Playnite.
        /// </summary>
        private async void OpenPlaynite()
        {
            Uri uri = new Uri(uriToLaunch);
            await Windows.System.Launcher.LaunchUriAsync(uri); // Call once to start the app

            // Call again to bring to focus
            Thread.Sleep(1000);
            await Windows.System.Launcher.LaunchUriAsync(uri);
            appLaunched = true;
        }
    }
}
