
using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using System;

namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public class Shapefile_ReadPointFeatures : Test
    {
        public override void Run()
        {
            var shpPath = GetTestFilePath("arcmap/shp/pt_utf8.shp");

            using (var shp = new ShapefilePointReader(shpPath))
            {
                PrintFieldValue("SHAPE", shp.ShapeType);
                PrintFieldNames(shp.Fields);


                while (shp.Read(out var deleted, out var feature))
                {
                    if (deleted)
                        continue;

                    Console.WriteLine("Record ID: " + feature.Attributes["Id"]);
                    foreach (var name in feature.Attributes.GetNames())
                    {
                        var val = feature.Attributes[name];
                        PrintFieldValue(name, val);
                    }
                    Console.WriteLine($"  SHAPE: {feature.Geometry}");
                    Console.WriteLine();
                }
            }
        }
    }
}
