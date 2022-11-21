using System;
using System.IO;
using System.Linq;
using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.ShapeFile.Extended
{
    [TestFixture]
    public class ShapeReaderTests
    {

        [Test]
        public void Ctor_SendNullPath_ShouldThrowException()
        {
            // TODO: Remove no longer relevant test
            //       ShpReader constructors support only a Stream as input data parameter.
            /*
            // Act.
            Assert.Catch<ArgumentNullException>(() =>
            {
                using var shpReader = Shp.OpenRead((string)null);
            });
            */
        }

        [Test]
        public void Ctor_SendEmptyPath_ShouldThrowException()
        {
            // TODO: Remove no longer relevant test
            //       ShpReader constructors support only a Stream as input data parameter.
            /*
            // Act.
            Assert.Catch<ArgumentNullException>(() =>
            {
                new IO.ShapeFile.Extended.ShapeReader(string.Empty);
            });
            */
        }

        [Test]
        public void Ctor_SendWhitespacePath_ShouldThrowException()
        {
            // TODO: Remove no longer relevant test
            //       ShpReader constructors support only a Stream as input data parameter.
            /*
            // Act.
            Assert.Catch<ArgumentNullException>(() =>
            {
                new IO.ShapeFile.Extended.ShapeReader("   \t   ");
            });
            */
        }

        [Test]
        public void Ctor_SendNonExistantFilePath_ShouldThrowException()
        {
            // TODO: Remove no longer relevant test
            //       ShpReader constructors support only a Stream as input data parameter.
            /*
            // Act.
            Assert.Catch<FileNotFoundException>(() =>
            {
                new IO.ShapeFile.Extended.ShapeReader(@"C:\this\is\sheker\path\should\never\exist\on\ur\pc");
            });
            */
        }

        [Test]
        public void Ctor_SendValidParameters_ShouldReturnNotNull()
        {
            // Arrange
            using var tempShp = new TempFileWriter(".shp", "line_ed50_geo");

            // Act.
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            // Assert.
            Assert.IsNotNull(shp);
            Assert.IsTrue(shp.Any());
        }

        [Test]
        public void FileHeader_ReadPoint_ShouldReturnCorrectValues()
        {
            // Arrange.
            var expectedMBR = new Envelope(34.14526022208882, 34.28293070132935, 31.85116738930965, 31.92063218020455);

            using var tempShp = new TempFileWriter(".shp", "point_ed50_geo");

            // Act.
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            // Assert.
            Assert.IsNotNull(shp);
            Assert.AreEqual(shp.ShapeType, ShapeType.Point);
            HelperMethods.AssertEnvelopesEqual(shp.BoundingBox, expectedMBR);
        }

        [Test]
        public void FileHeader_ReadLine_ShouldReturnCorrectValues()
        {
            // Arrange.
            var expectedMBR = new Envelope(639384.5630270261, 662946.9241196744, 3505730.839052265, 3515879.236960234);

            using var tempShp = new TempFileWriter(".shp", "line_ed50_utm36");

            // Act.
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            // Assert.
            Assert.IsNotNull(shp);
            Assert.AreEqual(shp.ShapeType, ShapeType.PolyLine);
            HelperMethods.AssertEnvelopesEqual(shp.BoundingBox, expectedMBR);
        }

        [Test]
        public void FileHeader_ReadPolygon_ShouldReturnCorrectValues()
        {
            // Arrange.
            var expectedMBR = new Envelope(33.47383821246188, 33.75452922072821, 32.0295864794076, 32.1886342399706);

            using var tempShp = new TempFileWriter(".shp", "polygon_wgs84_geo");

            // Act.
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            // Assert.
            Assert.IsNotNull(shp);
            Assert.AreEqual(shp.ShapeType, ShapeType.Polygon);
            HelperMethods.AssertEnvelopesEqual(shp.BoundingBox, expectedMBR);
        }

        [Test]
        public void ReadMBRs_ReadPoint_ShouldReturnCorrectValues()
        {
            // Arrange.

            var expectedInfos = new[]
                {
                    new Envelope(new Coordinate(34.282930701329349, 31.851167389309651)),
                    new Envelope(new Coordinate(34.145260222088822, 31.864369159253059)),
                    new Envelope(new Coordinate(34.181721116813314, 31.920632180204553))
                };

            using var tempShp = new TempFileWriter(".shp", "point_ed50_geo");

            // Act.
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());
            var infos = shp.Select(g => g.EnvelopeInternal).ToArray();

            // Assert.
            Assert.IsNotNull(infos);
            Assert.AreEqual(3, infos.Length);

            int currIndex = 0;

            foreach (var expectedInfo in expectedInfos)
            {
                HelperMethods.AssertEnvelopesEqual(expectedInfo, infos[currIndex++]);
            }
        }

        [Test]
        public void ReadMBRs_ReadUnifiedWithNullAtStart_ShouldReturnCorrectValues()
        {
            var expectedInfos = new[]
                {
                    new Envelope(),
                    new Envelope(-1.151515151515152, -0.353535353535354, -0.929292929292929, -0.419191919191919),
                    new Envelope(-0.457070707070707, 0.421717171717172, 0.070707070707071, 0.578282828282829),
                };

            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterialNullAtStart");

            // Act.
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());
            var infos = shp.Select(g => g.EnvelopeInternal).ToArray();

            // Assert.
            Assert.IsNotNull(infos);
            Assert.AreEqual(expectedInfos.Length, infos.Length);

            int currIndex = 0;

            foreach (var expectedInfo in expectedInfos)
            {
                HelperMethods.AssertEnvelopesEqual(expectedInfo, infos[currIndex++]);
            }
        }

        [Test]
        public void ReadMBRs_ReadUnifiedWithNullInMiddle_ShouldReturnCorrectValues()
        {
            // Arrange.
            var expectedInfos = new[]
                {
                    new Envelope(-1.151515151515152, -0.353535353535354, -0.929292929292929, -0.419191919191919),
                    new Envelope(),
                    new Envelope(-0.457070707070707, 0.421717171717172, 0.070707070707071, 0.578282828282829)
                };

            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterialNullInMiddle");

            // Act.
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());
            var infos = shp.Select(g => g.EnvelopeInternal).ToArray();

            // Assert.
            Assert.IsNotNull(infos);
            Assert.AreEqual(expectedInfos.Length, infos.Length);

            int currIndex = 0;

            foreach (var expectedInfo in expectedInfos)
            {
                HelperMethods.AssertEnvelopesEqual(expectedInfo, infos[currIndex++]);
            }
        }

        [Test]
        public void ReadMBRs_ReadUnifiedWithNullAtEnd_ShouldReturnCorrectValues()
        {
            // Arrange.
            var expectedInfos = new[]
                {
                    new Envelope(-1.151515151515152, -0.353535353535354, -0.929292929292929, -0.419191919191919),
                    new Envelope(-0.457070707070707, 0.421717171717172, 0.070707070707071, 0.578282828282829),
                    new Envelope()
                };

            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterialNullAtEnd");

            // Act.
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());
            var infos = shp.Select(g => g.EnvelopeInternal).ToArray();

            // Assert.
            Assert.IsNotNull(infos);
            Assert.AreEqual(expectedInfos.Length, infos.Length);

            int currIndex = 0;

            foreach (var expectedInfo in expectedInfos)
            {
                HelperMethods.AssertEnvelopesEqual(expectedInfo, infos[currIndex++]);
            }
        }

        [Test]
        public void ReadMBRs_ReadLine_ShouldReturnCorrectValues()
        {
            // Arrange.
            var expectedInfos = new[]
                {
                    new Envelope(34.573027972716453, 34.628034609274806, 31.803273460424684, 31.895998933480186),
                    new Envelope(34.396692412092257, 34.518021336158107, 31.778756216701534, 31.864880893370035),
                };

            using var tempShp = new TempFileWriter(".shp", "line_wgs84_geo");

            // Act.
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());
            var infos = shp.Select(g => g.EnvelopeInternal).ToArray();

            // Assert.
            Assert.IsNotNull(infos);
            Assert.AreEqual(2, infos.Length);

            int currIndex = 0;

            foreach (var expectedInfo in expectedInfos)
            {
                HelperMethods.AssertEnvelopesEqual(expectedInfo, infos[currIndex++]);
            }
        }

        [Test]
        public void ReadMBRs_ReadPolygon_ShouldReturnCorrectValues()
        {
            // Arrange.
            var expectedInfos = new[]
                {
                    new Envelope(33.719047819505683, 33.78096814177016, 31.928805665809271, 32.025301664150398),
                    new Envelope(33.819000337359398, 33.929011051318348, 31.97406740944362, 32.072449163771559)
                };

            using var tempShp = new TempFileWriter(".shp", "polygon_ed50_geo");

            // Act.
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());
            var infos = shp.Select(g => g.EnvelopeInternal).ToArray();

            // Assert.
            Assert.IsNotNull(infos);
            Assert.AreEqual(2, infos.Length);

            int currIndex = 0;

            foreach (var expectedInfo in expectedInfos)
            {
                HelperMethods.AssertEnvelopesEqual(expectedInfo, infos[currIndex++]);
            }
        }

        [Test]
        public void ReadShapeAtOffset_SendNegativeOffset_shouldThrowException()
        {
            // TODO: Remove no longer relevant test
            //       Current implementation supports forward only shape reading.
            /*
            // Arrange.
            GeometryFactory factory = new GeometryFactory();
            m_TmpFile = new TempFileWriter(".shp", ShpFiles.Read("polygon intersecting line"));
            m_Reader = new IO.ShapeFile.Extended.ShapeReader(m_TmpFile.Path);

            // Act.
            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                m_Reader.ReadShapeAtOffset(-1, factory);
            });
            */
        }

        [Test]
        public void ReadShapeAtOffset_SendOffsetAtEndOfFile_shouldThrowException()
        {
            // TODO: Remove no longer relevant test
            //       Current implementation supports forward only shape reading.
            /*
            // Arrange.
            GeometryFactory factory = new GeometryFactory();
            m_TmpFile = new TempFileWriter(".shp", ShpFiles.Read("polygon intersecting line"));
            m_Reader = new IO.ShapeFile.Extended.ShapeReader(m_TmpFile.Path);

            // Act.
            Assert.Catch<IndexOutOfRangeException>(() =>
            {
                m_Reader.ReadShapeAtOffset(ShpFiles.Read("polygon intersecting line").Length, factory);
            });
            */
        }

        [Test]
        public void ReadShapeAtOffset_ReadPolygonWithIntersectingLine_shouldReturnInvalidGeo()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "polygon intersecting line");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            bool[] expectedValidityResults = new bool[] { false, true };

            var firstGeo = shp.First();
            Assert.IsNotNull(firstGeo);
            Assert.AreEqual(firstGeo.IsValid, false);

            Assert.Catch<ArgumentException>(() =>
            {
                var secondGeo = shp.Skip(1).First();
            });
        }

        [Test]
        public void ReadShapeAtOffset_ReadPoint_shouldReturnCorrectValue()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "point_ed50_geo");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            double[,] expectedCoordinates = {{ 34.282930701329349, 31.851167389309651 },
                                             { 34.145260222088822, 31.864369159253059 },
                                             { 34.181721116813314, 31.920632180204553 }};

            // Act.
            int i = 0;
            foreach(var geo in shp)
            {
                // Assert.
                Assert.IsNotNull(geo);
                Assert.IsTrue(geo.IsValid);
                Assert.IsInstanceOf<Point>(geo);
                var givenPoint = geo as Point;

                HelperMethods.AssertDoubleValuesEqual(givenPoint.X, expectedCoordinates[i, 0]);
                HelperMethods.AssertDoubleValuesEqual(givenPoint.Y, expectedCoordinates[i, 1]);
                i++;
            }
        }

        [Test]
        public void ReadShapeAtOffset_ReadLines_shouldReturnCorrectValue()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "line_wgs84_geo");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            var expectedLines = new Coordinate[,]
            {
                {
                    new Coordinate(34.574599590903837, 31.884368958893564),
                    new Coordinate(34.57648553272869, 31.803273460424684),
                    new Coordinate(34.628034609274806, 31.875882220681703),
                    new Coordinate(34.573027972716453, 31.895998933480186),
                    new Coordinate(34.582143358203268, 31.886883547993374)
                },
                {
                    new Coordinate(34.448555812275849, 31.864880893370035),
                    new Coordinate(34.396692412092257, 31.778756216701534),
                    new Coordinate(34.468672525074325, 31.794158074937872),
                    new Coordinate(34.484703030585621, 31.844135533296601),
                    new Coordinate(34.518021336158107, 31.838163384184551)
                }
            };

            // Act.
            int i = 0;
            foreach (var geo in shp)
            {
                // Assert.
                Assert.IsNotNull(geo);
                Assert.IsTrue(geo.IsValid);
                Assert.IsInstanceOf<MultiLineString>(geo);
                var givenLine = geo.IsEmpty ? LineString.Empty : geo.GetGeometryN(0) as LineString;

                for (int j = 0; j < givenLine.Coordinates.Length; j++)
                {
                    var currPoint = givenLine.Coordinates[j];

                    HelperMethods.AssertDoubleValuesEqual(currPoint.X, expectedLines[i, j].X);
                    HelperMethods.AssertDoubleValuesEqual(currPoint.Y, expectedLines[i, j].Y);
                }
                i++;
            }
        }

        [Test]
        public void ReadShapeAtOffset_ReadPolygon_shouldReturnCorrectValue()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "polygon_ed50_geo");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            var expectedLines = new Coordinate[,]
            {
                {
                    new Coordinate(33.719047819505683, 31.989469320254013),
                    new Coordinate(33.730049025918099, 32.025301664150398),
                    new Coordinate(33.771538712027194, 32.008956957757299),
                    new Coordinate(33.78096814177016, 31.993555297099103),
                    new Coordinate(33.744507207486457, 31.928805665809271),
                    new Coordinate(33.719047819505683, 31.989469320254013)
                },
                {
                    new Coordinate(33.821829475819285, 32.051075573685317),
                    new Coordinate(33.860176141775888, 32.072449163771559),
                    new Coordinate(33.927125440097875, 32.054847113210094),
                    new Coordinate(33.929011051318348, 31.97878189417845),
                    new Coordinate(33.819000337359398, 31.97406740944362),
                    new Coordinate(33.821829475819285, 32.051075573685317)
                }
            };

            // Act.
            int i = 0;
            foreach (var geo in shp)
            {
                // Assert.
                Assert.IsNotNull(geo);
                Assert.IsTrue(geo.IsValid);
                Assert.IsInstanceOf<MultiPolygon>(geo);
                var givenPoly = geo.IsEmpty ? Polygon.Empty : geo.GetGeometryN(0) as Polygon;

                Assert.IsNotNull(givenPoly.ExteriorRing);
                Assert.AreSame(givenPoly.ExteriorRing, givenPoly.Shell);
                Assert.AreEqual(givenPoly.Shell.Coordinates.Length, expectedLines.GetLength(1));

                LineString givenLine = givenPoly.Shell;

                for (int j = 0; j < givenLine.Coordinates.Length; j++)
                {
                    var currPoint = givenLine.Coordinates[j];

                    HelperMethods.AssertDoubleValuesEqual(currPoint.X, expectedLines[i, j].X);
                    HelperMethods.AssertDoubleValuesEqual(currPoint.Y, expectedLines[i, j].Y);
                }
                i++;
            }
        }

        [Test]
        public void ReadShapeAtOffset_ReadAllPolygonsFromUnifiedWithNullAtStart_ShouldReturnCorrectValues()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterialNullAtStart");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            var expectedResult = new Coordinate[][]
            {
                Array.Empty<Coordinate>(),
                new Coordinate[]
                {
                    new Coordinate(-0.815656565656566, -0.439393939393939),
                    new Coordinate(-0.353535353535354, -0.795454545454545),
                    new Coordinate(-0.888888888888889,-0.929292929292929),
                    new Coordinate(-1.151515151515152, -0.419191919191919),
                    new Coordinate(-0.815656565656566,-0.439393939393939),
                },
                new Coordinate[]
                {
                    new Coordinate(0.068181818181818,0.578282828282829),
                    new Coordinate(0.421717171717172,0.070707070707071),
                    new Coordinate(-0.457070707070707,0.080808080808081),
                    new Coordinate(0.068181818181818,0.578282828282829),
                }
            };
            long[] offsets = { 112, 248 };

            // Act.
            for (int i = 0; i < offsets.Length; i++)
            {
                shp.Read();
                var geo = shp.Geometry;

                // Assert.
                Assert.IsNotNull(geo);
                Assert.IsTrue(geo.IsValid);
                Assert.IsInstanceOf<MultiPolygon>(geo);
                var givenPoly = geo.IsEmpty ? Polygon.Empty : geo.GetGeometryN(0) as Polygon;

                Assert.IsNotNull(givenPoly.ExteriorRing);
                Assert.AreSame(givenPoly.ExteriorRing, givenPoly.Shell);
                Assert.AreEqual(givenPoly.Shell.Coordinates.Length, expectedResult[i].Length);

                LineString givenLine = givenPoly.Shell;

                for (int j = 0; j < givenLine.Coordinates.Length; j++)
                {
                    var currPoint = givenLine.Coordinates[j];

                    HelperMethods.AssertDoubleValuesEqual(currPoint.X, expectedResult[i][j].X);
                    HelperMethods.AssertDoubleValuesEqual(currPoint.Y, expectedResult[i][j].Y);
                }
            }
        }

        [Test]
        public void ReadShapeAtOffset_ReadAllPolygonsFromUnifiedWithNullInMiddle_ShouldReturnCorrectValues()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterialNullInMiddle");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            var expectedResult = new Coordinate[][]
            {
                new Coordinate[]
                {
                    new Coordinate(-0.815656565656566, -0.439393939393939),
                    new Coordinate(-0.353535353535354, -0.795454545454545),
                    new Coordinate(-0.888888888888889,-0.929292929292929),
                    new Coordinate(-1.151515151515152, -0.419191919191919),
                    new Coordinate(-0.815656565656566,-0.439393939393939),
                },
                Array.Empty<Coordinate>(),
                new Coordinate[]
                {
                    new Coordinate(0.068181818181818,0.578282828282829),
                    new Coordinate(0.421717171717172,0.070707070707071),
                    new Coordinate(-0.457070707070707,0.080808080808081),
                    new Coordinate(0.068181818181818,0.578282828282829),
                }
            };
            long[] offsets = { 100, 248 };

            // Act.
            for (int i = 0; i < offsets.Length; i++)
            {
                shp.Read();
                var geo = shp.Geometry;

                // Assert.
                Assert.IsNotNull(geo);
                Assert.IsTrue(geo.IsValid);
                Assert.IsInstanceOf<MultiPolygon>(geo);
                var givenPoly = geo.IsEmpty ? Polygon.Empty : geo.GetGeometryN(0) as Polygon;

                Assert.IsNotNull(givenPoly.ExteriorRing);
                Assert.AreSame(givenPoly.ExteriorRing, givenPoly.Shell);
                Assert.AreEqual(givenPoly.Shell.Coordinates.Length, expectedResult[i].Length);

                LineString givenLine = givenPoly.Shell;

                for (int j = 0; j < givenLine.Coordinates.Length; j++)
                {
                    var currPoint = givenLine.Coordinates[j];

                    HelperMethods.AssertDoubleValuesEqual(currPoint.X, expectedResult[i][j].X);
                    HelperMethods.AssertDoubleValuesEqual(currPoint.Y, expectedResult[i][j].Y);
                }
            }
        }

        [Test]
        public void ReadShapeAtOffset_ReadAllPolygonsFromUnifiedWithNullAtEnd_ShouldReturnCorrectValues()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterialNullAtEnd");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            var expectedResult = new Coordinate[][]
            {
                new Coordinate[]
                {
                    new Coordinate(-0.815656565656566, -0.439393939393939),
                    new Coordinate(-0.353535353535354, -0.795454545454545),
                    new Coordinate(-0.888888888888889,-0.929292929292929),
                    new Coordinate(-1.151515151515152, -0.419191919191919),
                    new Coordinate(-0.815656565656566,-0.439393939393939),
                },
                new Coordinate[]
                {
                    new Coordinate(0.068181818181818,0.578282828282829),
                    new Coordinate(0.421717171717172,0.070707070707071),
                    new Coordinate(-0.457070707070707,0.080808080808081),
                    new Coordinate(0.068181818181818,0.578282828282829),
                }
            };
            long[] offsets = { 100, 236 };

            // Act.
            for (int i = 0; i < offsets.Length; i++)
            {
                shp.Read();
                var geo = shp.Geometry;

                // Assert.
                Assert.IsNotNull(geo);
                Assert.IsTrue(geo.IsValid);
                Assert.IsInstanceOf<MultiPolygon>(geo);
                var givenPoly = geo.GetGeometryN(0) as Polygon;

                Assert.IsNotNull(givenPoly.ExteriorRing);
                Assert.AreSame(givenPoly.ExteriorRing, givenPoly.Shell);
                Assert.AreEqual(givenPoly.Shell.Coordinates.Length, expectedResult[i].Length);

                LineString givenLine = givenPoly.Shell;

                for (int j = 0; j < givenLine.Coordinates.Length; j++)
                {
                    var currPoint = givenLine.Coordinates[j];

                    HelperMethods.AssertDoubleValuesEqual(currPoint.X, expectedResult[i][j].X);
                    HelperMethods.AssertDoubleValuesEqual(currPoint.Y, expectedResult[i][j].Y);
                }
            }
        }

        [Test]
        public void ReadShapeAtOffset_TryReadAfterDisposed_shouldThrowException()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "line_wgs84_geo");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            shp.Dispose();
            Assert.Catch<InvalidOperationException>(() =>
            {
                shp.First();
            });
        }

        [Test]
        public void ReadAllShapes_SendNullFactory_ShouldThrowException()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            // TODO: Changed original test logic.
            //       Geometry.DefaultFactory is used when provided factory is null.

            // Act.
            var geos = shp.ToList();

            // Assert.
            Assert.IsNotNull(geos);
            Assert.IsTrue(shp.Any());
        }

        [Test]
        public void ReadAllShapes_ReadEmptyShapeFile_ShouldReturnEmptyEnumerable()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "EmptyShapeFile");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            // Act.
            var geos = shp.ToList();

            // Assert.
            Assert.IsNotNull(geos);
            Assert.IsFalse(geos.Any());
        }

        [Test]
        public void ReadAllShapes_ReadPointZM_ShouldReturnCorrectValues()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "shape_PointZM");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());
            double errorMargin = Math.Pow(10, -6);

            double[,] expectedValues = {{-11348202.6085706, 4503476.68482375},
                                        {-601708.888562033, 3537065.37906758},
                                        {-7366588.02885523, -637831.461799072}};

            // Act.
            var shapes = shp.ToList();

            // Assert.
            Assert.IsNotNull(shapes);
            var shapesArr = shapes.ToArray();
            Assert.AreEqual(shapesArr.Length, 3);

            for (int i = 0; i < shapesArr.Length; i++)
            {
                Assert.IsInstanceOf<Point>(shapesArr[i]);
                var currPoint = shapesArr[i] as Point;
                HelperMethods.AssertDoubleValuesEqual(currPoint.X, expectedValues[i, 0], errorMargin);
                HelperMethods.AssertDoubleValuesEqual(currPoint.Y, expectedValues[i, 1], errorMargin);
                HelperMethods.AssertDoubleValuesEqual(currPoint.Z, 0);
                HelperMethods.AssertDoubleValuesEqual(currPoint.M, double.NaN);
            }
        }

        [Test]
        public void ReadAllShapes_ReadPointZMWithMissingMValues_ShouldReturnCorrectValues()
        {
            // TODO: Remove no longer relevant test
            //       This is not a valid SHP file. Record number 4 has ContentLength=-2.
            /*
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "shape_pointZM_MissingM values");
            var shp = Shp.OpenRead(tempShp.OpenRead());
            double errorMargin = Math.Pow(10, -6);

            double[,] expectedValues = {{-11348202.6085706, 4503476.68482375},
                                        {-601708.888562033, 3537065.37906758},
                                        {-7366588.02885523, -637831.461799072}};

            // Act.
            var shapes = shp.ToList();

            // Assert.
            Assert.IsNotNull(shapes);
            var shapesArr = shapes.ToArray();
            Assert.AreEqual(shapesArr.Length, 3);

            for (int i = 0; i < shapesArr.Length; i++)
            {
                Assert.IsInstanceOf<Point>(shapesArr[i]);
                var currPoint = shapesArr[i] as Point;
                HelperMethods.AssertDoubleValuesEqual(currPoint.X, expectedValues[i, 0], errorMargin);
                HelperMethods.AssertDoubleValuesEqual(currPoint.Y, expectedValues[i, 1], errorMargin);
                HelperMethods.AssertDoubleValuesEqual(currPoint.Z, 0);
                HelperMethods.AssertDoubleValuesEqual(currPoint.M, double.NaN);
            }
            */
        }

        [Test]
        public void ReadAllShapes_ReadPointM_ShouldReturnCorrectValues()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "shape_pointM");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            double[,] expectedValues = {{-133.606621226874, 66.8997078870497},
                                        {-68.0564751703992, 56.4888023369036},
                                        {-143.246348588121, 40.6796494644596},
                                        {-82.3232716650438, -21.014605647517}};

            // Act.
            var shapes = shp.ToList();

            // Assert.
            Assert.IsNotNull(shapes);
            var shapesArr = shapes.ToArray();
            Assert.AreEqual(shapesArr.Length, 4);

            for (int i = 0; i < shapesArr.Length; i++)
            {
                Assert.IsInstanceOf<Point>(shapesArr[i]);
                var currPoint = shapesArr[i] as Point;
                HelperMethods.AssertDoubleValuesEqual(currPoint.X, expectedValues[i, 0]);
                HelperMethods.AssertDoubleValuesEqual(currPoint.Y, expectedValues[i, 1]);
                HelperMethods.AssertDoubleValuesEqual(currPoint.Z, double.NaN);
                HelperMethods.AssertDoubleValuesEqual(currPoint.M, double.NaN);
            }
        }

        [Test]
        public void ReadAllShapes_ReadUnifiedChecksMaterial_ShouldRead2ShapesAndCorrectValues()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

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

            // Act.
            var shapes = shp.ToArray();

            Assert.IsNotNull(shapes);
            Assert.AreEqual(shapes.Length, 2);

            for (int i = 0; i < shapes.Length; i++)
            {
                Assert.IsInstanceOf<MultiPolygon>(shapes[i]);
                HelperMethods.AssertPolygonsEqual(shapes[i] as MultiPolygon, expectedResult[i]);
            }
        }

        [Test]
        public void ReadAllShapes_ReadAllPolygonsFromUnifiedWithNullAtStart_ShouldReturnCorrectValues()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterialNullAtStart");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            Polygon[] expectedResult = new Polygon[]
            {
                Polygon.Empty,
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

            // Act.
            var shapes = shp.ToArray();

            Assert.IsNotNull(shapes);
            Assert.AreEqual(shapes.Length, 3);

            for (int i = 0; i < shapes.Length; i++)
            {
                Assert.IsInstanceOf<MultiPolygon>(shapes[i]);
                HelperMethods.AssertPolygonsEqual(shapes[i] as MultiPolygon, expectedResult[i]);
            }
        }

        [Test]
        public void ReadAllShapes_ReadAllPolygonsFromUnifiedWithNullInMiddle_ShouldReturnCorrectValues()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterialNullInMiddle");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

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
                Polygon.Empty,
                new Polygon(new LinearRing(new Coordinate[]
                    {
                        new Coordinate(0.068181818181818,0.578282828282829),
                        new Coordinate(0.421717171717172,0.070707070707071),
                        new Coordinate(-0.457070707070707,0.080808080808081),
                        new Coordinate(0.068181818181818,0.578282828282829),
                    }))
            };

            // Act.
            var shapes = shp.ToArray();

            Assert.IsNotNull(shapes);
            Assert.AreEqual(shapes.Length, 3);

            for (int i = 0; i < shapes.Length; i++)
            {
                Assert.IsInstanceOf<MultiPolygon>(shapes[i]);
                HelperMethods.AssertPolygonsEqual(shapes[i] as MultiPolygon, expectedResult[i]);
            }
        }

        [Test]
        public void ReadAllShapes_ReadAllPolygonsFromUnifiedWithNullAtEnd_ShouldReturnCorrectValues()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterialNullAtEnd");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

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
                    })),
                Polygon.Empty
            };

            // Act.
            var shapes = shp.ToArray();

            Assert.IsNotNull(shapes);
            Assert.AreEqual(shapes.Length, 3);

            for (int i = 0; i < shapes.Length; i++)
            {
                Assert.IsInstanceOf<MultiPolygon>(shapes[i]);
                HelperMethods.AssertPolygonsEqual(shapes[i] as MultiPolygon, expectedResult[i]);
            }
        }

        [Test]
        public void ReadAllShapes_TryReadAfterDisposed_ShouldThrowException()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            // Act.
            shp.Dispose();
            Assert.Catch<InvalidOperationException>(() =>
            {
                shp.ToList();
            });
        }

        [Test]
        public void ReadShapeAtIndex_SendNullFactory_ShouldThrowException()
        {
            // TODO: Remove no longer relevant test
            //       Current implementation supports forward only shape reading.
            //       Geometry.DefaultFactory is used when provided factory is null.
            /*
            // Arrange.
            m_TmpFile = new TempFileWriter(".shp", ShpFiles.Read("UnifiedChecksMaterial"));
            m_Reader = new IO.ShapeFile.Extended.ShapeReader(m_TmpFile.Path);

            // Act.
            Assert.Catch<ArgumentNullException>(() =>
            {
                m_Reader.ReadShapeAtIndex(0, null);
            });
            */
        }

        [Test]
        public void ReadShapeAtIndex_SendNegativeIndex_ShouldThrowException()
        {
            // TODO: Remove no longer relevant test
            //       Current implementation supports forward only shape reading.
            /*
            // Arrange.
            m_TmpFile = new TempFileWriter(".shp", ShpFiles.Read("UnifiedChecksMaterial"));
            m_Reader = new IO.ShapeFile.Extended.ShapeReader(m_TmpFile.Path);
            GeometryFactory factory = new GeometryFactory();

            // Act.
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                m_Reader.ReadShapeAtIndex(-1, factory);
            });
            */
        }

        [Test]
        public void ReadShapeAtIndex_SendOutOfBoundIndex_ShouldThrowException()
        {
            // TODO: Remove no longer relevant test
            //       Current implementation supports forward only shape reading.
            /*
            // Arrange.
            m_TmpFile = new TempFileWriter(".shp", ShpFiles.Read("UnifiedChecksMaterial"));
            m_Reader = new IO.ShapeFile.Extended.ShapeReader(m_TmpFile.Path);
            GeometryFactory factory = new GeometryFactory();

            // Act.
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                m_Reader.ReadShapeAtIndex(2, factory);
            });
            */
        }

        [Test]
        public void ReadShapeAtIndex_ReadFirstUnifiedCheckMaterialShape_ShouldReturnRectangle()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            var expectedPolygon = new Polygon(new LinearRing(new Coordinate[]
                    {
                        new Coordinate(-0.815656565656566, -0.439393939393939),
                        new Coordinate(-0.353535353535354, -0.795454545454545),
                        new Coordinate(-0.888888888888889,-0.929292929292929),
                        new Coordinate(-1.151515151515152, -0.419191919191919),
                        new Coordinate(-0.815656565656566,-0.439393939393939),
                    }));

            // Act.
            var polygon = shp.First();

            Assert.IsNotNull(polygon);
            Assert.IsInstanceOf<MultiPolygon>(polygon);
            HelperMethods.AssertPolygonsEqual(polygon as MultiPolygon, expectedPolygon);
        }

        [Test]
        public void ReadShapeAtIndex_ReadSecondUnifiedCheckMaterialShape_ShouldReturnTriangle()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            var expectedPolygon = new Polygon(new LinearRing(new Coordinate[]
                    {
                        new Coordinate(0.068181818181818,0.578282828282829),
                        new Coordinate(0.421717171717172,0.070707070707071),
                        new Coordinate(-0.457070707070707,0.080808080808081),
                        new Coordinate(0.068181818181818,0.578282828282829),
                    }));

            // Act.
            var polygon = shp.Skip(1).First();

            Assert.IsNotNull(polygon);
            Assert.IsInstanceOf<MultiPolygon>(polygon);
            HelperMethods.AssertPolygonsEqual(polygon as MultiPolygon, expectedPolygon);
        }

        [Test]
        public void ReadShapeAtIndex_ReadUnifiedCheckMaterialWithNullAtStart_ShouldReturnBothShapesCorrectly()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterialNullAtStart");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            Polygon[] expectedResult = new Polygon[]
            {
                Polygon.Empty,
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

            // Act.
            for (int i = 0; i < expectedResult.Length; i++)
            {
                shp.Read();
                var result = shp.Geometry;

                Assert.IsNotNull(result);
                Assert.IsInstanceOf<MultiPolygon>(result);

                HelperMethods.AssertPolygonsEqual(result as MultiPolygon, expectedResult[i]);
            }
        }

        [Test]
        public void ReadShapeAtIndex_ReadUnifiedCheckMaterialWithNullAtEnd_ShouldReturnBothShapesCorrectly()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterialNullAtEnd");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

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

            // Act.
            for (int i = 0; i < expectedResult.Length; i++)
            {
                shp.Read();
                var result = shp.Geometry;

                Assert.IsNotNull(result);
                Assert.IsInstanceOf<MultiPolygon>(result);

                HelperMethods.AssertPolygonsEqual(result as MultiPolygon, expectedResult[i]);
            }
        }

        [Test]
        public void ReadShapeAtIndex_ReadUnifiedCheckMaterialWithNulLInMiddle_ShouldReturnBothShapesCorrectly()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterialNullInMiddle");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

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
                Polygon.Empty,
                new Polygon(new LinearRing(new Coordinate[]
                    {
                        new Coordinate(0.068181818181818,0.578282828282829),
                        new Coordinate(0.421717171717172,0.070707070707071),
                        new Coordinate(-0.457070707070707,0.080808080808081),
                        new Coordinate(0.068181818181818,0.578282828282829),
                    }))
            };

            // Act.
            for (int i = 0; i < expectedResult.Length; i++)
            {
                shp.Read();
                var result = shp.Geometry;

                Assert.IsNotNull(result);
                Assert.IsInstanceOf<MultiPolygon>(result);

                HelperMethods.AssertPolygonsEqual(result as MultiPolygon, expectedResult[i]);
            }
        }
        
        [Test]
        public void ReadShapeAtIndex_TryReadAfterDisposed_ShouldThrowException()
        {
            // Arrange.
            using var tempShp = new TempFileWriter(".shp", "UnifiedChecksMaterial");
            var shp = Shp.Shp.OpenRead(tempShp.OpenRead());

            // Act.
            shp.Dispose();
            Assert.Catch<InvalidOperationException>(() =>
            {
                shp.First();
            });
        }
    }
}
