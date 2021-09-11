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
        public string LineEnding { get; }

        public SelectedLines(ITextSnapshot textSnapshot, int startLineNo, int endLineNo)
        {
            StartPosition = textSnapshot.GetLineFromLineNumber(startLineNo).Start.Position;

            List<string> selectedLines = new List<string>();
            for (int i = startLineNo; i <= endLineNo; i++)
            {
                ITextSnapshotLine line = textSnapshot.GetLineFromLineNumber(i);
                selectedLines.Add(line.GetTextIncludingLineBreak());

                // Get line ending/break from first selected line
                if (i == startLineNo)
                    LineEnding = line.GetLineBreakText();
            }

            // Length including line breaks
            Length = selectedLines.Select(x => x.Count()).Sum();
            Lines = selectedLines.Select(x => x.TrimEnd());
        }
    }
}
