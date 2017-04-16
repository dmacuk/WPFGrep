using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utils.ObservableDictionary;
using Utils.Preference;
using WPFGrep.Utilities;
using WPFGrep.VOs;

namespace WPFGrep.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private const string PrefStartDirectory = "StartDirectory";
        private const string PrefFileTypes = "FileTypes";
        private const string PrefSearchFor = "SearchFor";
        private const string PrefSearchSubDirectories = "SearchSubDirectories";
        private const string PrefIgnoreCase = "SearchSubDirectories";
        private const string PrefNumberOfLines = "NumberOfLines";

        private readonly ThreadSafeList<GrepSearchWorker> _searchWorkers = new ThreadSafeList<GrepSearchWorker>();
        private string _fileTypes;
        private bool _ignoreCase;
        private int _numberOfLines;

        private ObservableDictionary<string, List<GrepResult>> _results =
            new ObservableDictionary<string, List<GrepResult>>();

        private string _searchFor;
        private bool _searching;
        private bool _searchSubDirectories;

        private string _startDirectory;

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                StartDirectory = "c:\\temp";
                var grepResult1 = new GrepResult
                {
                    Lines = new List<string>
                    {
                        "Line 1",
                        "Line 3",
                        "Line 3",
                        "Line 4"
                    }
                };
                var grepResult2 = new GrepResult
                {
                    Lines = new List<string>
                    {
                        "Line 5",
                        "Line 6",
                        "Line 7",
                        "Line 8"
                    }
                };
                _results.Add("Filename Here", new List<GrepResult> { grepResult1, grepResult2 });
                _results.Add("Filename 2 Here", new List<GrepResult> { grepResult1, grepResult2 });
            }
            else
            {
                LoadState();
                StartDirectory = "D:\\Documents\\Visual Studio 2015\\Projects";
            }
        }

        public string FileTypes
        {
            get => _fileTypes;
            set { Set(() => FileTypes, ref _fileTypes, value); }
        }

        public RelayCommand GetStartDirectoryCommand => new RelayCommand(GetStartDirectory);

        public RelayCommand SearchCommand => new RelayCommand(SearchCommandExecute, SearchCommandCanExecute);

        public string SearchFor
        {
            get => _searchFor;
            set { Set(() => SearchFor, ref _searchFor, value); }
        }

        private bool Searching
        {
            get => _searching;
            set { Set(() => Searching, ref _searching, value); }
        }

        public bool SearchSubDirectories
        {
            get => _searchSubDirectories;
            set { Set(() => SearchSubDirectories, ref _searchSubDirectories, value); }
        }

        public ObservableDictionary<string, List<GrepResult>> Results
        {
            get => _results;
            set { Set(() => Results, ref _results, value); }
        }

        public string StartDirectory
        {
            get => _startDirectory;
            set { Set(() => StartDirectory, ref _startDirectory, value); }
        }

        public RelayCommand StopCommand => new RelayCommand(StopCommandExecute, StopCommandCanExecute);

        public bool IgnoreCase
        {
            get => _ignoreCase;
            set { Set(() => IgnoreCase, ref _ignoreCase, value); }
        }

        public int NumberOfLines
        {
            get => _numberOfLines;
            set { Set(() => NumberOfLines, ref _numberOfLines, value); }
        }

        private void LoadState()
        {
            StartDirectory = PreferenceManager.GetPreference(PrefStartDirectory, "");
            FileTypes = PreferenceManager.GetPreference(PrefFileTypes, "");
            SearchFor = PreferenceManager.GetPreference(PrefSearchFor, "");
            SearchSubDirectories = PreferenceManager.GetPreference(PrefSearchSubDirectories, false);
            IgnoreCase = PreferenceManager.GetPreference(PrefIgnoreCase, false);
            NumberOfLines = PreferenceManager.GetPreference(PrefNumberOfLines, 1);
        }

        private void GetStartDirectory()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = StartDirectory
            };
            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
                StartDirectory = dialog.FileName;
        }

        private void MatchFound(object sender, MatchFoundEventArgs e)
        {
            switch (e.GrepSearchEvent)
            {
                case GrepSearchEvent.MatchFound:
                    var fileName = e.File.FullName;
                    var grepResult = new GrepResult
                    {
                        FileName = fileName,
                        LineNumber = e.LineNumber,
                        Lines =
                            File.ReadLines(fileName)
                                .Skip(e.LineNumber - _numberOfLines)
                                .Take(_numberOfLines + 1)
                                .ToList()
                    };
                    DispatcherHelper.CheckBeginInvokeOnUI(
                        () =>
                        {
                            if (!_results.ContainsKey(fileName)) _results.Add(fileName, new List<GrepResult>());
                            _results[fileName].Add(grepResult);
                            //                                                Set(() => Results, ref _results, new Dictionary<string, List<GrepResult>>(_results));
                        });
                    break;

                case GrepSearchEvent.Finished:
                    _searchWorkers.Remove((GrepSearchWorker)sender);
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
            Results.Clear();
            Searching = true;
            _searchWorkers.Clear();
            var filetypes = _fileTypes.Split(';');
            foreach (var filetype in filetypes)
            {
                var worker = new GrepSearchWorker(_startDirectory, filetype, _searchFor, _searchSubDirectories,
                    _ignoreCase);
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
            PreferenceManager.SetPreference(PrefIgnoreCase, IgnoreCase);
            PreferenceManager.SetPreference(PrefNumberOfLines, NumberOfLines);
        }
    }
}