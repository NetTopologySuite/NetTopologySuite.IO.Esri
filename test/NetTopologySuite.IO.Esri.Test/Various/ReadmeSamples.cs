using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf;
using NetTopologySuite.IO.Esri.Shp.Readers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test.Various
{
    /// <summary>
    /// Tests for samples included int the README.md
    /// </summary>
    internal class ReadmeSamples
    {
        private readonly string dbfPath;
        private readonly string shpPath;

        public ReadmeSamples()
        {
            shpPath = TestShapefiles.PathTo("fields_utf8.shp");
            dbfPath = Path.ChangeExtension(shpPath, ".dbf");
        }

        [Test]
        public void DbfReader()
        {
            using var dbf = new DbfReader(dbfPath);
            foreach (var record in dbf)
            {
                foreach (var fieldName in record.GetNames())
                {
                    Console.WriteLine($"{fieldName,10} {record[fieldName]}");
                }
                Console.WriteLine();
            }
        }

        [Test]
        public void ShpReader()
        {
            foreach (var geometry in Shapefile.ReadAllGeometries(shpPath))
            {
                Console.WriteLine(geometry);
            }
        }

        [Test]
        public void ShapefileReader()
        {
            foreach (var feature in Shapefile.ReadAllFeatures(shpPath))
            {
                foreach (var attrName in feature.Attributes.GetNames())
                {
                    Console.WriteLine($"{attrName,10}: {feature.Attributes[attrName]}");
                }
                Console.WriteLine($"     SHAPE: {feature.Geometry}");
                Console.WriteLine();
            }
        }

        [Test]
        public void ShapefileWriter()
        {
            var shpPath = TestShapefiles.GetTempShpPath();

            var features = new List<Feature>();
            for (int i = 1; i < 5; i++)
            {
                var lineCoords = new List<CoordinateZ>
                {
                    new CoordinateZ(i, i + 1, i),
                    new CoordinateZ(i, i, i),
                    new CoordinateZ(i + 1, i, i)
                };
                var line = new LineString(lineCoords.ToArray());
                var mline = new MultiLineString(new LineString[] { line });

                var attributes = new AttributesTable
                {
                    { "date", new DateTime(2000, 1, i + 1) },
                    { "float", i * 0.1 },
                    { "int", i },
                    { "logical", i % 2 == 0 },
                    { "text", i.ToString("0.00") }
                };

                var feature = new Feature(mline, attributes);
                features.Add(feature);
            }
            Shapefile.WriteAllFeatures(features, shpPath);

            TestShapefiles.DeleteShp(shpPath);
        }

    }
}
