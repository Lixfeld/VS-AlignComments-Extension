using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;

namespace AlignCommentsExtension.Classes
{
    public class SelectedLines
    {
        public int StartPosition { get; }
        public int Length { get; }
        public IEnumerable<string> Lines { get; }

        public SelectedLines(ITextSnapshot textSnapshot, int startLineNo, int endLineNo)
        {
            StartPosition = textSnapshot.GetLineFromLineNumber(startLineNo).Start.Position;

            List<string> selectedLines = new List<string>();
            for (int i = startLineNo; i <= endLineNo; i++)
            {
                selectedLines.Add(textSnapshot.GetLineFromLineNumber(i).GetTextIncludingLineBreak());
            }

            // Length including line breaks
            Length = selectedLines.Select(x => x.Count()).Sum();
            Lines = selectedLines.Select(x => x.TrimEnd());
        }
    }
}
