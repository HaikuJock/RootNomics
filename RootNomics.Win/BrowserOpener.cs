using Haiku.MonoGameUI;
using System;
using System.Diagnostics;
#if WINDOWS_UWP
using Windows.System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
#endif

namespace RootNomics.Win
{
    class BrowserOpener : BrowserOpening
    {
        public void OpenUrl(Uri url)
        {
#if WINDOWS_UWP
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Launcher.LaunchUriAsync(url));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#else
            var urlString = url.AbsoluteUri.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {urlString}") { CreateNoWindow = true });
#endif
        }
    }
}
