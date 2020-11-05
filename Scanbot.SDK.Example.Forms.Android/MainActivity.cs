using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using IO.Scanbot.Sdk.UI.View.Multiple_objects;
using IO.Scanbot.Sdk.UI.View.Multiple_objects.Configuration;
using ScanbotSDK.Xamarin.Forms.Android;
using System.Collections.Generic;
using ScanbotSDK.Xamarin.Forms;
using System.Threading.Tasks;

namespace Scanbot.SDK.Example.Forms.Droid
{
    [Activity(Label = "Scanbot.SDK.Example.Forms", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public static async Task<bool> StartMultipleObjectScanner()
        {
            // ScanbotSDK Internal API to get activity context
            var context = ScanbotSDK.Xamarin.Android.SBSDK.ApplicationContext;

            // Native implementation of MultipleObjectsDetector
            // You can either set the configuration here on the native level,
            // or write your own common wrapper class to control the configuration
            var configuration = new MultipleObjectsDetectorConfiguration();
            configuration.SetFlashButtonTitle("Pew!");
            var intent = MultipleObjectsDetectorActivity.NewIntent(context, configuration);

            // ScanbotSDK internal wrapper to "await" activity results,
            // to avoid implementing a result handler for each Scanner
            // This is not guaranteed to work with future updates and changes
            // will not be documented. Feel free to implement your own solution
            var result = await ActivityResultDispatcher
                .ReceiveProxyActivityResult(context, intent);

            // And here to have the native pages.
            // Convert the necessary data to any common format and retrive it
            var nativePages = MultipleObjectsDetectorActivity.ParseActivityResult(result.Intent);

            return true;
        }
    }
}