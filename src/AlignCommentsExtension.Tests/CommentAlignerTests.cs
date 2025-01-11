using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static AlignCommentsExtension.Classes.Constants;

namespace AlignCommentsExtension.Tests
{
    public class CommentAlignerTests
    {
        [Fact]
        public void SimpleAlignment()
        {
            FileComparer.Verify();
        }

        [Fact]
        public void SimpleAlignmentTabs()
        {
            FileComparer.Verify();
        }

        [Fact]
        public void SimpleAlignmentUnix()
        {
            FileComparer.Verify(lineEnding: UnixLineEnding);
        }

        [Fact]
        public void SimpleAlignmentVisualBasic()
        {
            FileComparer.Verify(delimiter: Apostrophe);
        }
    }
}
