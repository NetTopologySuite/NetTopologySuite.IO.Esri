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
            string test56 = TestShapefiles.PathTo("test56.shp");
            var factory = new GeometryFactory();
            int intValue = 56;
            string key = "id";
            var attributes = new AttributesTable();
            attributes.Add(key, intValue);
            var feature = new Feature(factory.CreatePoint(new Coordinate(1, 2)), attributes);

            Shapefile.WriteAllFeatures(new[] { feature }, test56);

            using (var reader = Shapefile.OpenRead(test56))
            {

                if (reader.RecordCount > 0)
                {
                    while (reader.Read(out var deleted))
                    {
                        var field = reader.Fields[key];
                        Assert.AreEqual(intValue.GetType(), field.Value.GetType());
                    }
                }
            }

        }
    }
}
