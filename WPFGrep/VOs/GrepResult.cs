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

        public string BeforeLines
        {
            get
            {
                var sb = new StringBuilder();
                var line = Lines.Count / 2;
                for (var i = 0; i < line; i++)
                {
                    sb.Append(Lines[i]).Append(Environment.NewLine);
                }

                return sb.ToString();
            }
        }

        public string AfterLines
        {
            get
            {
                var sb = new StringBuilder();
                var line = Lines.Count / 2 + 1;
                for (var i = line; i < Lines.Count; i++)
                {
                    sb.Append(Lines[i]).Append(Environment.NewLine);
                }

                return sb.ToString();
            }
        }
    }
}