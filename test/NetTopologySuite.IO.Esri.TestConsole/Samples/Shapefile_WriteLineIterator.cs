using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public class Shapefile_WriteLineIterator : Test
    {
        public override void Run()
        {
            var features = new List<Feature>();
            for (int i = 1; i < 5; i++)
            {
                var lineCoords = new List<CoordinateZ>();
                lineCoords.Add(new CoordinateZ(i, i + 1, i));
                lineCoords.Add(new CoordinateZ(i, i, i));
                lineCoords.Add(new CoordinateZ(i + 1, i, i));
                var line = new LineString(lineCoords.ToArray());
                var mline = new MultiLineString(new LineString[] { line });

                var attributes = new AttributesTable();
                attributes.Add("date", new DateTime(2000, 1, i + 1));
                attributes.Add("float", i * 0.1);
                attributes.Add("int", i);
                attributes.Add("logical", i % 2 == 0);
                attributes.Add("text", i.ToString("0.00"));

                var feature = new Feature(mline, attributes);
                features.Add(feature);
            }

            var shpPath = GetTempFilePath("abcd3.shp");
            Shapefile.WriteAllFeatures(features, shpPath);
            Console.WriteLine($"{features.Count} features have been written.");
        }
    }

}
