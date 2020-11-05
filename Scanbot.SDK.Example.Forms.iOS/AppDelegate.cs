using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using ScanbotSDK.iOS;
using ScanbotSDK.Xamarin.Forms;
using UIKit;

namespace Scanbot.SDK.Example.Forms.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        // TODO Add the Scanbot SDK license key here.
        // Please note: The Scanbot SDK will run without a license key for one minute per session!
        // After the trial period is over all Scanbot SDK functions as well as the UI components
        // will stop working or may be terminated. You can get an unrestricted
        // "no-strings-attached" 30 day trial license key for free.
        // Please submit the trial license form (https://scanbot.io/en/sdk/demo/trial)
        // on our website by using the app identifier
        // "io.scanbot.example.sdk.xamarin.forms" of this example app.
        const string LICENSE_KEY = null;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            ImagePicker.Forms.iOS.DependencyManager.Register();

            Console.WriteLine("Scanbot SDK Example: Initializing Scanbot SDK...");
            SBSDKInitializer.Initialize(app, LICENSE_KEY, new SBSDKConfiguration
            {
                EnableLogging = true,
                StorageBaseDirectory = GetDemoStorageBaseDirectory(),
                DetectorType = DocumentDetectorType.MLBased
            });

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        string GetDemoStorageBaseDirectory()
        {
            // For demo purposes we use a sub-folder in the Documents folder in the
            // Data Container of this App, since the contents can be shared via iTunes.
            // For more detais about the iOS file system see:
            // - https://developer.apple.com/library/archive/documentation/FileManagement/Conceptual/FileSystemProgrammingGuide/FileSystemOverview/FileSystemOverview.html
            // - https://docs.microsoft.com/en-us/xamarin/ios/app-fundamentals/file-system
            // - https://docs.microsoft.com/en-us/dotnet/api/system.environment.specialfolder

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var customDocumentsFolder = Path.Combine(documents, "my-custom-storage");
            Directory.CreateDirectory(customDocumentsFolder);

            return customDocumentsFolder;
        }

        static TaskCompletionSource<bool> source;
        public static Task<bool> StartMultipleObjectsScanner()
        {
            // iOS does not have a sweet internal wrapper, so we have to do it the hard way.
            // Create the task, return that one right away...
            // and later execute it in the handler we create.
            source = new TaskCompletionSource<bool>();

            var configuration = new SBSDKUIMultipleObjectScannerConfiguration(
                new SBSDKUIMultipleObjectScannerUIConfiguration { },
                new SBSDKUIMultipleObjectScannerTextConfiguration { FlashButtonTitle = "Pew!" },
                new SBSDKUIMultipleObjectScannerBehaviorConfiguration { });

            // Implement custom delegate 
            var handler = new MultipleObjectResultHandler();
            var controller = SBSDKUIMultipleObjectScannerViewController.CreateNewWithConfiguration(configuration, handler);

            // Access the root view controller. Don't worry, the root is the only one available anyway
            var root = UIApplication.SharedApplication.KeyWindow.RootViewController;
            // Make sure it's full screen
            controller.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
            root.PresentViewController(controller, true, null);

            return source.Task;
        }

        class MultipleObjectResultHandler : SBSDKUIMultipleObjectScannerViewControllerDelegate
        {
            public override void DidFinishWithDocument(SBSDKUIMultipleObjectScannerViewController viewController, SBSDKUIDocument document)
            {
                // And here we, again, have our pages that can be accessed
                // and relevant information retrieved via document.PageAtIndex
                var pageCount = document.NumberOfPages;
                // As with android, just return true to prove we've been here
                source.SetResult(true);
            }
        }
    }
}
