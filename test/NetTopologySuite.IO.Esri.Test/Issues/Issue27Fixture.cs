using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    [TestFixture]
    [ShapeFileIssueNumber(27)]
    public class Issue27Fixture
    {
        /// <summary>
        /// <see href="https://github.com/NetTopologySuite/NetTopologySuite.IO.ShapeFile/issues/27"/>
        /// </summary>
        [Test]
        public void Data_should_be_readable_after_reader_dispose()
        {
            var crustal_test = TestShapefiles.PathTo("crustal_test.shp");
            Assert.True(File.Exists(crustal_test));

            Envelope mbr;
            using (var reader = Shapefile.OpenRead(crustal_test))
            {
                mbr = reader.BoundingBox;
            }

            List<Feature> data = null;
            using (var reader = Shapefile.OpenRead(crustal_test, mbrFilter: mbr))
            {
                data = reader.ToList();
            }
            Assert.IsNotNull(data);
            Assert.IsNotEmpty(data);

            foreach (var item in data)
            {
                Assert.IsNotNull(item.Geometry);
                Assert.IsNotNull(item.Attributes["ID_GTR"]);
            }


            Envelope filterMbr = new Envelope(mbr.MinX, mbr.Centre.X, mbr.MinY, mbr.Centre.Y);
            List<Feature> filteredData = null;
            using (var reader = Shapefile.OpenRead(crustal_test, mbrFilter: filterMbr))
            {
                filteredData = reader.ToList();
            }
            Assert.IsTrue(filteredData.Count < data.Count, "Filtering by MBR failed");
        }
    }
}
