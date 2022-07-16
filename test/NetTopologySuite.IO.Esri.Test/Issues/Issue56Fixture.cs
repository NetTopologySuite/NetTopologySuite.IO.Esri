using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    [TestFixture]
    [ShapeFileIssueNumber(56)]
    public class Issue56Fixture
    {
        /// <summary>
        /// <see href="https://github.com/NetTopologySuite/NetTopologySuite.IO.ShapeFile/issues/56"/>
        /// </summary>
        [Test]
        public void Data_should_be_readable_after_reader_dispose()
        {
            string test56 = Path.Combine(CommonHelpers.TestShapefilesDirectory, "test56.shp");
            var factory = new GeometryFactory();
            int intValue = 56;
            string key = "id";
            var attributes = new AttributesTable();
            attributes.Add(key, intValue);
            var feature = new Feature(factory.CreatePoint(new Coordinate(1, 2)), attributes);

            var writer = new ShapefileDataWriter(test56);
            writer.Header = ShapefileDataWriter.GetHeader(feature, 1);
            writer.Write(new[] { feature });

            using (var reader = new ShapefileDataReader(test56, factory))
            {

                if (reader.RecordCount > 0)
                {
                    while (reader.Read())
                    {
                        int index = reader.GetOrdinal(key);
                        Assert.AreEqual(intValue.GetType(), reader.GetFieldType(index));
                    }
                }
            }

        }
    }
}
