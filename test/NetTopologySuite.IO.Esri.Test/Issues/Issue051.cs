using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Features;
using NetTopologySuite.IO.Esri.Dbf;
using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    /// <summary>
    /// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/51
    /// </summary>
    internal class Issue051
    {
        [Test]
        public void Read_When_ColumnLengthMissmatch()
        {
            var shpPath = TestShapefiles.PathTo("#51-columnLengthMissmatch.dbf");
            List<IAttributesTable> attributes = null;
            Assert.DoesNotThrow(() =>
            {
                var dbfReader = new DbfReader(shpPath);
                attributes = dbfReader.ToList();
            });

            Assert.NotNull(attributes);

            //First row is null and empty
            Assert.AreEqual(string.Empty, attributes[0]["RoadLinkId"]);
            Assert.AreEqual(null, attributes[0]["Tunnel"]);
            Assert.AreEqual(null, attributes[0]["Bridge"]);
            Assert.AreEqual(null, attributes[0]["CarAccess"]);
            Assert.AreEqual(null, attributes[0]["Walk"]);
            Assert.AreEqual(null, attributes[0]["MF_4_7"]);
            Assert.AreEqual(null, attributes[0]["COREID"]);

            //Second row has value but length is missmatch in each column
            Assert.AreEqual("osgb4000000080908489", attributes[1]["RoadLinkId"]);
            Assert.AreEqual(true, attributes[1]["Tunnel"]);
            Assert.AreEqual(false, attributes[1]["Bridge"]);
            Assert.AreEqual(true, attributes[1]["CarAccess"]);
            Assert.AreEqual(4.8, attributes[1]["Walk"]);
            Assert.AreEqual(50, attributes[1]["MF_4_7"]);
            Assert.AreEqual(55483, attributes[1]["COREID"]);
        }
    }
}
