﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Gat.Controls;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WPFGrep.ViewModel.Utilities;
using WPFGrep.ViewModel.VOs;

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
        private readonly Dictionary<FileInfo, List<GrepResult>> _results = new Dictionary<FileInfo, List<GrepResult>>();
        private bool _dirty;

        private string _fileTypes;
        private string _searchFor;
        private bool _searchSubDirectories;
        private string _startDirectory;

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                StartDirectory = "c:\\temp";
            }
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

        private void GetStartDirectory()
        {
            var openDialog = new OpenDialogView();
            var vm = (OpenDialogViewModel)openDialog.DataContext;

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
            if (!_results.ContainsKey(e.File)) _results.Add(e.File, new List<GrepResult>());
            _results[e.File].Add(new GrepResult
            {
                LineNumner = e.LineNumber,
                Line = e.Line
            });
        }

        private bool SearchCommandCanExecute()
        {
            return !string.IsNullOrWhiteSpace(SearchFor) &&
                   !string.IsNullOrWhiteSpace(FileTypes) &&
                   !string.IsNullOrWhiteSpace(StartDirectory);
        }

        private void SearchCommandExecute()
        {
            var filetypes = _fileTypes.Split(';');
            Parallel.ForEach(filetypes, fileType =>
            {
                var searcher = new GrepSearch(_startDirectory, fileType, _searchFor, _searchSubDirectories);
                searcher.MatchFound += MatchFound;
                searcher.Search();
            });
        }
    }
}