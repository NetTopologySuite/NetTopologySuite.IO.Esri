using NetTopologySuite.Geometries;
using NetTopologySuite.Features;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.Attributes
{
    public class AttributesTest
    { 
        [Test]
        public void TestShapeCreation()
        {
            var points = new Coordinate[3];
            points[0] = new Coordinate(0, 0);
            points[1] = new Coordinate(1, 0);
            points[2] = new Coordinate(1, 1);

            var line_string = new LineString(points);

            var attributes = new AttributesTable();
            attributes.Add("FOO", "FOO");

            var factory = GeometryFactory.Default;
            var feature = new Feature(factory.CreateMultiLineString(new LineString[] { line_string }), attributes);
            var features = new Feature[1];
            features[0] = feature;

            Shapefile.WriteAllFeatures(features, "line_string");
            Assert.That(File.Exists("line_string.shp"));
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

        [Test]
        public void TestSharcDbf()
        {
            string filename = TestShapefiles.PathToCountriesPt(".dbf");
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename + " not found at " + Environment.CurrentDirectory);

            using var reader = new Dbf.DbfReader(filename);
            Console.WriteLine("RecordLength: " + reader.RecordSize);
            Console.WriteLine("NumFields: " + reader.Fields.Count);
            Console.WriteLine("NumRecords: " + reader.RecordCount);
            Console.WriteLine("LastUpdateDate: " + reader.LastUpdateDate);
            foreach (var descr in reader.Fields)
            {
                Console.WriteLine("FieldName: " + descr.Name);
                Console.WriteLine("DBF Type: " + descr.FieldType);
                Console.WriteLine("Length: " + descr.Length);
                Console.WriteLine("DecimalCount: " + descr.NumericScale);
            }

            foreach (var record in reader)
            {
                foreach (var name in record.GetNames())
                {
                    Console.WriteLine(name + ": " + record[name]);
                }
            }
            Console.WriteLine();
        }

        [Test]
        public void ReadFromShapeFile()
        {
            var featureCollection = new List<IFeature>();
            string filename = TestShapefiles.PathToCountriesPt();
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename + " not found");
            using var dataReader = Shapefile.OpenRead(filename);
            while (dataReader.Read())
            {
                var feature = new Feature { Geometry = dataReader.Geometry };

                int length = dataReader.Fields.Count;
                string[] keys = new string[length];
                for (int i = 0; i < length; i++)
                    keys[i] = dataReader.Fields[i].Name;

                feature.Attributes = new AttributesTable();
                for (int i = 0; i < length; i++)
                {
                    object val = dataReader.Fields[i].Value;
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

            // Test write with stub header
            string stubHeaderFile = TestShapefiles.PathTo("testWriteStubHeader");
            TestShapefiles.DeleteShp(stubHeaderFile);
            Shapefile.WriteAllFeatures(featureCollection, stubHeaderFile);

            // Test write with header from a existing shapefile
            string shpHeaderFile = TestShapefiles.PathTo("testWriteShapefileHeader");
            TestShapefiles.DeleteShp(shpHeaderFile);

            using var stubHeaderReader = Shapefile.OpenRead(stubHeaderFile);
            var options = new ShapefileWriterOptions(stubHeaderReader);
            using var dataWriter = Shapefile.OpenWrite(shpHeaderFile, options); 
            dataWriter.Write(featureCollection);
        }
    }
}
