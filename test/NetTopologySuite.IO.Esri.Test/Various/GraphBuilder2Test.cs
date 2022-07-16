using NetTopologySuite.Geometries;
using NetTopologySuite.Features;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test.Various
{
    [TestFixture]
    public class GraphBuilder2Test
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            factory = GeometryFactory.Fixed;

            a = factory.CreateLineString(new Coordinate[]
                                             {
                                                 new Coordinate(0, 0),
                                                 new Coordinate(100, 0),
                                                 new Coordinate(200, 100),
                                                 new Coordinate(200, 200),
                                             });
            b = factory.CreateLineString(new Coordinate[]
                                             {
                                                 new Coordinate(0, 0),
                                                 new Coordinate(100, 100),
                                                 new Coordinate(200, 200),
                                             });
            c = factory.CreateLineString(new Coordinate[]
                                             {
                                                 new Coordinate(0, 0),
                                                 new Coordinate(0, 100),
                                                 new Coordinate(100, 200),
                                                 new Coordinate(200, 200),
                                             });
            d = factory.CreateLineString(new Coordinate[]
                                             {
                                                 new Coordinate(0, 0),
                                                 new Coordinate(300, 0),
                                                 new Coordinate(300, 200),
                                                 new Coordinate(150, 200),
                                                 new Coordinate(150, 300),
                                             });
            e = factory.CreateLineString(new Coordinate[]
                                             {
                                                 new Coordinate(100, 300),
                                                 new Coordinate(150, 300),
                                                 new Coordinate(200, 300),
                                             });

            result = factory.CreateLineString(new Coordinate[]
                                                  {
                                                      new Coordinate(0, 0),
                                                      new Coordinate(300, 0),
                                                      new Coordinate(300, 200),
                                                      new Coordinate(150, 200),
                                                      new Coordinate(150, 300),
                                                  });
            revresult = (LineString)result.Reverse();

            start = a.StartPoint;
            end = d.EndPoint;
        }

        #endregion

        private const string shp = ".shp";
        private const string shx = ".shx";
        private const string dbf = ".dbf";

        private GeometryFactory factory;
        private LineString a, b, c, d, e;
        private LineString result, revresult;
        private Point start, end;

        /// <summary>
        /// Loads the shapefile as a graph allowing SP analysis to be carried out
        /// </summary>
        /// <param name="fileName">The name of the shape file we want to load</param>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public LineString TestGraphBuilder2WithSampleGeometries(string fileName, Coordinate src, Coordinate dst)
        {
            var reader = new ShapefileReader(fileName);
            var edges = reader.ReadAll();
            return TestGraphBuilder2WithSampleGeometries(edges, src, dst);
        }

        /// <summary>
        /// Uses the passed geometry collection to generate a QuickGraph.
        /// </summary>
        /// <param name="edges"></param>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public LineString TestGraphBuilder2WithSampleGeometries(GeometryCollection edges, Coordinate src,
                                                                 Coordinate dst)
        {
            var builder = new GraphBuilder2(true);
            foreach (MultiLineString edge in edges.Geometries)
                foreach (LineString line in edge.Geometries)
                    builder.Add(line);
            builder.Initialize();

            return builder.Perform(src, dst);
        }

        [SetUp]
        public void FixtureSetup()
        {
            Environment.CurrentDirectory = CommonHelpers.TestShapefilesDirectory;
        }

        private void SaveGraphResult(Geometry path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            const string shapepath = "graphresult";
            if (File.Exists(shapepath + shp))
                File.Delete(shapepath + shp);
            Assert.IsFalse(File.Exists(shapepath + shp));
            if (File.Exists(shapepath + shx))
                File.Delete(shapepath + shx);
            Assert.IsFalse(File.Exists(shapepath + shx));
            if (File.Exists(shapepath + dbf))
                File.Delete(shapepath + dbf);
            Assert.IsFalse(File.Exists(shapepath + dbf));

            const string field1 = "OBJECTID";
            var feature = new Feature(path, new AttributesTable());
            feature.Attributes.Add(field1, 0);

            var header = new DbaseFileHeader { NumRecords = 1, NumFields = 1 };
            header.AddColumn(field1, 'N', 5, 0);

            var writer = new ShapefileDataWriter(shapepath, factory) { Header = header };
            writer.Write(new List<IFeature>(new[] { feature, }));

            Assert.IsTrue(File.Exists(shapepath + shp));
            Assert.IsTrue(File.Exists(shapepath + shx));
            Assert.IsTrue(File.Exists(shapepath + dbf));
        }

        [Test]
        [Ignore("graph.shp not present")]
        public void BuildGraphFromCompleteGraphShapefile()
        {
            const string shapepath = "graph.shp";
            const int count = 1179;

            Assert.IsTrue(File.Exists(shapepath));
            var reader = new ShapefileReader(shapepath);
            var edges = reader.ReadAll();
            Assert.IsNotNull(edges);
            Assert.IsInstanceOf(typeof(GeometryCollection), edges);
            Assert.AreEqual(count, edges.NumGeometries);

            var startls = edges.GetGeometryN(515).GetGeometryN(0) as LineString;
            Assert.IsNotNull(startls);
            var startPoint = startls.EndPoint;
            Assert.AreEqual(2317300d, startPoint.X);
            Assert.AreEqual(4843961d, startPoint.Y);

            var endls = edges.GetGeometryN(141).GetGeometryN(0) as LineString;
            ;
            Assert.IsNotNull(endls);
            var endPoint = endls.StartPoint;
            Assert.AreEqual(2322739d, endPoint.X);
            Assert.AreEqual(4844539d, endPoint.Y);

            var builder = new GraphBuilder2(true);
            foreach (MultiLineString mlstr in edges.Geometries)
            {
                Assert.AreEqual(1, mlstr.NumGeometries);
                var str = mlstr.GetGeometryN(0) as LineString;
                Assert.IsNotNull(str);
                Assert.IsTrue(builder.Add(str));
            }
            builder.Initialize();

            var path = builder.Perform(startPoint, endPoint);
            Assert.IsNotNull(path);
            SaveGraphResult(path);

            var reverse = builder.Perform(endPoint, startPoint);
            Assert.IsNotNull(reverse);
            Assert.AreEqual(path, reverse.Reverse());
        }

        [Test]
        [Ignore("minimalgraph.shp not present")]
        public void BuildGraphFromMinimalGraphShapefile()
        {
            const string shapepath = "minimalgraph.shp";
            const int count = 15;

            Assert.IsTrue(File.Exists(shapepath));
            var reader = new ShapefileReader(shapepath);
            var edges = reader.ReadAll();
            Assert.IsNotNull(edges);
            Assert.IsInstanceOf(typeof(GeometryCollection), edges);
            Assert.AreEqual(count, edges.NumGeometries);

            var startls = edges.GetGeometryN(0).GetGeometryN(0) as LineString;
            Assert.IsNotNull(startls);
            var endls = edges.GetGeometryN(5).GetGeometryN(0) as LineString;
            ;
            Assert.IsNotNull(endls);

            var builder = new GraphBuilder2(true);
            foreach (MultiLineString mlstr in edges.Geometries)
            {
                Assert.AreEqual(1, mlstr.NumGeometries);
                var str = mlstr.GetGeometryN(0) as LineString;
                Assert.IsNotNull(str);
                Assert.IsTrue(builder.Add(str));
            }
            builder.Initialize();

            var path = builder.Perform(startls.StartPoint, endls.EndPoint);
            Assert.IsNotNull(path);
        }

        [Test]
        [Ignore("strade_fixed.shp not present")]
        public void BuildGraphFromStradeShapefile()
        {
            string shapepath = "strade_fixed.shp";
            int count = 703;

            Assert.IsTrue(File.Exists(shapepath));
            var reader = new ShapefileReader(shapepath);
            var edges = reader.ReadAll();
            Assert.IsNotNull(edges);
            Assert.IsInstanceOf(typeof(GeometryCollection), edges);
            Assert.AreEqual(count, edges.NumGeometries);

            var startCoord = new Coordinate(2317300d, 4843961d);
            var endCoord = new Coordinate(2322739d, 4844539d);

            bool startFound = false;
            bool endFound = false;
            var builder = new GraphBuilder2(true);
            foreach (MultiLineString mlstr in edges.Geometries)
            {
                Assert.AreEqual(1, mlstr.NumGeometries);
                var str = mlstr.GetGeometryN(0) as LineString;
                Assert.IsNotNull(str);
                Assert.IsTrue(builder.Add(str));

                if (!startFound)
                {
                    var coords = new List<Coordinate>(str.Coordinates);
                    if (coords.Contains(startCoord))
                        startFound = true;
                }

                if (!endFound)
                {
                    var coords = new List<Coordinate>(str.Coordinates);
                    if (coords.Contains(endCoord))
                        endFound = true;
                }
            }
            builder.Initialize();
            Assert.IsTrue(startFound);
            Assert.IsTrue(endFound);

            var path = builder.Perform(startCoord, endCoord);
            Assert.IsNotNull(path);
            SaveGraphResult(path);

            var reverse = builder.Perform(startCoord, endCoord);
            Assert.IsNotNull(reverse);
            Assert.AreEqual(path, reverse.Reverse());
        }

        [Ignore("")]
        [Test]
        public void BuildStradeFixed()
        {
            string path = "strade" + shp;
            Assert.IsTrue(File.Exists(path));

            var reader = new ShapefileDataReader(path, factory);
            var features = new List<IFeature>(reader.RecordCount);
            while (reader.Read())
            {
                var feature = new Feature(reader.Geometry, new AttributesTable());
                object[] values = new object[reader.FieldCount - 1];
                reader.GetValues(values);
                for (int i = 0; i < values.Length; i++)
                {
                    string name = reader.GetName(i + 1);
                    object value = values[i];
                    feature.Attributes.Add(name, value);
                }
                features.Add(feature);
            }
            Assert.AreEqual(703, features.Count);

            string shapepath = "strade_fixed";
            if (File.Exists(shapepath + shp))
                File.Delete(shapepath + shp);
            Assert.IsFalse(File.Exists(shapepath + shp));
            if (File.Exists(shapepath + shx))
                File.Delete(shapepath + shx);
            Assert.IsFalse(File.Exists(shapepath + shx));
            if (File.Exists(shapepath + dbf))
                File.Delete(shapepath + dbf);
            Assert.IsFalse(File.Exists(shapepath + dbf));

            var header = reader.DbaseHeader;

            var writer = new ShapefileDataWriter(shapepath, factory);
            writer.Header = header;
            writer.Write(features);

            Assert.IsTrue(File.Exists(shapepath + shp));
            Assert.IsTrue(File.Exists(shapepath + shx));
            Assert.IsTrue(File.Exists(shapepath + dbf));
        }

        [Test]
        public void CheckGraphBuilder2ExceptionUsingARepeatedGeometry()
        {
            Assert.Catch<TopologyException>(() =>
            {
                var builder = new GraphBuilder2();
                Assert.IsTrue(builder.Add(a));
                Assert.IsFalse(builder.Add(a));
                Assert.IsFalse(builder.Add(a, a));
                builder.Initialize();
            });
        }

        [Test]
        public void CheckGraphBuilder2ExceptionUsingDifferentFactories()
        {
            Assert.Catch<TopologyException>(() =>
            {
                var builder = new GraphBuilder2();
                Assert.IsTrue(builder.Add(a));
                Assert.IsTrue(builder.Add(b, c));
                Assert.IsTrue(builder.Add(d));
                builder.Add(GeometryFactory.Default.CreateLineString(new Coordinate[]
                                                                         {
                                                                         new Coordinate(0, 0),
                                                                         new Coordinate(50, 50),
                                                                         }));
            });
        }

        [Test]
        public void CheckGraphBuilder2ExceptionUsingDoubleInitialization()
        {
            var builder = new GraphBuilder2();
            builder.Add(a);
            builder.Add(b, c);
            builder.Add(d);
            builder.Add(e);
            builder.Initialize();
            builder.Initialize();
        }

        [Test]
        public void CheckGraphBuilder2ExceptionUsingNoGeometries()
        {
            Assert.Catch<TopologyException>(() =>
            {
                var builder = new GraphBuilder2();
                builder.Initialize();
            });
        }

        [Test]
        public void CheckGraphBuilder2ExceptionUsingOneGeometry()
        {
            Assert.Catch<TopologyException>(() =>
            {
                var builder = new GraphBuilder2();
                Assert.IsTrue(builder.Add(a));
                builder.Initialize();
            });
        }

        [Test]
        public void TestBidirectionalGraphBuilder2WithSampleGeometries()
        {
            var builder = new GraphBuilder2(true);
            builder.Add(a);
            builder.Add(b, c);
            builder.Add(d);
            builder.Add(e);
            builder.Initialize();

            var path = builder.Perform(start.Coordinate, end.Coordinate);
            Assert.IsNotNull(path);
            Assert.AreEqual(result, path);

            var revpath = builder.Perform(end, start);
            Assert.IsNotNull(revpath);
            Assert.AreEqual(revresult, revpath);
        }

        [Test]
        public void TestGraphBuilder2WithSampleGeometries()
        {
            var builder = new GraphBuilder2();
            builder.Add(a);
            builder.Add(b, c);
            builder.Add(d);
            builder.Add(e);
            builder.Initialize();

            var path = builder.Perform(start, end);
            Assert.IsNotNull(path);
            Assert.AreEqual(result, path);
        }
    }
}
