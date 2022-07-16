using System;
using NUnit.Framework;

namespace NetTopologySuite.IO.ShapeFile.Test
{
    /// <summary>
    /// The issue number used in this test (or fixture) actually refers to an
    /// issue on https://github.com/NetTopologySuite/NetTopologySuite, back
    /// before this project was split out on its own.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class NtsIssueNumberAttribute : PropertyAttribute
    {
        public NtsIssueNumberAttribute(int issueNumber)
            : base("NetTopologySuite issue", issueNumber)
        {
        }
    }
}
