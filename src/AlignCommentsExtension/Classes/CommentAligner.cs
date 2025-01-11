using System;
using System.Collections.Generic;
using System.Linq;
using static AlignCommentsExtension.Classes.Constants;

namespace AlignCommentsExtension.Classes
{
    public class CommentAligner
    {
        public int TabSize { get; }
        public string LineEnding { get; }
        public string Delimiter { get; }

        private readonly List<string> lines;
        private readonly List<string> linesWithoutTabs;

        public CommentAligner(IEnumerable<string> lines, int tabSize, string lineEnding, string delimiter = DoubleSlash)
        {
            TabSize = tabSize;
            LineEnding = lineEnding;
            Delimiter = delimiter;

            this.lines = lines.ToList();
            this.linesWithoutTabs = lines.Select(x => x.Replace("\t", new string(' ', TabSize))).ToList();
        }

        public string GetText()
        {
            string text = string.Join(LineEnding, GetLines()) + LineEnding;
            return text;
        }

        private IEnumerable<string> GetLines()
        {
            // Get inline comment index without tabs
            int commentIndex = linesWithoutTabs.Select(x => x.LastIndexOf(Delimiter)).Max();

            List<string> newLines = new List<string>();
            for (int i = 0; i < lines.Count(); i++)
            {
                string line = lines[i];

                //ToDo(Lixfeld): Improve comment detection
                int index = line.LastIndexOf(Delimiter);
                int indexWithoutTabs = linesWithoutTabs[i].LastIndexOf(Delimiter);

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
    }
}
