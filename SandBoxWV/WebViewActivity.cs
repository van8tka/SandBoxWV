using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System;

namespace SandBoxWV.Resources
{
    [Activity(Label = "WebViewActivity")]
    public class WebViewActivity : Activity
    {
        private const string URL_TAG="url_tag";
        private WebView _webView;
        public static void NewInstance(Activity activity, string url)
        {
            var intent = new Intent(activity, typeof(WebViewActivity));
            intent.PutExtra(URL_TAG, url);
            activity.StartActivity(intent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.web_layout);
                InitWV();
            }
            catch (Exception er)
            {
                SbLog.E(er);
                Toast.MakeText(this, "WebViewActivity: "+ er.Message, ToastLength.Long).Show();
            }
        }

        private void InitWV()
        {
            _webView = FindViewById<WebView>(Resource.Id.webView);
            _webView.SetWebViewClient(new SbWebViewClient());
            _webView.Settings.JavaScriptEnabled = true;
            _webView.LoadUrl(Intent.GetStringExtra(URL_TAG));
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back && _webView.CanGoBack())
            {
                _webView.GoBack();
                return true;
            }
            return base.OnKeyDown(keyCode, e);
        }
    }
}