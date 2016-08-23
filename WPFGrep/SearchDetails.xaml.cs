using System;
using System.Windows;
using Utils.Window;
using WPFGrep.ViewModel;

namespace WPFGrep
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _model;

        /// <summary>
        ///     Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _model = (MainViewModel) DataContext;
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        protected override void OnActivated(EventArgs e)
        {
            this.LoadSettings();
        }


        protected override void OnDeactivated(EventArgs e)
        {
            this.SaveSettings();
            ((MainViewModel) DataContext).SaveState();
        }
    }
}