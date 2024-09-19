using NetTopologySuite.IO.Esri.Dbf;
using NUnit.Framework;
using System.Collections.Generic;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    internal class Issue046
    {
        [Test]
        public void TestNoDuplicateColumnName()
        {
            using var dbfReader = new DbfReader(TestShapefiles.PathTo("Issues/046/Antragsschläge 2024_POLYGONE.dbf"));
            var dbfFields = dbfReader.Fields;
            var fieldNames = new HashSet<string>();
            for (int i = 0; i < dbfFields.Count; i++)
            {
                Assert.That(fieldNames.Contains(dbfFields[i].Name), Is.False);
                fieldNames.Add(dbfFields[i].Name);
            }
        }
    }
}
