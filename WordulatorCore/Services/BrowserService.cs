using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using Microsoft.Toolkit.Wpf.UI.Controls;

namespace WordulatorCore.Services
{

    //Note on this class
    /*
     * We use a Webview (using windows toolkit) to render the HTML page of the epub for us,
     * this allows us to extract only the worded content of a particular page so that we avoid having to parse and deal with the HTML ourselves.
    */
     

    public class BrowserService
    {
        //ignoreing compiler warning for obsolescence. 
        //the webview is marked obsolete but Webview2 is not yet available from MS. so using obsolete while the newer doesn't exist.
        #pragma warning disable 618
        private WebView _headlessWebView; //a headless webview, or rather, webview that runs on a thread in the background and not shown to a user
                                            //we make use of the Windows Toolkit webview, representing the one used for UWP.
        #pragma warning restore 618

        public delegate void PageProcessedHandler(string content);

        public event PageProcessedHandler PageProcessed;

        public BrowserService()
        {
            _headlessWebView = new WebView();
            _headlessWebView.BeginInit(); // initialise the component. For some reason this does nothing. 
            _headlessWebView.DOMContentLoaded += HandleDomLoaded;
            _headlessWebView.NavigationCompleted += HandleNavigationCompleted;
            _headlessWebView.Loaded += HandleLoaded;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HandleNavigationCompleted(object sender, WebViewControlNavigationCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }


        public async void LoadPage(string html)
        {
            try
            {
                //this method will currently fire and then do nothing, as the webview is most likely not initialised. So DomContentLoaded never triggers.
                _headlessWebView.NavigateToString(html);   // xxxxxxx   terminates. TODO: figure out why WebView won't initialise in class library app.                            
            }
            catch (Exception ex)
            {
                throw ex;//used for debugging
            }
        }

        private async void HandleDomLoaded(object sender, WebViewControlDOMContentLoadedEventArgs e)
        {
            // This invokes a JS method inside the webview to retrieve the text of the page. 
            var content = await _headlessWebView.InvokeScriptAsync("eval", new[] { "document.body.innerText" });
            //invoke processed method with the parsed text from the webview.
            PageProcessed?.Invoke(content);
        }
    }
}
