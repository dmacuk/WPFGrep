using System.Collections.Generic;

namespace WPFGrep.VOs
{
    internal class GrepResult
    {
        public int LineNumber { get; set; }
        public IEnumerable<string> Lines { get; set; }
    }
}