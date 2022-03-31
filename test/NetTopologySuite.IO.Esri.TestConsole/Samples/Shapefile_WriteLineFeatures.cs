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


            var dateField = DbfField.Create("date", typeof(DateTime));
            var floatField = DbfField.Create("float", typeof(double));
            var intField = DbfField.Create("int", typeof(int));
            var LogicalField = DbfField.Create("logical", typeof(bool));
            var textField = DbfField.Create("text", typeof(string));

            var features = GetFeatures();
            using (var shp = new ShapefilePolyLineWriter(shpPath, ShapeType.PolyLine, dateField, floatField, intField, LogicalField, textField))
            {
                shp.Write(features);
                Console.WriteLine($"{features.Count} features was written.");
            }

            foreach (var feature in Shapefile.ReadAllFeatures(shpPath))
            {
                PrintFeature(feature);
            }
        }

        private List<Feature> GetFeatures()
        {
            var features = new List<Feature>();
            for (int i = 1; i < 5; i++)
            {
                var p1 = new CoordinateZ(i, i + 1, i);
                var p2 = new CoordinateZ(i, i, i);
                var p3 = new CoordinateZ(i + 1, i, i);
                var line = GeometryFactory.Default.CreateLineString(new Coordinate[] { p1, p2, p3 });
                var mline = GeometryFactory.Default.CreateMultiLineString(new LineString[] { line });

                var attributes = new AttributesTable(StringComparer.Ordinal);
                attributes.Add("date", new DateTime(2000, 1, i + 1));
                attributes.Add("float", i * 0.1);
                attributes.Add("int", i);
                attributes.Add("logical", i % 2 == 0);
                attributes.Add("text", i.ToString("0.00"));

                var feature = new Feature(mline, attributes);
                features.Add(feature);
            }
            return features;
        }
    }
}
