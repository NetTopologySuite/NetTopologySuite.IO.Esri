using NUnit.Framework;
using System.IO;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using System;
using NetTopologySuite.IO.Esri.Dbf;

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
        public void Invalid_data_should_be_skipped()
        {
            var options = new ShapefileReaderOptions()
            {
                Factory = GeometryFactory.Default,
                GeometryBuilderMode = GeometryBuilderMode.SkipInvalidShapes
            };

            string shp_path = TestShapefiles.PathTo("Victoria North.shp");
            Assert.True(File.Exists(shp_path));

            var data = Shapefile.ReadAllGeometries(shp_path, options);  
            Assert.IsNotNull(data);
            Assert.IsNotEmpty(data);
            Assert.AreEqual(1, data.Length);

            Assert.IsTrue(data[0].IsValid);
            Assert.IsFalse(data[0].IsEmpty);
            Assert.IsInstanceOf<MultiPolygon>(data[0]);
        }


        [Test]
        public void Invalid_data_should_be_ignored()
        {
            var options = new ShapefileReaderOptions()
            {
                Factory = GeometryFactory.Default,
                GeometryBuilderMode = GeometryBuilderMode.IgnoreInvalidShapes
            };

            string shp_path = TestShapefiles.PathTo("Victoria North.shp");
            Assert.True(File.Exists(shp_path));

            var data = Shapefile.ReadAllGeometries(shp_path, options);
            Assert.IsNotNull(data);
            Assert.IsNotEmpty(data);
            Assert.AreEqual(2, data.Length);

            Assert.IsFalse(data[0].IsValid);
            Assert.IsInstanceOf<MultiPolygon>(data[0]);

            Assert.IsTrue(data[1].IsValid);
            Assert.IsFalse(data[1].IsEmpty);
            Assert.IsInstanceOf<MultiPolygon>(data[1]);
        }


        [Test]
        public void Invalid_data_should_throw_error()
        {
            var options = new ShapefileReaderOptions()
            {
                Factory = GeometryFactory.Default,
                GeometryBuilderMode = GeometryBuilderMode.Strict
            };

            string shp_path = TestShapefiles.PathTo("Victoria North.shp");
            Assert.True(File.Exists(shp_path));

            Assert.Catch<Exception>(() =>
            {
                var data = Shapefile.ReadAllGeometries(shp_path, options);
            });
        }


        [Test]
        public void Invalid_data_should_be_fixed()
        {
            var options = new ShapefileReaderOptions()
            {
                Factory = GeometryFactory.Default,
                GeometryBuilderMode = GeometryBuilderMode.FixInvalidShapes
            };

            string shp_path = TestShapefiles.PathTo("Victoria North.shp");
            Assert.True(File.Exists(shp_path));

            var data = Shapefile.ReadAllGeometries(shp_path, options);
            Assert.IsNotNull(data);
            Assert.IsNotEmpty(data);
            Assert.AreEqual(2, data.Length);

            Assert.IsTrue(data[0].IsValid);
            Assert.IsInstanceOf<MultiPolygon>(data[0]);

            Assert.IsTrue(data[1].IsValid);
            Assert.IsFalse(data[1].IsEmpty);
            Assert.IsInstanceOf<MultiPolygon>(data[1]);
        }
    }
}
