using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace RapidCMS.Example.Maui
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}