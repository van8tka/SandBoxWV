using Android.Webkit;
using System;
using System.IO;

namespace SandBoxWV
{
    internal sealed class SbWebViewClient : WebViewClient
    {
        private const string URL_HOST = "https://3dotvet.ru/";
        private const string CATCH_FBX_TEXT = "fbx";
        private const string MIME_TYPE = "application/octet-stream";
        private const string ENCODING = "utf-8";


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
                var dn = new SbCacheFileService(view.Context);
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
 
        public WebResourceResponse ResponseCachedFile(SbCacheFileService dn, string url)
        {
            var st = new FileStream(dn.GetPathToFile(url), FileMode.Open, FileAccess.Read); 
            return new WebResourceResponse(MIME_TYPE, ENCODING , st);
        }
    }
}