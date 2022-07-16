using NetTopologySuite.Geometries;
using NetTopologySuite.Features;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite.IO.ShapeFile.Test.Attributes
{
    public class AttributesTest
    {
        protected GeometryFactory Factory { get; private set; }

        protected WKTReader Reader { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public void Start()
        {
            // Set current dir to shapefiles dir
            Environment.CurrentDirectory = CommonHelpers.TestShapefilesDirectory;

            this.Factory = new GeometryFactory();
            this.Reader = new WKTReader();

            // ReadFromShapeFile();
            // TestSharcDbf();
            TestShapeCreation();
        }

        private void TestShapeCreation()
        {
            var points = new Coordinate[3];
            points[0] = new Coordinate(0, 0);
            points[1] = new Coordinate(1, 0);
            points[2] = new Coordinate(1, 1);

            var line_string = new LineString(points);

            var attributes = new AttributesTable();
            attributes.Add("FOO", "FOO");

            var feature = new Feature(Factory.CreateMultiLineString(new LineString[] { line_string }), attributes);
            var features = new Feature[1];
            features[0] = feature;

            var shp_writer = new ShapefileDataWriter("line_string")
            {
                Header = ShapefileDataWriter.GetHeader(features[0], features.Length)
            };
            shp_writer.Write(features);
        }

        [Test]
        public void TestConstructor2()
        {
            IAttributesTable at = null;
            /*
            Assert.DoesNotThrow(
                () => at = new AttributesTable(new[] {new[] {"key1", new object()}, new[] {"key2", new object()}}));
            Assert.That(at, Is.Not.Null);
            Assert.That(at.Count, Is.EqualTo(2));
            Assert.That(at.Exists("key1"), Is.True);
            Assert.That(at.Exists("key2"), Is.True);
            Assert.Throws<ArgumentException>(
                () => at = new AttributesTable(new[] {new[] {"key1", new object()}, new[] {(object) "key2",}}));
            Assert.Throws<ArgumentException>(
                () => at = new AttributesTable(new[] {new[] {"key1", new object()}, new[] {new object(), "item2",}}));
             */
            Assert.DoesNotThrow(() => at = new AttributesTable { { "key1", new object() }, { "key2", new object() } });
            Assert.That(at, Is.Not.Null);
            Assert.That(at.Count, Is.EqualTo(2));
            Assert.That(at.Exists("key1"), Is.True);
            Assert.That(at.Exists("key2"), Is.True);
        }

        private void TestSharcDbf()
        {
            const string filename = @"Strade.dbf";
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename + " not found at " + Environment.CurrentDirectory);

            var reader = new DbaseFileReader(filename);
            var header = reader.GetHeader();
            Console.WriteLine("HeaderLength: " + header.HeaderLength);
            Console.WriteLine("RecordLength: " + header.RecordLength);
            Console.WriteLine("NumFields: " + header.NumFields);
            Console.WriteLine("NumRecords: " + header.NumRecords);
            Console.WriteLine("LastUpdateDate: " + header.LastUpdateDate);
            foreach (var descr in header.Fields)
            {
                Console.WriteLine("FieldName: " + descr.Name);
                Console.WriteLine("DBF Type: " + descr.DbaseType);
                Console.WriteLine("CLR Type: " + descr.Type);
                Console.WriteLine("Length: " + descr.Length);
                Console.WriteLine("DecimalCount: " + descr.DecimalCount);
                Console.WriteLine("DataAddress: " + descr.DataAddress);
            }

            var ienum = reader.GetEnumerator();
            while (ienum.MoveNext())
            {
                var objs = (ArrayList)ienum.Current;
                foreach (object obj in objs)
                    Console.WriteLine(obj);
            }
            Console.WriteLine();
        }

        private void ReadFromShapeFile()
        {
            var featureCollection = new List<IFeature>();
            const string filename = @"country";
            if (!File.Exists(filename + ".dbf"))
                throw new FileNotFoundException(filename + " not found at " + Environment.CurrentDirectory);
            var dataReader = new ShapefileDataReader(filename, new GeometryFactory());
            while (dataReader.Read())
            {
                var feature = new Feature { Geometry = dataReader.Geometry };

                int length = dataReader.DbaseHeader.NumFields;
                string[] keys = new string[length];
                for (int i = 0; i < length; i++)
                    keys[i] = dataReader.DbaseHeader.Fields[i].Name;

                feature.Attributes = new AttributesTable();
                for (int i = 0; i < length; i++)
                {
                    object val = dataReader.GetValue(i);
                    feature.Attributes.Add(keys[i], val);
                }

                featureCollection.Add(feature);
            }

            int index = 0;
            Console.WriteLine("Elements = " + featureCollection.Count);
            foreach (var feature in featureCollection)
            {
                Console.WriteLine("Feature " + index++);
                var table = feature.Attributes as AttributesTable;
                foreach (string name in table.GetNames())
                    Console.WriteLine(name + ": " + table[name]);
            }

            //Directory
            string dir = CommonHelpers.TestShapefilesDirectory + Path.DirectorySeparatorChar;
            // Test write with stub header
            string file = dir + "testWriteStubHeader";
            if (File.Exists(file + ".shp")) File.Delete(file + ".shp");
            if (File.Exists(file + ".shx")) File.Delete(file + ".shx");
            if (File.Exists(file + ".dbf")) File.Delete(file + ".dbf");

            var dataWriter = new ShapefileDataWriter(file);
            dataWriter.Header = ShapefileDataWriter.GetHeader(featureCollection[0] as IFeature, featureCollection.Count);
            dataWriter.Write(featureCollection);

            // Test write with header from a existing shapefile
            file = dir + "testWriteShapefileHeader";
            if (File.Exists(file + ".shp")) File.Delete(file + ".shp");
            if (File.Exists(file + ".shx")) File.Delete(file + ".shx");
            if (File.Exists(file + ".dbf")) File.Delete(file + ".dbf");

            dataWriter = new ShapefileDataWriter(file)
            {
                Header =
                    ShapefileDataWriter.GetHeader(dir + "country.dbf")
            };
            dataWriter.Write(featureCollection);
        }
    }
}
