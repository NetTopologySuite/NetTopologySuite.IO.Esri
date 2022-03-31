using System;

namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public class Shapefile_ReadPointIterator : Test
    {
        public override void Run()
        {
            var shpPath = GetTestFilePath("arcmap/shp/pt_utf8.shp");

            foreach (var feature in Shapefile.ReadAllFeatures(shpPath))
            {
                Console.WriteLine(" Record ID: " + feature.Attributes["Id"]);
                foreach (var attrName in feature.Attributes.GetNames())
                {
                    Console.WriteLine($"{attrName,10}: {feature.Attributes[attrName]}");
                }
                Console.WriteLine($"     SHAPE: {feature.Geometry}");
                Console.WriteLine();
            }
        }
    }
}
