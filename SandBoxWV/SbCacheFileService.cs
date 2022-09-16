using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SandBoxWV.Extenstions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace SandBoxWV
{
    class SbCacheFileService
    {
        private const string CACHE_FOLDER = "3dotvet_cache";
        private Context _context;

        public SbCacheFileService(Context context)
        {
            _context = context;
        }


        public string GetPathToFile(string url)
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
                using (var client = new HttpClient())
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
}