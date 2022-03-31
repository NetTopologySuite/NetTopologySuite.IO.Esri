
using System;

namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public class Shapefile_ReadPointCore : Test
    {
        public override void Run()
        {
            var shpPath = GetTestFilePath("arcmap/shp/pt_utf8.shp");

            using (var shp = Shapefile.OpenRead(shpPath))
            {
                PrintFieldValue("SHAPE", shp.ShapeType);
                PrintFieldNames(shp.Fields);

                while (shp.Read(out var deleted))
                {
                    if (deleted)
                        continue;

                    Console.WriteLine("Record ID: " + shp.Fields["Id"].Value);
                    foreach (var field in shp.Fields)
                    {
                        PrintFieldValue(field.Name, field.Value);
                    }
                    PrintFieldValue("SHAPE", shp.Geometry);
                    Console.WriteLine();
                }
            }
        }
    }
}
