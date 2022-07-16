using System;
using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test
{
    /// <summary>
    /// The issue number used in this test (or fixture) refers to an issue on
    /// https://github.com/NetTopologySuite/NetTopologySuite.IO.ShapeFile, created
    /// after this project was split out on its own (and thus, it got its own
    /// set of issue numbers).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ShapeFileIssueNumberAttribute : PropertyAttribute
    {
        public ShapeFileIssueNumberAttribute(int issueNumber)
            : base("NetTopologySuite.IO.ShapeFile issue", issueNumber)
        {
        }
    }
}
