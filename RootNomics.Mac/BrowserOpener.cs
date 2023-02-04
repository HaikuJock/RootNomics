using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
//using AppKit;
//using Foundation;
using Haiku.MonoGameUI;
using Microsoft.Xna.Framework.Input;

namespace RootNomics.Mac
{
    class BrowserOpener : BrowserOpening
    {
        public void OpenUrl(Uri url)
        {
            //NSWorkspace.SharedWorkspace.OpenURL(
            //    NSUrl.FromString(url.ToString()),
            //    NSWorkspaceLaunchOptions.Default,
            //    new NSDictionary(),
            //    out NSError error);
        }
    }
}
