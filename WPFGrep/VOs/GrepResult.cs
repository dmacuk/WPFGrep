using System;
using System.Collections.Generic;
using System.Text;

namespace WPFGrep.VOs
{
    public class GrepResult
    {
        public int LineNumber { get; set; }
        public List<string> Lines { get; set; }
        public string FileName { get; set; }

        public string FormattedLines
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var line in Lines)
                    sb.Append(line).Append(Environment.NewLine);
                sb.Length = sb.Length - Environment.NewLine.Length;
                return sb.ToString();
            }
        }
    }
}