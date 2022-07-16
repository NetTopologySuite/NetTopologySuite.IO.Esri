using NetTopologySuite.Geometries;
using NetTopologySuite.Features;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test.Various
{
    [TestFixture]
    public class PathFinderTest
    {
        private const string shp = ".shp";
        private const string shx = ".shx";
        private const string dbf = ".dbf";

        private GeometryFactory factory;

        [SetUp]
        public void FixtureSetup()
        {
            Environment.CurrentDirectory = CommonHelpers.TestShapefilesDirectory;

            factory = GeometryFactory.Fixed;
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

            var writer = new ShapefileDataWriter(shapepath, factory) { Header = header };
            writer.Write(features);

            Assert.IsTrue(File.Exists(shapepath + shp));
            Assert.IsTrue(File.Exists(shapepath + shx));
            Assert.IsTrue(File.Exists(shapepath + dbf));
        }

        private Geometry LoadGraphResult()
        {
            string path = "graphresult.shp";
            Assert.IsTrue(Path.GetExtension(path) == shp);

            var reader = new ShapefileReader(path);
            var coll = reader.ReadAll();
            Assert.AreEqual(1, coll.Count);

            var geom = coll.GetGeometryN(0);
            Assert.IsInstanceOf(typeof(MultiLineString), geom);
            var str = geom.GetGeometryN(0);
            Assert.IsInstanceOf(typeof(LineString), str);
            return str;
        }

        private void SaveGraphResult(Geometry path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            string shapepath = "graphresult";
            if (File.Exists(shapepath + shp))
                File.Delete(shapepath + shp);
            Assert.IsFalse(File.Exists(shapepath + shp));
            if (File.Exists(shapepath + shx))
                File.Delete(shapepath + shx);
            Assert.IsFalse(File.Exists(shapepath + shx));
            if (File.Exists(shapepath + dbf))
                File.Delete(shapepath + dbf);
            Assert.IsFalse(File.Exists(shapepath + dbf));

            string field1 = "OBJECTID";
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
        [Ignore("graph.shp is not present")]
        public void BuildGraphFromCompleteGraphShapefile()
        {
            string shapepath = "graph.shp";
            int count = 1179;

            Assert.IsTrue(File.Exists(shapepath), string.Format("File not found: '{0}'", shapepath));
            var reader = new ShapefileReader(shapepath);
            var edges = reader.ReadAll();
            Assert.IsNotNull(edges);
            Assert.IsInstanceOf(typeof(GeometryCollection), edges);
            Assert.AreEqual(count, edges.NumGeometries);

            // Insert arbitrary userdata
            for (int i = 0; i < count; i++)
            {
                var g = edges.GetGeometryN(i) as MultiLineString;
                Assert.IsNotNull(g);
                var ls = g.GetGeometryN(0) as LineString;
                Assert.IsNotNull(ls);

                Assert.IsNull(ls.UserData);
                ls.UserData = i;
                Assert.IsNotNull(ls.UserData);
            }

            var startls = edges.GetGeometryN(515).GetGeometryN(0) as LineString;
            Assert.IsNotNull(startls);
            var startPoint = startls.EndPoint;
            Assert.AreEqual(2317300d, startPoint.X);
            Assert.AreEqual(4843961d, startPoint.Y);

            var endls = edges.GetGeometryN(141).GetGeometryN(0) as LineString; ;
            Assert.IsNotNull(endls);
            var endPoint = endls.StartPoint;
            Assert.AreEqual(2322739d, endPoint.X);
            Assert.AreEqual(4844539d, endPoint.Y);

            var finder = new PathFinder(true);
            foreach (MultiLineString mlstr in edges.Geometries)
            {
                Assert.AreEqual(1, mlstr.NumGeometries);
                var str = mlstr.GetGeometryN(0) as LineString;
                Assert.IsNotNull(str);
                Assert.IsNotNull(str.UserData);
                Assert.IsTrue(finder.Add(str));
            }
            finder.Initialize();

            int expectedResultCount = 8;
            var path = finder.Find(startPoint, endPoint);
            Assert.IsNotNull(path);
            Assert.IsInstanceOf(typeof(MultiLineString), path);
            var strings = (MultiLineString)path;
            Assert.AreEqual(expectedResultCount, strings.NumGeometries);
            foreach (var g in strings.Geometries)
            {
                Assert.IsNotNull(g.UserData);
                Console.WriteLine("{0} : {1}", g.UserData, g);
            }

            var reversedPath = finder.Find(endPoint, startPoint);
            Assert.IsNotNull(reversedPath);
            Assert.IsInstanceOf(typeof(MultiLineString), reversedPath);

            var reversedStrings = (MultiLineString)reversedPath;
            Assert.AreEqual(expectedResultCount, reversedStrings.NumGeometries);
            foreach (var g in reversedStrings.Geometries)
            {
                Assert.IsNotNull(g.UserData);
                Console.WriteLine("{0} : {1}", g.UserData, g);
            }

            for (int i = 0; i < expectedResultCount; i++)
            {
                var item = strings.GetGeometryN(i);
                var itemReversed = strings.GetGeometryN(expectedResultCount - 1 - i);
                Assert.AreNotEqual(item.UserData, itemReversed.UserData);
                Assert.AreNotEqual(item, itemReversed);
            }
        }
    }
}
