using Android.App;
using Android.Content.PM;
using Android.OS;

namespace SeriDoviz
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? bundle)
        {
            base.OnCreate(bundle);
            var col = Android.Graphics.Color.ParseColor("#0E1E3D");
            Window?.SetStatusBarColor(col);
            Window?.SetNavigationBarColor(col);
        }

    }

}
