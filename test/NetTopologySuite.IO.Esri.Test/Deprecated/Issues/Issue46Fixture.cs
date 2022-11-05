using NUnit.Framework;
using System.IO;
using NetTopologySuite.Geometries;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.Issues
{
    /// <summary>
    /// <see href="https://github.com/NetTopologySuite/NetTopologySuite.IO.ShapeFile/issues/46"/>
    /// </summary>
    [TestFixture]
    [ShapeFileIssueNumber(46)]
    public class Issue46Fixture
    {
        [Test]
        public void Invalid_data_should_be_not_read_as_default()
        {
            var options = new ShapefileReaderOptions();
            options.Factory = GeometryFactory.Default;
            options.SkipFailures = true;

            string shp_path = TestShapefiles.PathTo("Victoria North.shp");
            Assert.True(File.Exists(shp_path));

            var data = Shapefile.ReadAllGeometries(shp_path, options);  
            Assert.IsNotNull(data);
            Assert.IsNotEmpty(data);
            Assert.AreEqual(2, data.Length);

            Assert.IsTrue(data[0].IsEmpty);
            Assert.IsInstanceOf<MultiPolygon>(data[0]);
            Assert.IsTrue(options.Factory.CreateMultiPolygon().EqualsExact(data[0]));

            Assert.IsFalse(data[1].IsEmpty);
            Assert.IsInstanceOf<MultiPolygon>(data[1]);
        }
    }
}
