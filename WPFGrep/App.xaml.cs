using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Utils.Preference;

namespace WPFGrep
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            PreferenceManager.GetInstance("WPFGrep.prefs");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            PreferenceManager.SavePreferences();
        }
    }
}
