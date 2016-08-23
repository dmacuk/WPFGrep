using System;
using System.Windows;
using Utils.Preference;
using Utils.Window;
using WPFGrep.ViewModel;

namespace WPFGrep
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string StartDirectory = "StartDirectory";
        private const string FileTypes = "FileTypes";
        private const string SearchFor = "SearchFor";
        private const string SearchSubDirectories = "SearchSubDirectories";
        private readonly MainViewModel _model;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _model = (MainViewModel)DataContext;
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        protected override void OnActivated(EventArgs e)
        {
            this.LoadSettings();
            LoadState();
        }

        private void LoadState()
        {
            _model.StartDirectory = PreferenceManager.GetPreference(StartDirectory, "");
            _model.FileTypes = PreferenceManager.GetPreference(FileTypes, "");
            _model.SearchFor = PreferenceManager.GetPreference(SearchFor, "");
            _model.SearchSubDirectories = PreferenceManager.GetPreference(SearchSubDirectories, false);
        }

        protected override void OnDeactivated(EventArgs e)
        {
            this.SaveSettings();
            SaveState();
        }

        private void SaveState()
        {
            PreferenceManager.SetPreference(StartDirectory, _model.StartDirectory);
            PreferenceManager.SetPreference(FileTypes, _model.FileTypes);
            PreferenceManager.SetPreference(SearchFor, _model.SearchFor);
            PreferenceManager.SetPreference(SearchSubDirectories, _model.SearchSubDirectories);
        }
    }
}