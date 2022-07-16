using NUnit.Framework;
using System.IO;
using NetTopologySuite.Geometries;

namespace NetTopologySuite.IO.Esri.Test.Issues
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
            var factory = GeometryFactory.Default;

            string shp_path = Path.Combine(CommonHelpers.TestShapefilesDirectory, "Victoria North.shp");
            Assert.True(File.Exists(shp_path));

            var reader = new ShapefileReader(shp_path, factory);
            var data = reader.ReadAll();
            Assert.IsNotNull(data);
            Assert.IsNotEmpty(data);
            Assert.AreEqual(2, data.Count);

            Assert.IsTrue(data[0].IsEmpty);
            Assert.IsInstanceOf<Polygon>(data[0]);
            Assert.IsTrue(factory.CreatePolygon().EqualsExact(data[0]));

            Assert.IsFalse(data[1].IsEmpty);
            Assert.IsInstanceOf<Polygon>(data[1]);
        }
    }
}
