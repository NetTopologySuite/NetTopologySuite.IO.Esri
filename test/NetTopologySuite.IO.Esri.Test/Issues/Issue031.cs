using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NetTopologySuite.IO.Esri.Test.Issues;

/// <summary>
/// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/31
/// </summary>
internal class Issue031
{

    [Test]
    public void CreateShp_NullInt_Auto()
    {
        var features = new List<Feature>();
        for (var i = 1; i < 5; i++)
        {
            var lineCoords = new List<Coordinate>
            {
                new(i, i + 1),
                new(i, i),
                new(i + 1, i)
            };
            var line = new LineString(lineCoords.ToArray());
            var mline = new MultiLineString(new LineString[] { line });

            // When all features have a null value, the field type cannot be detected correctly.
            int? nullableIntValue = i % 3 == 0 ? 1 : null;

            var attributes = new AttributesTable
            {
                { "date", new DateTime() },
                { "float", i * 0.1 },
                { "int", nullableIntValue },
                { "logical", i % 2 == 0 },
                { "text", i.ToString("0.00") }
            };
            

            var feature = new Feature(mline, attributes);
            features.Add(feature);
        }

        var shpPath = TestShapefiles.GetTempShpPath();
        Shapefile.WriteAllFeatures(features, shpPath);
        TestShapefiles.DeleteShp(shpPath);
    }

    [Test]
    public void CreateShp_NullInt_Auto_AllNullError()
    {
        var features = new List<Feature>();
        for (var i = 1; i < 5; i++)
        {
            var lineCoords = new List<Coordinate>
            {
                new(i, i + 1),
                new(i, i),
                new(i + 1, i)
            };
            var line = new LineString(lineCoords.ToArray());
            var mline = new MultiLineString(new LineString[] { line });

            int? nullableIntValue = null;

            var attributes = new AttributesTable
            {
                { "date", new DateTime() },
                { "float", i * 0.1 },
                { "int", nullableIntValue },
                { "logical", i % 2 == 0 },
                { "text", i.ToString("0.00") }
            };


            var feature = new Feature(mline, attributes);
            features.Add(feature);
        }

        var shpPath = TestShapefiles.GetTempShpPath();

        // When all features have a null value, the field type cannot be detected correctly.
        // To solve this, the `AttributesTable` needs to be extended to store attribute types along with attribute values,
        // so that `AttributesTable.GetType()` returns propert attribute type instead of default `typeof(object)`.
        // Se also: https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/31#issuecomment-1975112219
        var exception = Assert.Throws<ShapefileException>(() => Shapefile.WriteAllFeatures(features, shpPath));

        TestShapefiles.DeleteShp(shpPath);
        Console.WriteLine(exception.Message);
    }

    [Test]
    public void CreateShp_NullInt_Manual_AttributeTable()
    {
        var features = new List<Feature>();
        for (var i = 1; i < 5; i++)
        {
            var lineCoords = new List<Coordinate>
            {
                new(i, i + 1),
                new(i, i),
                new(i + 1, i)
            };
            var line = new LineString(lineCoords.ToArray());
            var mline = new MultiLineString(new LineString[] { line });

            int? nullableIntValue = null;

            var attributes = new AttributesTable
            {
                { "date", new DateTime() },
                { "float", i * 0.1 },
                { "int", nullableIntValue },
                { "logical", i % 2 == 0 },
                { "text", i.ToString("0.00") }
            };  

            var feature = new Feature(mline, attributes);
            features.Add(feature);
        }

        var fields = new List<DbfField>();
        fields.AddDateField("date");
        fields.AddFloatField("float");
        fields.AddNumericInt32Field("int");
        fields.AddLogicalField("logical");
        fields.AddCharacterField("text");

        var options = new ShapefileWriterOptions(ShapeType.PolyLine, fields.ToArray());
        var shpPath = TestShapefiles.GetTempShpPath();
        using (var shpWriter = Shapefile.OpenWrite(shpPath, options))
        {
            shpWriter.Write(features);
        }
        TestShapefiles.DeleteShp(shpPath);
    }

    [Test]
    public void CreateShp_NullInt_Manual_ShpWriter()
    {
        var fields = new List<DbfField>();
        var dateField = fields.AddDateField("date");
        var floatField = fields.AddFloatField("float");
        var intField = fields.AddNumericInt32Field("int");
        var logicalField = fields.AddLogicalField("logical");
        var textField = fields.AddCharacterField("text");

        var options = new ShapefileWriterOptions(ShapeType.PolyLine, fields.ToArray());
        var shpPath = TestShapefiles.GetTempShpPath();
        using (var shpWriter = Shapefile.OpenWrite(shpPath, options))
        {
            for (var i = 1; i < 5; i++)
            {
                var lineCoords = new List<Coordinate>
                {
                    new(i, i + 1),
                    new(i, i),
                    new(i + 1, i)
                };
                var line = new LineString(lineCoords.ToArray());
                var mline = new MultiLineString(new LineString[] { line });

                int? nullableIntValue = null;

                shpWriter.Geometry = mline;
                dateField.DateValue = DateTime.Now;
                floatField.NumericValue = i * 0.1;
                intField.NumericValue = nullableIntValue;
                logicalField.LogicalValue = i % 2 == 0;
                textField.StringValue = i.ToString("0.00");
                shpWriter.Write();
            }
        }
        TestShapefiles.DeleteShp(shpPath);
    }
}
