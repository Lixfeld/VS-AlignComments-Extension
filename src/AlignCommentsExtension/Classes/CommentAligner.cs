using System;
using System.Collections.Generic;
using System.Linq;

namespace AlignCommentsExtension.Classes
{
    public class CommentAligner
    {
        private const int DefaultTabSize = 4;
        private const string DoubleSlash = "//";
        private const string WindowsLineEnding = "\r\n";

        public IEnumerable<string> Lines { get; }
        public int TabSize { get; }
        public string LineEnding { get; }

        public CommentAligner(IEnumerable<string> lines, int tabSize = DefaultTabSize, string lineEnding = WindowsLineEnding)
        {
            Lines = lines;
            TabSize = tabSize;
            LineEnding = lineEnding;
        }

        private int GetCommentIndexWithoutTabs()
        {
            var linesWithoutTabs = Lines.Select(x => x.Replace("\t", new string(' ', TabSize)));
            return linesWithoutTabs.Select(x => x.LastIndexOf(DoubleSlash)).Max();
        }

        private IEnumerable<string> GetLines()
        {
            int commentIndex = GetCommentIndexWithoutTabs();

            List<string> newLines = new List<string>();
            foreach (string line in Lines)
            {
                //ToDo(Lixfeld): Improve comment detection
                int index = line.LastIndexOf(DoubleSlash);
                if (index <= -1)
                {
                    // Add unchanged line
                    newLines.Add(line);
                }
                else
                {
                    string comment = line.Substring(index);
                    string subString = line.Substring(0, index).TrimEnd();

                    // Append spaces (considering tab count and size) to align comments
                    int commentTabIndex = commentIndex - (subString.Count(x => x == '\t') * (TabSize - 1));
                    subString += new string(' ', commentTabIndex - subString.Length);

                    string newLine = subString + comment;
                    newLines.Add(newLine);
                }
            }
            return newLines;
        }

        public string GetText()
        {
            string text = string.Join(LineEnding, GetLines()) + LineEnding;
            return text;
        }
    }
}
