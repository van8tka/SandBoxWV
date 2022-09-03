using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Java.Lang;
using Realms;
using SandBoxWV.Resources;
using System.Collections.Generic;
using System.Linq;

namespace SandBoxWV
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private SandBoxDb _db;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.activity_main);
                _db = new SandBoxDb();
                var edit = FindViewById<EditText>(Resource.Id.txtURL);
                var btn = FindViewById<Button>(Resource.Id.btnGo);
                btn.Click += (_, e) =>
                {
                    string url = edit.Text.Trim();
                    if (string.IsNullOrEmpty(url))
                    {
                        edit.Error = "Ошибка ввода URL адреса";
                        return;
                    }
                    GoToWebView(url);
                };

                var lv = FindViewById<RecyclerView>(Resource.Id.recycler_view);
                _history = _db.Get().ToList();
                _adapter = new HistoryAdapter(_history);
                _adapter.ItemClick += (_, e) =>
                {
                    GoToWebView(_history.ElementAt(e));
                };
                lv.SetAdapter(_adapter);
            }
            catch(System.Exception e)
            {
                Toast.MakeText(Application, "MainActivity: "+e.Message, ToastLength.Long).Show();
            }
        }
        private HistoryAdapter _adapter;
        IList<string> _history;
        private void GoToWebView(string url)
        {
            if (url.StartsWith("http://"))
            {
                url = url.Replace("http://", "https://");
            }
            else if (!url.StartsWith("https://"))
                url = "https://" + url;
            _db.AddHistory(url);
            _history.Insert(0, url);
            _adapter.NotifyItemInserted(0);
            WebViewActivity.NewInstance(this, url);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    public class History : RealmObject
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Url { get; set; }
    }


    public class SandBoxDb
    {
        private Realm _realm;
        public SandBoxDb()
        {
              _realm = Realm.GetInstance();
        }

        public IEnumerable<string> Get()
        {
            var items = _realm.All<History>().OrderByDescending(x => x.Id);
            if(items!=null && items.Any())
                return items.AsEnumerable<History>().Take(50).Select(x=>x.Url);
            return new string[] { };  
        }

        public void AddHistory(string url)
        {
            _realm.Write(() =>
            {
                _realm.Add(new History { Url = url, Id = GetNextKey() });
            });
        }

        private int GetNextKey() 
        {
            try 
            {
                int number = 0;
                var items = _realm.All<History>();
                if(items!=null && items.Any())
                {
                    number = items.AsEnumerable().Max(x => x.Id)+1;
                }
                return number; 
            } 
            catch (IndexOutOfBoundsException e)
            { 
                return 0; 
            }
        }
       
    }


}