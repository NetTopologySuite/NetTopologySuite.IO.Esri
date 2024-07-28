using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Test.Issues;

/// <summary>
/// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/53
/// </summary>
internal class Issue053
{
    [Test]
    public void Projection_Utf8_BOM()
    {
        var fields = new List<DbfField>();
        var fidField = fields.AddNumericInt32Field("fid");
        var options = new ShapefileWriterOptions(ShapeType.Polygon, fields.ToArray())
        {
            Projection = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137.0,298.257223563]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]]"
        };

        var shpPath = TestShapefiles.GetTempShpPath();
        using (var shpWriter = Shapefile.OpenWrite(shpPath, options))
        {
            shpWriter.Geometry = SampleGeometry.SampleMultiPolygon;
            fidField.NumericValue = 1;
            shpWriter.Write();
        }

        var expectedProjectionString = options.Projection;
        var expectedProjectionBytes = options.Encoding.GetBytes(options.Projection);

        var prjPath = Path.ChangeExtension(shpPath, ".prj");
        var storedProjectionString = File.ReadAllText(prjPath);
        var storedProjectionBytes = File.ReadAllBytes(prjPath);

        TestShapefiles.DeleteShp(shpPath);

        Assert.AreEqual(expectedProjectionString, storedProjectionString);
        Assert.AreEqual(expectedProjectionBytes, storedProjectionBytes);
    }

    [Test]
    public static void Utf8_BOM_Default()
    {
        var encoding = Encoding.UTF8;
        var filePath = Path.GetTempFileName();
        var expectedString = "abc";
        var expectedBytes = encoding.GetBytes(expectedString);
        WriteFile(filePath, expectedString, encoding);

        var storedString = File.ReadAllText(filePath, encoding);
        var storedBytes = File.ReadAllBytes(filePath);

        Assert.AreEqual(expectedString, storedString); // C# is cleaver enough to ignore BOM when reading
        Assert.AreNotEqual(expectedBytes, storedBytes); // Not equal because of BOM stored by default
    }

    [Test]
    public static void Utf8_BOM_Included()
    {
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        var filePath = Path.GetTempFileName();
        var expectedString = "abc";
        var expectedBytes = encoding.GetBytes(expectedString);
        WriteFile(filePath, expectedString, encoding);

        var storedString = File.ReadAllText(filePath, encoding);
        var storedBytes = File.ReadAllBytes(filePath);

        Assert.AreEqual(expectedString, storedString); // C# is cleaver enough to ignore BOM when reading
        Assert.AreNotEqual(expectedBytes, storedBytes); // Not equal because of BOM stored explicitly
    }

    [Test]
    public static void Utf8_BOM_Excluded()
    {
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        var filePath = Path.GetTempFileName();
        var expectedString = "abc";
        var expectedBytes = encoding.GetBytes(expectedString);
        WriteFile(filePath, expectedString, encoding);

        var storedString = File.ReadAllText(filePath, encoding);
        var storedBytes = File.ReadAllBytes(filePath);

        Assert.AreEqual(expectedString, storedString);
        Assert.AreEqual(expectedBytes, storedBytes); 
    }

    private static void WriteFile(string filePath, string content, Encoding encoding)
    {
        using (StreamWriter writer = new StreamWriter(filePath, false, encoding))
        {
            writer.Write(content);
        }
    }
}
