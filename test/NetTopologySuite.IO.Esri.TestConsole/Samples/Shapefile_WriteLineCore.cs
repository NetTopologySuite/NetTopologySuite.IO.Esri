using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;
using System;

namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public class Shapefile_WriteLineCore : Test
    {
        public override void Run()
        {
            var shpPath = GetTempFilePath("abcd2.shp");

            var options = new ShapefileWriterOptions(ShapeType.PolyLine);
            var dateField = options.AddDateField("date");
            var floatField = options.AddFloatField("float");
            var intField = options.AddNumericInt32Field("int");
            var LogicalField = options.AddLogicalField("logical");
            var textField = options.AddCharacterField("text");

            var pointsBuffer = new CoordinateSequenceBuffer();
            var lineBuffer = GeometryFactory.Default.CreateLineString(pointsBuffer);
            var multiLineBuffer = GeometryFactory.Default.CreateMultiLineString(new LineString[] { lineBuffer });

            using (var shp = new ShapefilePolyLineWriter(shpPath, options))
            {
                for (int i = 1; i < 5; i++)
                {
                    // Avoid expensive boxing and unboxing value types (mind thath attribute values are set directly trough DbfField definition)
                    dateField.DateValue = new DateTime(2000, 1, i + 1);
                    floatField.NumericValue = i * 0.1;
                    intField.NumericValue = i;
                    LogicalField.LogicalValue = i % 2 == 0;
                    textField.StringValue = i.ToString("0.00");

                    // Avoid realocating new LineString and Coordinate[] array over and over.
                    pointsBuffer.Clear();
                    pointsBuffer.AddXyz(i, i + 1, i);
                    pointsBuffer.AddXyz(i, i, i);
                    pointsBuffer.AddXyz(i + 1, i, i);
                    multiLineBuffer.GeometryChanged();
                    lineBuffer.GeometryChanged(); // ???

                    shp.Geometry = multiLineBuffer;

                    shp.Write();
                    Console.WriteLine("Feature number " + i + " have been written.");
                }
            }

            foreach (var feature in Shapefile.ReadAllFeatures(shpPath))
            {
                PrintFeature(feature);
            }
        }
    }
}
