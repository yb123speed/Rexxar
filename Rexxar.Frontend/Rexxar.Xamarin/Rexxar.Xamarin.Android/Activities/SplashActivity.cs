
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;

namespace Rexxar.Xamarin.Droid.Activities
{
    [Activity(Label = "SplashActivity",
              Icon = "@mipmap/icon",
              Theme = "@style/Splash",
              NoHistory = true,
              MainLauncher = true,
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //InvokeMainActivity();
        }

        protected override void OnResume()
        {
            base.OnResume();
            InvokeMainActivity();
        }

        private void InvokeMainActivity()
        {
            Task.Delay(1000).Wait();
            StartActivity(new Intent(this,typeof(MainActivity)));
        }
    }
}
