using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    [TestFixture]
    [ShapeFileIssueNumber(60)]
    public class Issue60Fixture
    {
        /// <summary>
        /// <see href="https://github.com/NetTopologySuite/NetTopologySuite.IO.ShapeFile/issues/60"/>
        /// </summary>
        /// <remarks>without fix results into System.OverflowException</remarks>
        [Test]
        public void Feature_without_fields_should_be_written_correctly()
        {
            string test56 = TestShapefiles.PathTo("test60.shp");
            var factory = new GeometryFactory();
            var attributes = new AttributesTable();
            var feature = new Feature(factory.CreatePoint(new Coordinate(1, 2)), attributes);

            Shapefile.WriteAllFeatures(new[] { feature }, test56);

            using (var reader = Shapefile.OpenRead(test56))
            {

                if (reader.RecordCount > 0)
                {
                    while (reader.Read(out var deleted))
                    {
                        Assert.AreEqual(feature.Geometry.AsText(), reader.Geometry.AsText());
                    }
                }
            }
        }

        [Test]
        public void Header_length_should_always_be_minimal_33()
        {
            //var header = new DbaseFileHeader();
            //Assert.AreEqual(33, header.HeaderLength);
            // TODO: Remove no longer relevant test
        }
    }
}
