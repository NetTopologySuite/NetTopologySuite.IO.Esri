using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;
using System;

namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public class Shapefile_WritePointCore : Test
    {
        public override void Run()
        {
            var shpPath = GetTempFilePath("point.shp");


            var options = new ShapefileWriterOptions(ShapeType.Point);
            var dateField = options.AddDateField("date");
            var floatField = options.AddFloatField("float");
            var intField = options.AddNumericInt32Field("int");
            var logicalField = options.AddLogicalField("logical");
            var textField = options.AddCharacterField("text");

            using (var shp = new ShapefilePointWriter(shpPath, options))
            {
                for (int i = 1; i < 5; i++)
                {
                    dateField.DateValue = new DateTime(2000, 1, i + 1);
                    floatField.NumericValue = i * 0.1;
                    intField.NumericValue = i;
                    logicalField.LogicalValue = i % 2 == 0;
                    textField.StringValue = i.ToString("0.00");

                    var coordinates = new CoordinateZ(i, i + 1, i + 2);
                    shp.Geometry = new Point(coordinates);

                    shp.Write();
                    Console.WriteLine("Record number " + i + " have been written.");
                }
                Console.WriteLine();
            }

            using (var shp = Shapefile.OpenRead(shpPath))
            {
                PrintFeatures(shp);
            }
        }
    }
}
