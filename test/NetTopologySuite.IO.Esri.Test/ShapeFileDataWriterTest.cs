﻿using NetTopologySuite.Geometries;
using NetTopologySuite.Algorithm.Match;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries.Implementation;
using NetTopologySuite.IO.Streams;
using NUnit.Framework;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace NetTopologySuite.IO.ShapeFile.Test
{

    [TestFixture]
    public class ShapeFileDataWriterTest
    {
        protected GeometryFactory Factory { get; private set; }

        protected WKTReader Reader { get; private set; }

        public ShapeFileDataWriterTest()
        {
            // Set current dir to shapefiles dir
            Environment.CurrentDirectory = CommonHelpers.TestShapefilesDirectory;

            this.Factory = new GeometryFactory();
            this.Reader = new WKTReader();
        }

        [Test]
        public void TestCreateEmptyShapefile()
        {
            const string filename = "__empty";
            const string emptyShp = filename + ".shp";
            const string emptyShx = filename + ".shx";
            const string emptyDbf = filename + ".dbf";
            if (File.Exists(emptyShp))
                File.Delete(emptyShp);
            if (File.Exists(emptyShx))
                File.Delete(emptyShx);
            if (File.Exists(emptyDbf))
                File.Delete(emptyDbf);

            var writer = new ShapefileDataWriter(filename, Factory);
            writer.Header = new DbaseFileHeader();
            writer.Write(new IFeature[0]);

            Assert.That(File.Exists(emptyShp), Is.True);
            Assert.That(File.Exists(emptyShx), Is.True);
            Assert.That(File.Exists(emptyDbf), Is.True);
        }

        [Test]
        public void TestWriteZValuesShapeFile()
        {
            TestWriteZMValuesShapeFile(false);
        }

        [Test]
        public void TestWriteZMValuesShapeFile()
        {
            TestWriteZMValuesShapeFile(true);
        }

        private void TestWriteZMValuesShapeFile(bool testM)
        {
            var points = new Coordinate[3];
            points[0] = new Coordinate(0, 0);
            points[1] = new Coordinate(1, 0);
            points[2] = new Coordinate(1, 1);

            var csFactory = DotSpatialAffineCoordinateSequenceFactory.Instance;
            var sequence = csFactory.Create(3, Ordinates.XYZM);
            for (int i = 0; i < 3; i++)
            {
                sequence.SetOrdinate(i, Ordinate.X, points[i].X);
                sequence.SetOrdinate(i, Ordinate.Y, points[i].Y);
                sequence.SetOrdinate(i, Ordinate.Z, 1 + i);
                if (testM)
                    sequence.SetOrdinate(i, Ordinate.M, 11 + i);
            }
            var lineString = Factory.CreateLineString(sequence);

            var attributes = new AttributesTable();
            attributes.Add("FOO", "Trond");

            var feature = new Feature(Factory.CreateMultiLineString(new[] { lineString }), attributes);
            var features = new Feature[1];
            features[0] = feature;

            var shpWriter = new ShapefileDataWriter("ZMtest", Factory)
            {
                Header = ShapefileDataWriter.GetHeader(features[0], features.Length)
            };
            shpWriter.Write(features);

            // Now let's read the file and verify that we got Z and M back
            var factory = new GeometryFactory(DotSpatialAffineCoordinateSequenceFactory.Instance);

            using (var reader = new ShapefileDataReader("ZMtest", factory))
            {
                reader.Read();
                var geom = reader.Geometry;

                for (int i = 0; i < 3; i++)
                {
                    var c = geom.Coordinates[i];
                    Assert.AreEqual(i + 1, c.Z);
                }

                if (testM)
                {
                    sequence = ((LineString)geom).CoordinateSequence;
                    for (int i = 0; i < 3; i++)
                    {
                        Assert.AreEqual(sequence.GetOrdinate(i, Ordinate.M), 11 + i);
                    }
                }

                // Run a simple attribute test too
                string v = reader.GetString(1);
                Assert.AreEqual(v, "Trond");
            }
        }

        [Test]
        public void TestWriteSimpleShapeFile()
        {
            var p1 = Factory.CreatePoint(new Coordinate(100, 100));
            var p2 = Factory.CreatePoint(new Coordinate(200, 200));

            var coll = new GeometryCollection(new Geometry[] { p1, p2, });
            ShapefileWriter.WriteGeometryCollection(@"test_arcview", coll);

            // Not read by ArcView!!!
        }

        [Test]
        public void TestReadWritePoint()
        {
            var geomsWrite = ShapeFileShapeFactory.CreateShapes(OgcGeometryType.Point, Ordinates.XY);
            DoTest(geomsWrite, Ordinates.XY);
            geomsWrite = ShapeFileShapeFactory.CreateShapes(OgcGeometryType.Point, Ordinates.XYM);
            DoTest(geomsWrite, Ordinates.XYM);
            geomsWrite = ShapeFileShapeFactory.CreateShapes(OgcGeometryType.Point, Ordinates.XYZM);
            DoTest(geomsWrite, Ordinates.XYZM);
        }

        [Test]
        public void TestReadWriteMultiPoint()
        {
            var geomsWrite = ShapeFileShapeFactory.CreateShapes(OgcGeometryType.MultiPoint, Ordinates.XY);
            DoTest(geomsWrite, Ordinates.XY);
            geomsWrite = ShapeFileShapeFactory.CreateShapes(OgcGeometryType.MultiPoint, Ordinates.XYM);
            DoTest(geomsWrite, Ordinates.XYM);
            geomsWrite = ShapeFileShapeFactory.CreateShapes(OgcGeometryType.MultiPoint, Ordinates.XYZM);
            DoTest(geomsWrite, Ordinates.XYZM);
        }

        [Test]
        public void TestReadWriteLineal()
        {
            var geomsWrite = ShapeFileShapeFactory.CreateShapes(OgcGeometryType.LineString, Ordinates.XY);
            DoTest(geomsWrite, Ordinates.XY);
            geomsWrite = ShapeFileShapeFactory.CreateShapes(OgcGeometryType.LineString, Ordinates.XYM);
            DoTest(geomsWrite, Ordinates.XYM);
            geomsWrite = ShapeFileShapeFactory.CreateShapes(OgcGeometryType.LineString, Ordinates.XYZM);
            DoTest(geomsWrite, Ordinates.XYZM);
        }

        [Test]
        public void TestReadWritePolygonal()
        {
            var geomsWrite = ShapeFileShapeFactory.CreateShapes(OgcGeometryType.Polygon, Ordinates.XY);
            DoTest(geomsWrite, Ordinates.XY);
            geomsWrite = ShapeFileShapeFactory.CreateShapes(OgcGeometryType.Polygon, Ordinates.XYM);
            DoTest(geomsWrite, Ordinates.XYM);
            geomsWrite = ShapeFileShapeFactory.CreateShapes(OgcGeometryType.Polygon, Ordinates.XYZM);
            DoTest(geomsWrite, Ordinates.XYZM, false);
        }

        private static void DoTest(GeometryCollection geomsWrite, Ordinates ordinates, bool testGetOrdinate = true)
        {
            string fileName = string.Empty;

            try
            {

                fileName = Path.GetTempFileName();
                fileName = Path.ChangeExtension(fileName, "shp");
                ShapefileWriter.WriteGeometryCollection(fileName, geomsWrite);
                var reader = new ShapefileReader(fileName, ShapeFileShapeFactory.FactoryRead);
                var geomsRead = reader.ReadAll();

                // This tests x- and y- values
                if (!geomsWrite.EqualsExact(geomsRead))
                {
                    Assert.AreEqual(geomsWrite.NumGeometries, geomsRead.NumGeometries);
                    //
                    // This akward test is necessary since EqualsTopologically throws currently exceptions
                    bool equal = true;
                    for (int i = 0; i < geomsRead.NumGeometries; i++)
                    {
                        var gw = geomsWrite.GetGeometryN(i);
                        var gr = geomsRead.GetGeometryN(i);
                        if (gw.IsEmpty && gr.IsEmpty)
                        {
                            if ((gw is ILineal && gr is ILineal) ||
                                (gw is IPolygonal && gr is IPolygonal))
                            {
                                // suppose these are equal
                            }
                            else
                            {
                                Console.WriteLine(string.Format("Geometries don't match at index {0}", i));
                                Console.WriteLine(string.Format("  written: {0}", gw.AsText()));
                                Console.WriteLine(string.Format("  read   : {0}", gr.AsText()));
                                equal = false;
                                Assert.IsTrue(equal, "Differenced found in geometries written and read!");
                            }
                        }
                        else if (!gw.EqualsExact(gr))
                        {
                            double hsm = new HausdorffSimilarityMeasure().Measure(gw, gr);
                            double asm = new AreaSimilarityMeasure().Measure(gw, gr);
                            double smc = SimilarityMeasureCombiner.Combine(hsm, asm);
                            if (!gw.EqualsNormalized(gr) || (1d - smc) > 1e-7)
                            {
                                Console.WriteLine(string.Format("Geometries don't match at index {0}", i));
                                Console.WriteLine(string.Format("  written: {0}", gw.AsText()));
                                Console.WriteLine(string.Format("  read   : {0}", gr.AsText()));
                                equal = false;
                                Assert.IsTrue(equal, "Differenced found in geometries written and read!");
                            }
                        }
                    }

                    //For polygons this has a tendency to fail, since the polygonhandler might rearrange the whole thing
                    if (testGetOrdinate)
                    {
                        if ((ordinates & Ordinates.Z) == Ordinates.Z)
                        {
                            double[] writeZ = geomsWrite.GetOrdinates(Ordinate.Z);
                            double[] readZ = geomsRead.GetOrdinates(Ordinate.Z);
                            Assert.IsTrue(ArraysEqual(writeZ, readZ));
                        }

                        if ((ordinates & Ordinates.M) == Ordinates.M)
                        {
                            double[] writeM = geomsWrite.GetOrdinates(Ordinate.M);
                            double[] readM = geomsRead.GetOrdinates(Ordinate.M);
                            Assert.IsTrue(ArraysEqual(writeM, readM));
                        }
                    }

                }

                // delete sample files
                File.Delete(fileName);
                File.Delete(Path.ChangeExtension(fileName, "shx"));
                File.Delete(Path.ChangeExtension(fileName, "dbf"));
            }
            catch (AssertionException ex)
            {
                Console.WriteLine("Failed test with {0}", ordinates);
                Console.WriteLine(ex.Message);
                Console.WriteLine("  Testfile '{0}' not deleted!", fileName);
                throw;
            }

        }

        private static bool ArraysEqual(double[] writeZ, double[] readZ)
        {
            if (writeZ == null ^ readZ == null)
                return false;

            if (writeZ == null)
                return true;

            if (writeZ.Length != readZ.Length)
                return false;

            for (int i = 0; i < writeZ.Length; i++)
                if (Math.Abs(writeZ[i] - readZ[i]) > 1E-7) return false;

            return true;
        }

        private static class ShapeFileShapeFactory
        {
            public static GeometryCollection CreateShapes(OgcGeometryType type, Ordinates ordinates, int number = 50)
            {
                bool[] empty = new bool[number];
                empty[Rnd.Next(2, number / 2)] = true;
                empty[Rnd.Next(number / 2, number)] = true;

                var result = new Geometry[number];
                for (int i = 0; i < number; i++)
                {
                    switch (type)
                    {
                        case OgcGeometryType.Point:
                            result[i] = CreatePoint(ordinates, empty[i]);
                            break;
                        case OgcGeometryType.MultiPoint:
                            result[i] = CreateMultiPoint(ordinates, empty[i]);
                            break;

                        case OgcGeometryType.LineString:
                        case OgcGeometryType.MultiLineString:
                            result[i] = CreateLineal(ordinates, empty[i]);
                            break;
                        case OgcGeometryType.Polygon:
                        case OgcGeometryType.MultiPolygon:
                            result[i] = CreatePolygonal(ordinates, empty[i]);
                            break;
                    }

                    /*
                    // Ensure no empty elements
                    if (result[i] == null || (result[i].IsEmpty && result[i].OgcGeometryType == OgcGeometryType.GeometryCollection))
                        i--;
                    */
                    // Ensure not null and not geometry collection
                    if (result[i] == null || result[i].OgcGeometryType == OgcGeometryType.GeometryCollection)
                        i--;
                }

                return Factory.CreateGeometryCollection(result);
            }

            private static readonly Random Rnd = new Random(9936528);

            private static readonly CoordinateSequenceFactory CsFactory =
                DotSpatialAffineCoordinateSequenceFactory.Instance;

            public static readonly GeometryFactory FactoryRead = new GeometryFactory(new PrecisionModel(PrecisionModels.Floating), 4326, CsFactory);

            public static readonly GeometryFactory Factory = new GeometryFactory(new PrecisionModel(1000), 4326, CsFactory);

            private static Geometry CreatePoint(Ordinates ordinates, bool empty)
            {
                if (empty)
                {
                    return Factory.CreatePoint((CoordinateSequence)null);
                }

                var seq = CsFactory.Create(1, ordinates);
                foreach (var o in ToOrdinateArray(ordinates))
                    seq.SetOrdinate(0, o, RandomOrdinate(o, Factory.PrecisionModel));
                return Factory.CreatePoint(seq);
            }

            private static Geometry CreateMultiPoint(Ordinates ordinates, bool empty)
            {
                if (empty)
                {
                    return Factory.CreateMultiPoint((CoordinateSequence)null);
                }
                int numPoints = Rnd.Next(75, 101);
                var seq = CsFactory.Create(numPoints, ordinates);
                for (int i = 0; i < numPoints; i++)
                    foreach (var o in ToOrdinateArray(ordinates))
                        seq.SetOrdinate(i, o, RandomOrdinate(o, Factory.PrecisionModel));

                return Factory.CreateMultiPoint(seq);
            }

            private static Geometry CreateLineal(Ordinates ordinates, bool empty)
            {
                switch (Rnd.Next(2))
                {
                    case 0:
                        return CreateLineString(ordinates, empty);
                    default:
                        return CreateMultiLineString(ordinates, empty);
                }
            }

            private static Geometry CreateLineString(Ordinates ordinates, bool empty)
            {
                if (empty)
                {
                    return Factory.CreateLineString((CoordinateSequence)null);
                }

                int numPoints = Rnd.Next(75, 101);
                var seq = CsFactory.Create(numPoints, ordinates);
                for (int i = 0; i < numPoints; i++)
                    foreach (var o in ToOrdinateArray(ordinates))
                        seq.SetOrdinate(i, o, RandomOrdinate(o, Factory.PrecisionModel));

                return Factory.CreateLineString(seq);
            }

            private static Geometry CreateMultiLineString(Ordinates ordinates, bool empty)
            {
                if (empty)
                {
                    Factory.CreateMultiLineString(null);
                }

                int numLineStrings = Rnd.Next(0, 11);
                if (numLineStrings <= 2)
                    numLineStrings = 0;

                var lineString = new LineString[numLineStrings];
                for (int i = 0; i < numLineStrings; i++)
                    lineString[i] = (LineString)CreateLineString(ordinates, false);

                return Factory.CreateMultiLineString(lineString);
            }

            private static Geometry CreatePolygonal(Ordinates ordinates, bool empty)
            {
                if (Rnd.Next(2) == 0)
                    return CreatePolygon(ordinates, empty);
                return CreateMultiPolygon(ordinates, empty);
            }

            private static Geometry CreatePolygon(Ordinates ordinates, bool empty, int nextKind = -1)
            {
                if (empty)
                {
                    Factory.CreatePolygon((CoordinateSequence)null);
                }
                if (nextKind == -1) nextKind = Rnd.Next(0, 5);

                double x = RandomOrdinate(Ordinate.X, Factory.PrecisionModel);
                double y = RandomOrdinate(Ordinate.Y, Factory.PrecisionModel);

                switch (nextKind)
                {
                    case 0: // circle
                        var ring = CreateCircleRing(ordinates, x, y, 3 * Rnd.NextDouble());
                        return Factory.CreatePolygon(ring, null);
                    case 1: // rectangle
                        ring = CreateRectangleRing(ordinates, x, y, 6 * Rnd.NextDouble(), 3 * Rnd.NextDouble());
                        return Factory.CreatePolygon(ring, null);
                    case 2: // cirle with hole
                        double radius = 3 * Rnd.NextDouble();
                        var shell = CreateCircleRing(ordinates, x, y, radius);
                        var hole = CreateCircleRing(ordinates, x, y, 0.66 * radius, true);
                        return Factory.CreatePolygon(shell, new[] { hole });
                    case 3: // rectanglee with hole
                        double width = 6 * Rnd.NextDouble();
                        double height = 3 * Rnd.NextDouble();
                        shell = CreateRectangleRing(ordinates, x, y, width, height);
                        hole = CreateRectangleRing(ordinates, x, y, 0.66 * width, 0.66 * height, true);
                        return Factory.CreatePolygon(shell, new[] { hole });
                    case 4: // rectanglee with hole
                        width = 6 * Rnd.NextDouble();
                        height = 3 * Rnd.NextDouble();
                        shell = CreateRectangleRing(ordinates, x, y, width, height);
                        hole = CreateCircleRing(ordinates, x, y, 0.33 * Math.Min(width, height), true);
                        return Factory.CreatePolygon(shell, new[] { hole });
                    default:
                        throw new NotSupportedException();
                }
            }

            private static LinearRing CreateCircleRing(Ordinates ordinates, double x, double y, double radius, bool reverse = false)
            {
                var seq = CsFactory.Create(4 * 12 + 1, ordinates);
                double angle = Math.PI * 2;
                const double quandrantStep = Math.PI / 2d / 12d;
                int k = 0;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        double dx = radius * Math.Cos(angle);
                        double dy = radius * Math.Sin(angle);
                        seq.SetOrdinate(k, Ordinate.X, Factory.PrecisionModel.MakePrecise(x + dx));
                        seq.SetOrdinate(k, Ordinate.Y, Factory.PrecisionModel.MakePrecise(y + dy));
                        if ((ordinates & Ordinates.Z) == Ordinates.Z)
                            seq.SetOrdinate(k, Ordinate.Z, RandomOrdinate(Ordinate.Z, Factory.PrecisionModel));
                        if ((ordinates & Ordinates.Z) == Ordinates.Z)
                            seq.SetOrdinate(k, Ordinate.M, RandomOrdinate(Ordinate.M, Factory.PrecisionModel));
                        k++;
                        angle -= quandrantStep;
                    }
                }
                seq.SetOrdinate(k, Ordinate.X, seq.GetOrdinate(0, Ordinate.X));
                seq.SetOrdinate(k, Ordinate.Y, seq.GetOrdinate(0, Ordinate.Y));
                if ((ordinates & Ordinates.Z) == Ordinates.Z)
                    seq.SetOrdinate(k, Ordinate.Z, seq.GetOrdinate(0, Ordinate.Z));
                if ((ordinates & Ordinates.M) == Ordinates.M)
                    seq.SetOrdinate(k, Ordinate.M, seq.GetOrdinate(0, Ordinate.M));

                return Factory.CreateLinearRing(reverse ? seq.Reversed() : seq);
            }

            private static LinearRing CreateRectangleRing(Ordinates ordinates, double x, double y, double width, double height, bool reverse = false)
            {
                double dx = Factory.PrecisionModel.MakePrecise(width / 2);
                double dy = Factory.PrecisionModel.MakePrecise(height / 2);

                var seq = CsFactory.Create(5, ordinates);

                seq.SetOrdinate(0, Ordinate.X, Factory.PrecisionModel.MakePrecise(x - dx));
                seq.SetOrdinate(0, Ordinate.Y, Factory.PrecisionModel.MakePrecise(y - dy));
                seq.SetOrdinate(1, Ordinate.X, Factory.PrecisionModel.MakePrecise(x - dx));
                seq.SetOrdinate(1, Ordinate.Y, Factory.PrecisionModel.MakePrecise(y + dy));
                seq.SetOrdinate(2, Ordinate.X, Factory.PrecisionModel.MakePrecise(x + dx));
                seq.SetOrdinate(2, Ordinate.Y, Factory.PrecisionModel.MakePrecise(y + dy));
                seq.SetOrdinate(3, Ordinate.X, Factory.PrecisionModel.MakePrecise(x + dx));
                seq.SetOrdinate(3, Ordinate.Y, Factory.PrecisionModel.MakePrecise(y - dy));
                seq.SetOrdinate(4, Ordinate.X, Factory.PrecisionModel.MakePrecise(x - dx));
                seq.SetOrdinate(4, Ordinate.Y, Factory.PrecisionModel.MakePrecise(y - dy));

                if ((ordinates & (Ordinates.Z | Ordinates.M)) != Ordinates.None)
                {
                    int k = 0;
                    for (; k < 4; k++)
                    {
                        if ((ordinates & Ordinates.Z) == Ordinates.Z)
                            seq.SetOrdinate(k, Ordinate.Z, RandomOrdinate(Ordinate.Z, Factory.PrecisionModel));
                        if ((ordinates & Ordinates.Z) == Ordinates.Z)
                            seq.SetOrdinate(k, Ordinate.M, RandomOrdinate(Ordinate.M, Factory.PrecisionModel));
                    }
                    if ((ordinates & Ordinates.Z) == Ordinates.Z)
                        seq.SetOrdinate(k, Ordinate.Z, seq.GetOrdinate(0, Ordinate.Z));
                    if ((ordinates & Ordinates.M) == Ordinates.M)
                        seq.SetOrdinate(k, Ordinate.M, seq.GetOrdinate(0, Ordinate.M));
                }

                return Factory.CreateLinearRing(reverse ? seq.Reversed() : seq);
            }

            private static Geometry CreateMultiPolygon(Ordinates ordinates, bool empty)
            {
                if (empty)
                {
                    Factory.CreateMultiPolygon(null);
                }

                switch (Rnd.Next(2))
                {
                    case 0:
                        int numPolygons = Rnd.Next(4);
                        var polygons = new Polygon[numPolygons];
                        for (int i = 0; i < numPolygons; i++)
                            polygons[i] = (Polygon)CreatePolygon(ordinates, false);
                        return Factory.BuildGeometry(new Collection<Geometry>(polygons)).Union();

                    case 1:
                        polygons = new Polygon[2];
                        double radius = 5 * Rnd.NextDouble();
                        double x = RandomOrdinate(Ordinate.X, Factory.PrecisionModel);
                        double y = RandomOrdinate(Ordinate.Y, Factory.PrecisionModel);
                        var shell = CreateCircleRing(ordinates, x, y, radius);
                        var hole = CreateCircleRing(ordinates, x, y, 0.66 * radius, true);
                        polygons[0] = Factory.CreatePolygon(shell, new[] { hole });
                        shell = CreateCircleRing(ordinates, x, y, 0.5 * radius);
                        hole = CreateCircleRing(ordinates, x, y, 0.15 * radius, true);
                        polygons[1] = Factory.CreatePolygon(shell, new[] { hole });
                        return Factory.CreateMultiPolygon(polygons);

                    default:
                        throw new NotSupportedException();
                }
            }

            private static double RandomOrdinate(Ordinate o, PrecisionModel pm)
            {
                switch (o)
                {
                    case Ordinate.X:
                        return pm.MakePrecise(-180 + 360 * Rnd.NextDouble());
                    case Ordinate.Y:
                        return pm.MakePrecise(-90 + 180 * Rnd.NextDouble());
                    case Ordinate.Z:
                        return 200 * Rnd.NextDouble();
                    case Ordinate.M:
                        return 200 + 200 * Rnd.NextDouble();
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        [Test /*, ExpectedException(typeof(ArgumentException))*/]
        // see https://code.google.com/p/nettopologysuite/issues/detail?id=146
        public void Issue146_ShapeCreationWithInvalidAttributeName()
        {
            var points = new Coordinate[3];
            points[0] = new Coordinate(0, 0);
            points[1] = new Coordinate(1, 0);
            points[2] = new Coordinate(1, 1);
            var ls = new LineString(points);
            var mls = GeometryFactory.Default.CreateMultiLineString(new LineString[] { ls });

            var attrs = new AttributesTable();
            attrs.Add("Simulation name", "FOO");

            var features = new[] { new Feature(mls, attrs) };
            ShapefileDataWriter shp_writer = null;
            Assert.Throws<ArgumentException>(() => shp_writer =
                new ShapefileDataWriter("invalid_line_string")
                {
                    Header = ShapefileDataWriter.GetHeader(features[0], features.Length)
                });

            //Assert.Throws<ArgumentException>(() => shp_writer.Write(features));
        }

        [Test/*, ExpectedException(typeof(ArgumentException))*/]
        // see: https://github.com/NetTopologySuite/NetTopologySuite/issues/111
        public void issue_111_pointhandler_with_invalid_values()
        {
            var factory = GeometryFactory.Default;

            var p = factory.CreatePoint(new CoordinateZ(0, 0));
            Geometry[] arr = { p, GeometryCollection.Empty };
            var geometries = factory.CreateGeometryCollection(arr);

            var shapeType = Shapefile.GetShapeType(geometries);
            Assert.AreEqual(ShapeGeometryType.PointZM, shapeType);

            string tempPath = Path.GetTempFileName();
            var sfw = new ShapefileWriter(Path.GetFileNameWithoutExtension(tempPath), shapeType);
            Assert.Throws<ArgumentException>(() => sfw.Write(geometries));
        }

        [Test/*, ExpectedException(typeof(ArgumentException))*/]
        // see: https://github.com/NetTopologySuite/NetTopologySuite/issues/111
        public void issue_111_multiPointhandler_with_invalid_values()
        {
            var factory = GeometryFactory.Default;

            var p = factory.CreatePoint(new CoordinateZ(0, 0));
            var mp = factory.CreateMultiPoint(new[] { p });
            Geometry[] arr = new[] { mp, GeometryCollection.Empty };
            var geometries = factory.CreateGeometryCollection(arr);

            var shapeType = Shapefile.GetShapeType(geometries);
            Assert.AreEqual(ShapeGeometryType.MultiPointZM, shapeType);

            string tempPath = Path.GetTempFileName();
            var sfw = new ShapefileWriter(Path.GetFileNameWithoutExtension(tempPath), shapeType);
            Assert.Throws<ArgumentException>(() => sfw.Write(geometries));

        }

        [Test/*, ExpectedException(typeof(ArgumentException))*/]
        // see: https://github.com/NetTopologySuite/NetTopologySuite/issues/111
        public void issue_111_multilinehandler_with_invalid_values()
        {
            var factory = GeometryFactory.Default;

            var points = new Coordinate[3];
            points[0] = new CoordinateZ(0, 0);
            points[1] = new CoordinateZ(1, 0);
            points[2] = new CoordinateZ(1, 1);
            var ls = factory.CreateLineString(points);

            var mls = factory.CreateMultiLineString(new[] { ls });
            Geometry[] arr = new[] { mls, GeometryCollection.Empty };
            var geometries = factory.CreateGeometryCollection(arr);

            var shapeType = Shapefile.GetShapeType(geometries);
            Assert.AreEqual(ShapeGeometryType.LineStringZM, shapeType);

            string tempPath = Path.GetTempFileName();
            var sfw = new ShapefileWriter(Path.GetFileNameWithoutExtension(tempPath), shapeType);
            Assert.Throws<ArgumentException>(() => sfw.Write(geometries));
        }

        [Test/*, ExpectedException(typeof(ArgumentException))*/]
        // see: https://github.com/NetTopologySuite/NetTopologySuite/issues/111
        public void issue_111_polygonhandler_with_invalid_values()
        {
            var factory = GeometryFactory.Default;

            var points = new Coordinate[5];
            points[0] = new CoordinateZ(0, 0);
            points[1] = new CoordinateZ(1, 0);
            points[2] = new CoordinateZ(1, 1);
            points[3] = new CoordinateZ(0, 1);
            points[4] = new CoordinateZ(0, 0);
            var poly = factory.CreatePolygon(points);

            var mpoly = factory.CreateMultiPolygon(new[] { poly });
            Geometry[] arr = new[] { mpoly, GeometryCollection.Empty };
            var geometries = factory.CreateGeometryCollection(arr);

            var shapeType = Shapefile.GetShapeType(geometries);
            Assert.AreEqual(ShapeGeometryType.PolygonZM, shapeType);

            string tempPath = Path.GetTempFileName();
            var sfw = new ShapefileWriter(Path.GetFileNameWithoutExtension(tempPath), shapeType);

            Assert.Throws<ArgumentException>(() => sfw.Write(geometries));
        }

        // ESRI spec says index file is mandatory, but we do allow writing a
        // shapefile without it if someone specifically asks us to.
        [Test]
        public void WriteShouldWorkWithoutIndexFileWhenRequested()
        {
            var pt = GeometryFactory.Default.CreatePoint(new Coordinate(2, 3));
            var attributes = new AttributesTable { { "Foo", "Bar" } };
            Feature[] features = { new Feature(pt, attributes) };

            string baseFileName = TestContext.CurrentContext.Test.ID;
            string shpFilePath = baseFileName + ".shp";
            string dbfFilePath = baseFileName + ".dbf";
            string shxFilePath = baseFileName + ".shx";

            var reg = new ShapefileStreamProviderRegistry(
                shapeStream: new FileStreamProvider(StreamTypes.Shape, shpFilePath),
                dataStream: new FileStreamProvider(StreamTypes.Data, dbfFilePath),
                indexStream: null,
                validateShapeProvider: true,
                validateDataProvider: true,
                validateIndexProvider: false);

            var wr = new ShapefileDataWriter(reg, GeometryFactory.Default, CodePagesEncodingProvider.Instance.GetEncoding(1252));
            wr.Header = ShapefileDataWriter.GetHeader(features[0], features.Length);
            wr.Write(features);

            Assert.True(File.Exists(shpFilePath));
            Assert.True(File.Exists(dbfFilePath));
            Assert.False(File.Exists(shxFilePath));
        }

        private static Ordinate[] ToOrdinateArray(Ordinates ordinates)
        {
            var result = new Ordinate[OrdinatesUtility.OrdinatesToDimension(ordinates)];
            int nextIndex = 0;
            for (int i = 0; i < 32; i++)
            {
                if (ordinates.HasFlag((Ordinates)(1 << i)))
                {
                    result[nextIndex++] = (Ordinate)i;
                }
            }

            return result;
        }

        [Test, ShapeFileIssueNumber(24)]
        public void WriteShouldWriteMultiPoints()
        {
            var attribs = new AttributesTable
            {
                { "Id", 10 }
            };
            var coors = new Coordinate[2]
            {
                new Coordinate(123.0, 023.0),
                new Coordinate(123.0, 100.0)
            };
            var points = Factory.CreateMultiPointFromCoords(coors);
            var feature = new Feature(points, attribs);

            var filename = Path.ChangeExtension(Path.GetTempFileName(), ".shp");
            var writer = new ShapefileDataWriter(filename, Factory)
            {
                Header = ShapefileDataWriter.GetHeader(feature, 1)
            };
            writer.Write(new[] { feature });

            var reader = new ShapefileDataReader(filename, Factory);
            while (reader.Read())
            {
                Assert.IsNotNull(reader.Geometry);
                Assert.IsInstanceOf<MultiPoint>(reader.Geometry);
                Assert.AreEqual(1, reader.GetOrdinal("Id"));
                Assert.AreEqual(10, reader.GetInt32(1));
            }
        }
    }
}
