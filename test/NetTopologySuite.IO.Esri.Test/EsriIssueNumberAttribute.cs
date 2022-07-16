using System;
using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test
{
    /// <summary>
    /// The issue number used in this test (or fixture) refers to an issue on
    /// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri, created
    /// after this project was moved to new repository (and thus, it got its own
    /// set of issue numbers).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class EsriIssueNumberAttribute : PropertyAttribute
    {
        public EsriIssueNumberAttribute(int issueNumber)
            : base("NetTopologySuite.IO.Esri issue", issueNumber)
        {
        }
    }
}
