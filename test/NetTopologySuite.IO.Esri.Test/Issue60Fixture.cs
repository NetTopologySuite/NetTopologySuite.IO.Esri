using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test
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
            string test56 = Path.Combine(CommonHelpers.TestShapefilesDirectory, "test60.shp");
            var factory = new GeometryFactory();
            var attributes = new AttributesTable();
            var feature = new Feature(factory.CreatePoint(new Coordinate(1, 2)), attributes);

            var writer = new ShapefileDataWriter(test56);
            writer.Header = ShapefileDataWriter.GetHeader(feature, 1);
            writer.Write(new[] { feature });

            using (var reader = new ShapefileDataReader(test56, factory)) {

                if (reader.RecordCount > 0)
                {
                    while (reader.Read())
                    {
                        Assert.AreEqual(feature.Geometry.AsText(), reader.Geometry.AsText());
                    }
                }
            }
        }

        [Test]
        public void Header_length_should_always_be_minimal_33()
        {
            var header = new DbaseFileHeader();
            Assert.AreEqual(33, header.HeaderLength);
        }
    }
}
