
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Google.Android.Material.Snackbar;

namespace SandBoxWV
{
    public class PermissionBean
    {
        public PermissionBean(string name) => Name = name;
        public PermissionBean(bool granted, string name) : this(name) => Granted = granted;
        public bool Granted { get; set; }
        public string Name { get; set; }
    }

    public class PermissionServcie
    {
        private IList<string> _permissions;
        private IList<PermissionBean> _permissionBeans;
        public IList<PermissionBean> GetPermissionBeans => _permissionBeans;
        private Activity _activity;
        private string _rationales;
        private const int REQUEST_PERM = 10;

        //ctor
        public PermissionServcie(Activity activity)
        {
            _activity = activity;
            _rationales =
                "Необходимо разрешение для записи в память для кэширования 3D объектов пользователя";
            _permissions = new List<string>();   
                //  Android.Manifest.Permission.RecordAudio
          //  if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Q)
                _permissions.Add(Android.Manifest.Permission.WriteExternalStorage);
        }
        public void RequestPermissions()
        {
            GetPermissionsForRequest();
            var permWithRationale = new List<PermissionBean>();
            var permWithoutRationale = new List<PermissionBean>();
            GetRationalePermissions(permWithRationale, permWithoutRationale);
            RequestRationalePermissions(permWithRationale);
            RequestNonRationalePermissions(permWithoutRationale);
        }


        private void RequestNonRationalePermissions(IList<PermissionBean> permWithoutRationale)
        {
            if (permWithoutRationale.Any())
                ActivityCompat.RequestPermissions(_activity, _permissionBeans.Select(x => x.Name).ToArray(), REQUEST_PERM);
        }
        private void RequestRationalePermissions(IList<PermissionBean> permWithRationale)
        {
            if (permWithRationale.Any())
            {
                Snackbar.Make(_activity.FindViewById(Android.Resource.Id.Content),
                 _rationales,
                 Snackbar.LengthIndefinite)
                 .SetAction("Разрешить",
                    new Action<View>(delegate (View obj) { ActivityCompat.RequestPermissions(_activity, permWithRationale.Select(x => x.Name).ToArray(), REQUEST_PERM); })).Show();

            }
        }
        private void GetRationalePermissions(IList<PermissionBean> permWithRationale, IList<PermissionBean> permWithoutRationale)
        {
            if (_permissionBeans.Any())
            {
                for (int i = 0; i < _permissionBeans.Count; i++)
                {
                    var perm = _permissionBeans.ElementAt(i);
                    if (ActivityCompat.ShouldShowRequestPermissionRationale(_activity, perm.Name))
                        permWithRationale.Add(perm);
                    else
                        permWithoutRationale.Add(perm);
                }
            }
        }
        private void GetPermissionsForRequest()
        {
            _permissionBeans = new List<PermissionBean>();
            for (int i = 0; i < _permissions.Count; i++)
            {
                var permName = _permissions.ElementAt(i);
                var pBean = new PermissionBean(permName);
                if (ContextCompat.CheckSelfPermission(_activity, permName) == (int)Permission.Granted)
                    pBean.Granted = true;
                else
                    _permissionBeans.Add(pBean);
            }
        }
        public void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            if (requestCode == REQUEST_PERM)
            {
                for (int i = 0; i < permissions.Length; i++)
                {
                    var permName = permissions.ElementAt(i);
                    if (grantResults[i] == Permission.Granted)
                    {
                        var permission = _permissionBeans.FirstOrDefault(x => x.Name == permName);
                        if (permission != null)
                        {
                            _permissionBeans.Remove(permission);
                        }
                    }
                    else
                    {
                        Toast.MakeText(_activity ,permName + " - разрешение отклонено", ToastLength.Short).Show();
                    }
                }
            }
        }
    }
}