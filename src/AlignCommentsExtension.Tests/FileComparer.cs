using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using AlignCommentsExtension.Classes;
using Xunit;
using static AlignCommentsExtension.Classes.Constants;

namespace AlignCommentsExtension.Tests
{
    public static class FileComparer
    {
        private const string TestFilesDirectory = @"TestFiles\";

        public static void Verify(int tabSize = DefaultTabSize, string lineEnding = WindowsLineEnding, [CallerMemberName] string fileName = "")
        {
            // Arrange
            string testFileName = TestFilesDirectory + fileName + ".test";
            string expectedFileName = TestFilesDirectory + fileName + ".expected";

            string[] lines = File.ReadAllLines(testFileName);
            string expectedText = File.ReadAllText(expectedFileName);

            // Act
            CommentAligner commentAligner = new CommentAligner(lines, tabSize, lineEnding);
            string actualText = commentAligner.GetText();

            // Assert
            Assert.Equal(expectedText, actualText);
        }
    }
}
