using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Slot.Droid
{
    [Activity(Label = "Slot",ScreenOrientation =Android.Content.PM.ScreenOrientation.Portrait,Theme ="@style/Splash",Icon ="@drawable/sloticon",MainLauncher =true,NoHistory =true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartActivity(typeof(MainActivity));
            // Create your application here
        }
    }
}