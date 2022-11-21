using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;
using System;
using System.Collections.Generic;

namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public class Shapefile_WriteLineFeatures : Test
    {
        public override void Run()
        {
            var shpPath = GetTempFilePath("abcd1.shp");

            var options = new ShapefileWriterOptions(ShapeType.PolyLine);
            options.AddField("date", typeof(DateTime));
            options.AddField("float", typeof(double));
            options.AddField("int", typeof(int));
            options.AddField("logical", typeof(bool));
            options.AddField("text", typeof(string));

            var features = GetFeatures();
            using (var shp = new ShapefilePolyLineWriter(shpPath, options))
            {
                shp.Write(features);
                Console.WriteLine($"{features.Count} features was written.");
            }

            foreach (var feature in Shapefile.ReadAllFeatures(shpPath))
            {
                PrintFeature(feature);
            }
        }

        private static List<Feature> GetFeatures()
        {
            var features = new List<Feature>();
            for (int i = 1; i < 5; i++)
            {
                var p1 = new CoordinateZ(i, i + 1, i);
                var p2 = new CoordinateZ(i, i, i);
                var p3 = new CoordinateZ(i + 1, i, i);
                var line = GeometryFactory.Default.CreateLineString(new Coordinate[] { p1, p2, p3 });
                var mline = GeometryFactory.Default.CreateMultiLineString(new LineString[] { line });

                var attributes = new AttributesTable(new(StringComparer.Ordinal))
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
            return features;
        }
    }
}
