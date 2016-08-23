using System;
using System.Collections.Generic;
using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Gat.Controls;
using Utils.Preference;
using WPFGrep.Utilities;
using WPFGrep.VOs;

namespace WPFGrep.ViewModel
{
    /// <summary>
    ///     This class contains properties that the main View can data bind to.
    ///     <para>
    ///         See http://www.mvvmlight.net
    ///     </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private const string PrefStartDirectory = "StartDirectory";
        private const string PrefFileTypes = "FileTypes";
        private const string PrefSearchFor = "SearchFor";
        private const string PrefSearchSubDirectories = "SearchSubDirectories";

        private readonly Dictionary<FileInfo, List<GrepResult>> _results = new Dictionary<FileInfo, List<GrepResult>>();
        private readonly ThreadSafeList<GrepSearchWorker> _searchWorkers = new ThreadSafeList<GrepSearchWorker>();
        private bool _dirty;

        private string _fileTypes;
        private string _searchFor;
        private bool _searchSubDirectories;
        private string _startDirectory;

        public MainViewModel()
        {
            if (IsInDesignMode)
                StartDirectory = "c:\\temp";
            else
                LoadState();
        }

        public string FileTypes
        {
            get { return _fileTypes; }
            set
            {
                if (Set(() => FileTypes, ref _fileTypes, value)) _dirty = true;
            }
        }

        public RelayCommand GetStartDirectoryCommand => new RelayCommand(GetStartDirectory);

        public RelayCommand SearchCommand => new RelayCommand(SearchCommandExecute, SearchCommandCanExecute);

        public string SearchFor
        {
            get { return _searchFor; }
            set
            {
                if (Set(() => SearchFor, ref _searchFor, value))
                    _dirty = true;
            }
        }

        public bool Searching { get; set; }

        public bool SearchSubDirectories
        {
            get { return _searchSubDirectories; }
            set
            {
                if (Set(() => SearchSubDirectories, ref _searchSubDirectories, value)) _dirty = true;
            }
        }

        public string StartDirectory
        {
            get { return _startDirectory; }
            set
            {
                if (Set(() => StartDirectory, ref _startDirectory, value)) _dirty = true;
            }
        }

        public RelayCommand StopCommand => new RelayCommand(StopCommandExecute, StopCommandCanExecute);

        private void LoadState()
        {
            StartDirectory = PreferenceManager.GetPreference(PrefStartDirectory, "");
            FileTypes = PreferenceManager.GetPreference(PrefFileTypes, "");
            SearchFor = PreferenceManager.GetPreference(PrefSearchFor, "");
            SearchSubDirectories = PreferenceManager.GetPreference(PrefSearchSubDirectories, false);
        }

        private void GetStartDirectory()
        {
            var openDialog = new OpenDialogView();
            var vm = (OpenDialogViewModel) openDialog.DataContext;

            //            // Adding file filter
            //            vm.AddFileFilterExtension(".txt");
            //
            //            // Show dialog and take result into account
            //            bool? result = vm.Show();
            //            if (result == true)
            //            {
            //                // Get selected file path
            //                StartDirectory = vm.SelectedFilePath;
            //            }
            //            else
            //            {
            //                StartDirectory = string.Empty;
            //            }

            // Setting date format by using predefined date format
            //            vm.DateFormat = OpenDialogViewModelBase.ISO8601_DateFormat;

            // Setting folder dialog
            vm.IsDirectoryChooser = true;
            vm.SelectedFilePath = StartDirectory;

            // Show dialog and take result into account
            var result = vm.Show();
            if (result == true) StartDirectory = vm.SelectedFilePath;

            //            // Setting save dialog
            //            vm.IsDirectoryChooser = false;
            //            vm.IsSaveDialog = true;
            //            vm.Show();
            //
            //            // Customize UI texts
            //            vm.CancelText = "Abort";
            //            vm.Caption = "Caption";
            //            vm.DateFormat = "yy_MM_dd HH:mm:ss";
            //            vm.DateText = "DateTime";
            //            vm.FileFilterText = "File extension";
            //            vm.FileNameText = "File path";
            //            vm.NameText = "File";
            //            vm.SaveText = "Store";
            //            vm.SizeText = "Length";
            //            vm.TypeText = "File Type";
            //
            //            // Setting window properties
            ////            vm.Owner = MainWindow;
            ////            vm.StartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            //
            //            // Show
            //            vm.Show();
        }

        private void MatchFound(object sender, MatchFoundEventArgs e)
        {
            switch (e.GrepSearchEvent)
            {
                case GrepSearchEvent.MatchFound:
                    if (!_results.ContainsKey(e.File)) _results.Add(e.File, new List<GrepResult>());
                    _results[e.File].Add(new GrepResult
                    {
                        LineNumner = e.LineNumber,
                        Line = e.Line
                    });
                    break;

                case GrepSearchEvent.Finished:
                    _searchWorkers.Remove((GrepSearchWorker) sender);
                    if (_searchWorkers.Count == 0) Searching = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Invalid GrepSearchEvent: " + e.GrepSearchEvent);
            }
        }

        private bool SearchCommandCanExecute()
        {
            return !Searching && !string.IsNullOrWhiteSpace(SearchFor) && !string.IsNullOrWhiteSpace(FileTypes) &&
                   !string.IsNullOrWhiteSpace(StartDirectory);
        }

        private void SearchCommandExecute()
        {
            Searching = true;
            _searchWorkers.Clear();
            var filetypes = _fileTypes.Split(';');
            foreach (var filetype in filetypes)
            {
                var worker = new GrepSearchWorker(_startDirectory, filetype, _searchFor, _searchSubDirectories);
                _searchWorkers.Add(worker);
                worker.MatchFound += MatchFound;
                worker.Search();
            }
        }

        private bool StopCommandCanExecute()
        {
            return Searching;
        }

        private void StopCommandExecute()
        {
            foreach (var search in _searchWorkers)
                search.Stop();
        }

        public void SaveState()
        {
            PreferenceManager.SetPreference(PrefStartDirectory, StartDirectory);
            PreferenceManager.SetPreference(PrefFileTypes, FileTypes);
            PreferenceManager.SetPreference(PrefSearchFor, SearchFor);
            PreferenceManager.SetPreference(PrefSearchSubDirectories, SearchSubDirectories);
        }
    }
}