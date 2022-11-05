using System;
using System.IO;
using System.Linq;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.Strtree;
using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using NetTopologySuite.IO.Esri;
using NetTopologySuite.IO.Esri.Test;
using NUnit.Framework;
using NetTopologySuite.Features;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.ShapeFile.Extended
{
    [TestFixture]
    public class ShapeDataReaderTests
    {
        [Test]
        public void Ctor_SendNullPath_ShouldThrowException()
        {
            // Act.
            Assert.Catch<ArgumentNullException>(() =>
            {
                using var shapefileReader = Shapefile.OpenRead((string)null);
            });
        }

        [Test]
        public void Ctor_SendEmptyPath_ShouldThrowException()
        {
            // Act.
            Assert.Catch<ArgumentException>(() =>
            {
                using var shapefileReader = Shapefile.OpenRead(string.Empty);
            });
        }

        [Test]
        public void Ctor_SendWhitespacePath_ShouldThrowException()
        {
            // Act.
            Assert.Catch<IOException>(() =>
            {
                using var shapefileReader = Shapefile.OpenRead("   \t   ");
            });
        }

        [Test]
        public void Ctor_SendNonExistantFilePath_ShouldThrowException()
        {
            // Act.
            Assert.Catch<DirectoryNotFoundException>(() =>
            {
                using var shapefileReader = Shapefile.OpenRead(@"C:\this\is\sheker\path\should\never\exist\on\ur\pc");
            });
        }

        [Test]
        public void Ctor_SendShpWithNoDbf_ShouldThrowException()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");

            // Act.
            Assert.Catch<FileNotFoundException>(() =>
            {
                using var shapefileReader = Shapefile.OpenRead(tempShp.Path);
            });
        }

        [Test]
        public void Ctor_SendNullSpatialIndex_ShouldNotThrowException()
        {
            // TODO: Remove no longer relevant test
            //       IDX file is not used during reading at all.
            //       It is stored during writing for compability reasons.
            /*
            // Arrange.
            m_TempFiles = new TempFileWriter[]
            {
                new TempFileWriter(".shp", ShpFiles.Read("UnifiedChecksMaterial")),
                new TempFileWriter(".dbf", DbfFiles.Read("UnifiedChecksMaterial")),
            };

            // Act.
            Assert.Catch<ArgumentNullException>(() =>
            {
                m_shapeDataReader = new ShapeDataReader(m_TempFiles[0].Path, null);
            });
            */
        }

        [Test]
        public void Ctor_SendNullGeometryFactory_ShouldThrowException()
        {
            // TODO: Remove no longer relevant test
            //       Geometry.DefaultFactory is used when provided factory is null.
            /*
            // Arrange.
            m_TempFiles = new TempFileWriter[]
            {
                new TempFileWriter(".shp", ShpFiles.Read("UnifiedChecksMaterial")),
                new TempFileWriter(".dbf", DbfFiles.Read("UnifiedChecksMaterial")),
            };

            // Act.
            Assert.Catch<ArgumentNullException>(() =>
            {
                m_shapeDataReader = new ShapeDataReader(m_TempFiles[0].Path, new STRtree<ShapeLocationInFileInfo>(), null);
            });
            */
        }

        [Test]
        public void Ctor_SendShpWithNoPrj_ShouldReturnNotNull()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            using var tempDbf = new TempFileWriter(".dbf", "UnifiedChecksMaterial");

            // Act.
            using var shapefileReader = Shapefile.OpenRead(tempShp.Path);
            Assert.IsNotNull(shapefileReader);
        }

        [Test]
        public void Ctor_SetAsyncIndexToTrue_ShouldReturnNotNull()
        {
            // TODO: Remove no longer relevant test
            //       STRtree is not supported
            /*
            // Arrange.
            m_TempFiles = new TempFileWriter[]
            {
                new TempFileWriter(".shp", ShpFiles.Read("UnifiedChecksMaterial")),
                new TempFileWriter(".dbf", DbfFiles.Read("UnifiedChecksMaterial"))
            };

            // Act.
            m_shapeDataReader = new ShapeDataReader(m_TempFiles[0].Path, new STRtree<ShapeLocationInFileInfo>(), new GeometryFactory(), true);

            // Assert.
            Assert.IsNotNull(m_shapeDataReader);
            */
        }

        [Test]
        public void Ctor_SetAsyncIndexToFalse_ShouldReturnNotNull()
        {
            // TODO: Remove no longer relevant test
            //       STRtree is not supported
            /*
            // Arrange.
            m_TempFiles = new TempFileWriter[]
            {
                new TempFileWriter(".shp", ShpFiles.Read("UnifiedChecksMaterial")),
                new TempFileWriter(".dbf", DbfFiles.Read("UnifiedChecksMaterial")),
            };

            // Act.
            m_shapeDataReader = new ShapeDataReader(m_TempFiles[0].Path, new STRtree<ShapeLocationInFileInfo>(), new GeometryFactory(), false);

            // Assert.
            Assert.IsNotNull(m_shapeDataReader);
            */
        }

        [Test]
        public void ShapeFileBounds_ReadPointED50Geo_ShouldReturnCorrectEnvelope()
        {
            // Arrange.
            var expectedMBR = new Envelope(34.14526022208882, 34.28293070132935, 31.85116738930965, 31.92063218020455);

            // Act.
            using var shapefileReader = Shapefile.OpenRead(TestShapefiles.PathTo("point_ed50_geo"));

            // Assert.
            HelperMethods.AssertEnvelopesEqual(expectedMBR, shapefileReader.BoundingBox);
        }

        [Test]
        public void ReadByGeoFilter_ReadAllInBounds_ShouldReturnAllShapesAndCorrectDbfData()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            using var tempDbf = new TempFileWriter(".dbf", "UnifiedChecksMaterial");

            Polygon[] expectedResult = new Polygon[]
            {
                new Polygon(new LinearRing(new Coordinate[]
                    {
                        new Coordinate(-0.815656565656566, -0.439393939393939),
                        new Coordinate(-0.353535353535354, -0.795454545454545),
                        new Coordinate(-0.888888888888889,-0.929292929292929),
                        new Coordinate(-1.151515151515152, -0.419191919191919),
                        new Coordinate(-0.815656565656566,-0.439393939393939),
                    })),
                new Polygon(new LinearRing(new Coordinate[]
                    {
                        new Coordinate(0.068181818181818,0.578282828282829),
                        new Coordinate(0.421717171717172,0.070707070707071),
                        new Coordinate(-0.457070707070707,0.080808080808081),
                        new Coordinate(0.068181818181818,0.578282828282829),
                    }))
            };

            string[] expectedShapeMetadata = new string[] { "Rectangle", "Triangle" };

            using var boundingBoxReader = Shapefile.OpenRead(tempShp.Path);
            var options = new ShapefileReaderOptions()
            {
                MbrFilter = boundingBoxReader.BoundingBox
            };

            // Act.
            var results = Shapefile.ReadAllFeatures(tempShp.Path, options); 

            // Assert.
            Assert.IsNotNull(results);

            int currIndex = 0;
            foreach (var result in results)
            {
                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Attributes);

                Polygon resultPolygon = GetPolygon(result.Geometry);
                HelperMethods.AssertPolygonsEqual(resultPolygon, expectedResult[currIndex]);

                object shapeNameData = result.Attributes["ShapeName"];
                Assert.IsInstanceOf<string>(shapeNameData);

                Assert.AreEqual((string)shapeNameData, expectedShapeMetadata[currIndex]);
                currIndex++;
            }
        }

        [Test]
        public void ReadByGeoFilter_ReadWithWholeTriangleInBounds_ShouldReturnTriangle()
        {
            var boundsWithWholeTriangle = new Envelope(-0.62331, 0.63774, -0.02304, 0.76942);

            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            using var tempDbf = new TempFileWriter(".dbf", "UnifiedChecksMaterial");

            var expectedTriangle = new Polygon(new LinearRing(new Coordinate[]
                    {
                        new Coordinate(0.068181818181818,0.578282828282829),
                        new Coordinate(0.421717171717172,0.070707070707071),
                        new Coordinate(-0.457070707070707,0.080808080808081),
                        new Coordinate(0.068181818181818,0.578282828282829),
                    }));

            string expectedShapeMetadata = "Triangle";

            var options = new ShapefileReaderOptions()
            {
                MbrFilter = boundsWithWholeTriangle
            };

            // Act.
            using var shapefileReader = Shapefile.OpenRead(tempShp.Path, options);
            var result = shapefileReader.Single();

            // Assert.
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Attributes);

            Polygon resultPolygon = GetPolygon(result.Geometry);
            HelperMethods.AssertPolygonsEqual(resultPolygon, expectedTriangle);

            object shapeNameData = result.Attributes["ShapeName"];
            Assert.IsInstanceOf<string>(shapeNameData);

            Assert.AreEqual((string)shapeNameData, expectedShapeMetadata);
        }

        [Test]
        public void ReadByGeoFilter_ReadWithWholeRectangleInBounds_ShouldReturnRectangle()
        {
            var boundsWithWholeTriangle = new Envelope(-1.39510, -0.12716, -1.13938, -0.22977);

            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            using var tempDbf = new TempFileWriter(".dbf", "UnifiedChecksMaterial");

            var expectedTriangle = new Polygon(new LinearRing(new Coordinate[]
                    {
                        new Coordinate(-0.815656565656566, -0.439393939393939),
                        new Coordinate(-0.353535353535354, -0.795454545454545),
                        new Coordinate(-0.888888888888889,-0.929292929292929),
                        new Coordinate(-1.151515151515152, -0.419191919191919),
                        new Coordinate(-0.815656565656566,-0.439393939393939),
                    }));

            string expectedShapeMetadata = "Rectangle";

            var options = new ShapefileReaderOptions()
            {
                MbrFilter = boundsWithWholeTriangle
            };

            // Act.
            using var shapefileReader = Shapefile.OpenRead(tempShp.Path, options);
            var result = shapefileReader.Single();

            // Assert.
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Attributes);

            Polygon resultPolygon = GetPolygon(result.Geometry);
            HelperMethods.AssertPolygonsEqual(resultPolygon, expectedTriangle);

            object shapeNameData = result.Attributes["ShapeName"];
            Assert.IsInstanceOf<string>(shapeNameData);

            Assert.AreEqual((string)shapeNameData, expectedShapeMetadata);
        }

        [Test]
        public void ReadByGeoFilter_ReadWithWholeRectangleInBoundsAndFilterByGeometryOption_ShouldReturnRectangle()
        {
            var boundsWithWholeTriangle = new Envelope(-1.39510, -0.12716, -1.13938, -0.22977);

            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            using var tempDbf = new TempFileWriter(".dbf", "UnifiedChecksMaterial");

            var expectedTriangle = new Polygon(new LinearRing(new Coordinate[]
                    {
                        new Coordinate(-0.815656565656566, -0.439393939393939),
                        new Coordinate(-0.353535353535354, -0.795454545454545),
                        new Coordinate(-0.888888888888889,-0.929292929292929),
                        new Coordinate(-1.151515151515152, -0.419191919191919),
                        new Coordinate(-0.815656565656566,-0.439393939393939),
                    }));

            string expectedShapeMetadata = "Rectangle";

            var options = new ShapefileReaderOptions()
            {
                MbrFilter = boundsWithWholeTriangle,
                MbrFilterOption = MbrFilterOption.FilterByGeometry
            };

            // Act.
            using var shapefileReader = Shapefile.OpenRead(tempShp.Path, options);
            var result = shapefileReader.Single();

            // Assert.
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Attributes);

            Polygon resultPolygon = GetPolygon(result.Geometry);
            HelperMethods.AssertPolygonsEqual(resultPolygon, expectedTriangle);

            object shapeNameData = result.Attributes["ShapeName"];
            Assert.IsInstanceOf<string>(shapeNameData);

            Assert.AreEqual((string)shapeNameData, expectedShapeMetadata);
        }

        [Test]
        public void ReadByGeoFilter_ReadWithRectanglePartiallyInBounds_ShouldReturnRectangle()
        {
            var boundsWithWholeTriangle = new Envelope(-0.93340, -0.38902, -0.73281, -0.29179);

            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            using var tempDbf = new TempFileWriter(".dbf", "UnifiedChecksMaterial");

            var expectedTriangle = new Polygon(new LinearRing(new Coordinate[]
                    {
                        new Coordinate(-0.815656565656566, -0.439393939393939),
                        new Coordinate(-0.353535353535354, -0.795454545454545),
                        new Coordinate(-0.888888888888889,-0.929292929292929),
                        new Coordinate(-1.151515151515152, -0.419191919191919),
                        new Coordinate(-0.815656565656566,-0.439393939393939),
                    }));

            string expectedShapeMetadata = "Rectangle";

            var options = new ShapefileReaderOptions()
            {
                MbrFilter = boundsWithWholeTriangle
            };

            // Act.
            using var shapefileReader = Shapefile.OpenRead(tempShp.Path, options);
            var result = shapefileReader.Single();

            // Assert.
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Attributes);

            Polygon resultPolygon = GetPolygon(result.Geometry);
            HelperMethods.AssertPolygonsEqual(resultPolygon, expectedTriangle);

            object shapeNameData = result.Attributes["ShapeName"];
            Assert.IsInstanceOf<string>(shapeNameData);

            Assert.AreEqual((string)shapeNameData, expectedShapeMetadata);
        }

        [Test]
        public void ReadByGeoFilter_ReadWithRectanglePartiallyInBoundsAndFilterByGeometryOption_ShouldReturnRectangle()
        {
            var boundsWithWholeTriangle = new Envelope(-0.93340, -0.38902, -0.73281, -0.29179);

            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            using var tempDbf = new TempFileWriter(".dbf", "UnifiedChecksMaterial");

            var expectedTriangle = new Polygon(new LinearRing(new Coordinate[]
                    {
                        new Coordinate(-0.815656565656566, -0.439393939393939),
                        new Coordinate(-0.353535353535354, -0.795454545454545),
                        new Coordinate(-0.888888888888889,-0.929292929292929),
                        new Coordinate(-1.151515151515152, -0.419191919191919),
                        new Coordinate(-0.815656565656566,-0.439393939393939),
                    }));

            string expectedShapeMetadata = "Rectangle";

            var options = new ShapefileReaderOptions()
            {
                MbrFilter = boundsWithWholeTriangle,
                MbrFilterOption = MbrFilterOption.FilterByGeometry
            };

            // Act.
            using var shapefileReader = Shapefile.OpenRead(tempShp.Path, options);
            var result = shapefileReader.Single();

            // Assert.
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Attributes);

            Polygon resultPolygon = GetPolygon(result.Geometry);
            HelperMethods.AssertPolygonsEqual(resultPolygon, expectedTriangle);

            object shapeNameData = result.Attributes["ShapeName"];
            Assert.IsInstanceOf<string>(shapeNameData);

            Assert.AreEqual((string)shapeNameData, expectedShapeMetadata);
        }

        [Test]
        public void ReadByGeoFilter_ReadWithRectangleMBRPartiallyInBoundsAndFilterByGeometryOption_ShouldReturnNoGeometries()
        {
            var boundsWithWholeTriangle = new Envelope(-1.17459, -1.00231, -1.09803, -0.80861);

            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            using var tempDbf = new TempFileWriter(".dbf", "UnifiedChecksMaterial");

            var options = new ShapefileReaderOptions()
            {
                MbrFilter = boundsWithWholeTriangle,
                MbrFilterOption = MbrFilterOption.FilterByGeometry
            };

            // Act.
            var results = Shapefile.ReadAllFeatures(tempShp.Path, options);

            // Assert.
            Assert.IsNotNull(results);
            Assert.IsFalse(results.Any());
        }

        // I give it as a parameter a rectangle that partially intersects only with the MBR of the
        // shape, and doesn't intersect with the shape itself at all.
        [Test]
        public void ReadByGeoFilter_ReadWithRectangleMBRPartiallyInBounds_ShouldReturnRectangle()
        {
            var boundsWithWholeTriangle = new Envelope(-1.17459, -1.00231, -1.09803, -0.80861);

            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            using var tempDbf = new TempFileWriter(".dbf", "UnifiedChecksMaterial");

            var expectedTriangle = new Polygon(new LinearRing(new Coordinate[]
                    {
                        new Coordinate(-0.815656565656566, -0.439393939393939),
                        new Coordinate(-0.353535353535354, -0.795454545454545),
                        new Coordinate(-0.888888888888889,-0.929292929292929),
                        new Coordinate(-1.151515151515152, -0.419191919191919),
                        new Coordinate(-0.815656565656566,-0.439393939393939),
                    }));

            string expectedShapeMetadata = "Rectangle";

            var options = new ShapefileReaderOptions()
            {
                MbrFilter = boundsWithWholeTriangle,
                MbrFilterOption = MbrFilterOption.FilterByExtent
            };

            // Act.
            using var shapefileReader = Shapefile.OpenRead(tempShp.Path, options);
            var result = shapefileReader.Single();

            // Assert.
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Attributes);

            Polygon resultPolygon = GetPolygon(result.Geometry);
            HelperMethods.AssertPolygonsEqual(resultPolygon, expectedTriangle);

            object shapeNameData = result.Attributes["ShapeName"];
            Assert.IsInstanceOf<string>(shapeNameData);

            Assert.AreEqual((string)shapeNameData, expectedShapeMetadata);
        }

        [Test]
        public void ReadByGeoFilter_ReadWithNoShapeInBounds_ShouldReturnEmptyEnumerable()
        {
            var boundsWithWholeTriangle = new Envelope(-1.17459, -1.00231, -1.09803, -1.5);

            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            using var tempDbf = new TempFileWriter(".dbf", "UnifiedChecksMaterial");

            var options = new ShapefileReaderOptions()
            {
                MbrFilter = boundsWithWholeTriangle
            };

            // Act.
            var results = Shapefile.ReadAllFeatures(tempShp.Path, options);

            // Assert.
            Assert.IsNotNull(results);
            Assert.IsFalse(results.Any());
        }

        [Test]
        public void ReadByGeoFilter_ReadWithNoShapeInBoundsAndFilterByGeometryOption_ShouldReturnEmptyEnumerable()
        {
            var boundsWithWholeTriangle = new Envelope(-1.17459, -1.00231, -1.09803, -1.5);

            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            using var tempDbf = new TempFileWriter(".dbf", "UnifiedChecksMaterial");

            var options = new ShapefileReaderOptions()
            {
                MbrFilter = boundsWithWholeTriangle,
                MbrFilterOption = MbrFilterOption.FilterByGeometry
            };

            // Act.
            var results = Shapefile.ReadAllFeatures(tempShp.Path, options);

            // Assert.
            Assert.IsNotNull(results);
            Assert.IsFalse(results.Any());
        }

        [Test, ShapeFileIssueNumber(27)]
        public void ReadByGeoFilter_ReadDbfDataAfterReaderObjectDisposed_ShouldNotThrowException()
        {
            var boundsWithWholeTriangle = new Envelope(-1.17459, -1.00231, -1.09803, -0.80861);

            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            using var tempDbf = new TempFileWriter(".dbf", "UnifiedChecksMaterial");

            var options = new ShapefileReaderOptions()
            {
                MbrFilter = boundsWithWholeTriangle
            };

            // Act.
            using var shapefileReader = Shapefile.OpenRead(tempShp.Path, options);
            var result = shapefileReader.Single();

            // Dispose of the reader object.
            shapefileReader.Dispose();

            // Assert.
            Assert.IsNotNull(result.Attributes);
        }

        [Test, ShapeFileIssueNumber(27)]
        public void ReadByGeoFilter_ReadShapeDataAfterReaderObjectDisposed_ShouldNotThrowException()
        {
            var boundsWithWholeTriangle = new Envelope(-1.17459, -1.00231, -1.09803, -0.80861);

            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            using var tempDbf = new TempFileWriter(".dbf", "UnifiedChecksMaterial");

            var options = new ShapefileReaderOptions()
            {
                MbrFilter = boundsWithWholeTriangle
            };

            // Act.
            using var shapefileReader = Shapefile.OpenRead(tempShp.Path, options);
            var result = shapefileReader.Single();
            shapefileReader.Dispose();

            // Assert.
            Assert.IsNotNull(result.Geometry);
        }

        private Polygon GetPolygon(Geometry geometry)
        {
            var multiPolygon = geometry as MultiPolygon;
            Assert.IsNotNull(multiPolygon);
            Assert.AreEqual(geometry.NumGeometries, 1);

            var polygon = multiPolygon.GetGeometryN(0) as Polygon;
            Assert.IsNotNull(polygon);
            return polygon;
        }
    }
}
