using System;
using System.IO;

namespace WPFGrep.Utilities
{
    internal class MatchFoundEventArgs:EventArgs
    {
        public FileInfo File { get; set; }
        public int LineNumber { get; set; }
        public string Line { get; set; }
        public GrepSearchEvent GrepSearchEvent { get; set; }
    }
}