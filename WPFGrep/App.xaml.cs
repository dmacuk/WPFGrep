using GalaSoft.MvvmLight.Threading;
using System.Windows;
using Utils.Preference;

namespace WPFGrep
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherHelper.Initialize();
            PreferenceManager.GetInstance("WPFGrep.prefs");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            PreferenceManager.SavePreferences();
        }
    }
}