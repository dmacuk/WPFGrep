using System;
using System.IO;
using System.Text.RegularExpressions;

namespace WPFGrep.ViewModel.Utilities
{
    public class GrepSearch
    {
        private readonly RegexOptions _regexOptions;

        private readonly string _searchFor;

        private readonly string _searchPattern;

        private readonly bool _searchSubDirectories;

        private readonly DirectoryInfo _startDirectory;

        public GrepSearch(string startDirectory, string searchPattern, string searchFor, bool searchSubDirectories)
        {
            _startDirectory = new DirectoryInfo(startDirectory);
            _searchPattern = searchPattern;
            _searchFor = searchFor;
            _searchSubDirectories = searchSubDirectories;
            _regexOptions = RegexOptions.IgnoreCase;
        }

        public delegate void MatchFoundEventHandler(object sender, MatchFoundEventArgs e);

        public event MatchFoundEventHandler MatchFound;

        public void Search()
        {
            Search(_startDirectory);
        }

        private void OnChanged(MatchFoundEventArgs e)
        {
            MatchFound?.Invoke(this, e);
        }

        private void Search(DirectoryInfo dir)
        {
            if (_searchSubDirectories)
            {
                foreach (var directory in dir.GetDirectories())
                {
                    Search(directory);
                }
            }

            foreach (var file in dir.EnumerateFiles(_searchPattern))
            {
                var reader = file.OpenText();
                string line;
                var lineCount = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    lineCount++;
                    if (Regex.IsMatch(line, _searchFor, _regexOptions))
                    {
                        OnChanged(new MatchFoundEventArgs
                        {
                            File = file,
                            LineNumber = lineCount,
                            Line = line
                        });
                    }
                }
            }
        }
    }

    public class MatchFoundEventArgs : EventArgs
    {
        public FileInfo File { get; set; }
        public string Line { get; set; }
        public int LineNumber { get; set; }
    }
}