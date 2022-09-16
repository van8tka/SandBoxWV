using Android.Content;
using Android.Webkit;
using Android.Widget;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SandBoxWV
{
    class SbWebViewClient : WebViewClient
    {
        private const string URL_HOST = "https://3dotvet.ru/";
        private const string CATCH_FBX_TEXT = "fbx";
       
        public override bool ShouldOverrideUrlLoading(WebView view, IWebResourceRequest request)
        {
            var url = request.Url.ToString();
            SbLog.I("SbWebViewClient_OverrideUrl", url);
            view.LoadUrl(url);
            return false;
        }

        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            var url = request.Url.Path;
            SbLog.I("SbWebViewClient_InterceptRequest", url);
           
            if (url.EndsWith(CATCH_FBX_TEXT, StringComparison.OrdinalIgnoreCase))
            {
                var dn = new AndroidDownloader(view.Context);
                if (!dn.IsExistFile(URL_HOST + url))
                {
                    dn.DownloadFile(URL_HOST + url);
                    return base.ShouldInterceptRequest(view, request);
                }
                else
                    return ResponseCachedFile(dn, URL_HOST + url); 
            }
            else
            {
                return base.ShouldInterceptRequest(view, request);
            }
        }
 

        //upload to webview
        public WebResourceResponse ResponseCachedFile(AndroidDownloader dn, string url)
        {
            var st = new FileStream(dn.GetPathToFile(url), FileMode.Open, FileAccess.Read); 
            return new WebResourceResponse("application/octet-stream", "utf-8", st);
        }
    }


    public class AndroidDownloader
    {
        private const string CACHE_FOLDER = "3dotvet_cache";
        private Context _context;
        public AndroidDownloader(Context context)
        {
            _context = context;
        }


        public string GetPathToFile(string url )
        {
            string pathToNewFolder = string.Empty;
            try
            {
                pathToNewFolder = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, CACHE_FOLDER);
                Directory.CreateDirectory(pathToNewFolder);
            }
            catch (UnauthorizedAccessException e)
            {
                Toast.MakeText(_context, "Добавьте разрешение памяти в настройках смартфона", ToastLength.Long).Show();
            }
            string pathToNewFile = Path.Combine(pathToNewFolder, Path.GetFileName(url));
            return pathToNewFile;
        }


        public async void DownloadFile(string url)
        {
            try
            {
                using (var client = new  HttpClient())
                await client.DownloadFileTaskAsync(new Uri(url), GetPathToFile(url));
            }
            catch (Exception e)
            {
                SbLog.E(e);
                Toast.MakeText(_context, e.Message, ToastLength.Long).Show();
                Debugger.Break();
            }
        }

        public bool IsExistFile(string url, long lenght = -1)
        {
            var pathFile = GetPathToFile(url);
            var exist = File.Exists(pathFile);
            if (!exist)
                return false;
            var lengthExist = new FileInfo(pathFile).Length;
            if (lenght > -1 && lengthExist != lenght)
                return false;
            return true;
        }
    }

    public static class HttpClientUtils
    {
        public static async Task DownloadFileTaskAsync(this HttpClient client, Uri uri, string FileName)
        {
            using (var s = await client.GetStreamAsync(uri))
            {
                using (var fs = new FileStream(FileName, FileMode.CreateNew))
                {
                    await s.CopyToAsync(fs);
                }
            }
        }
    }
}