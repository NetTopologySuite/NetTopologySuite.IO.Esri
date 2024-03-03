using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test.Issues;

/// <summary>
/// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/42
/// </summary>
internal class Issue042
{
    private const long GB = 1024 * 1024 * 1024;
    private const long MB = 1024 * 1024;

    [Test]
    public void CreateShapefile_1MB_Success()
    {
        string shpPath = TestShapefiles.GetTempShpPath();
        (long shpSize, int featureCount) = CreateShapefile(1 * MB, shpPath);
        Console.WriteLine($"Feature count: {featureCount}");
        Console.WriteLine($"SHP size:      {shpSize}");
        TestShapefiles.DeleteShp(shpPath);
    }

    [Test]
    public void CreateShapefile_2GB_Error()
    {
        string shpPath = @"D:\Temp\CreateShapefile_2GB_Error.shp";
        if (Directory.Exists(Path.GetDirectoryName(shpPath))) // Do not create 2GB files during pipeline tests.
        {
            long geometrySize = 399;
            long minShpSize = 2 * GB + geometrySize;
            Assert.Throws<ShapefileException>(() => CreateShapefile(minShpSize, shpPath));
        }
    }

    [Test]
    public void CreateShapefile_2GB_Success()
    {
        string shpPath = @"D:\Temp\CreateShapefile_2GB_Success.shp";
        if (Directory.Exists(Path.GetDirectoryName(shpPath))) // Do not create 2GB files during pipeline tests.
        {
            long geometrySize = 399;
            long minShpSize = 2 * GB - geometrySize;
            (long shpSize, int featureCount) = CreateShapefile(minShpSize, shpPath);
            Console.WriteLine($"Feature count: {featureCount}");
            Console.WriteLine($"SHP size:      {shpSize}");
        }
    }

    private static (long shpSize, int featureCount) CreateShapefile(long minSzie, string shpPath)
    {
        var fields = new List<DbfField>();
        var fidField = fields.AddNumericInt32Field("fid");
        var fid = 1;

        var options = new ShapefileWriterOptions(ShapeType.Polygon, fields.ToArray());

        using var shpStream = File.OpenWrite(shpPath);
        using var shxStream = File.OpenWrite(Path.ChangeExtension(shpPath, ".shx"));
        using var dbfStream = File.OpenWrite(Path.ChangeExtension(shpPath, ".dbf"));
        using var shpWriter = Shapefile.OpenWrite(shpStream, shxStream, dbfStream, null, options);

        while (shpStream.Length < minSzie)
        {
            shpWriter.Geometry = SampleGeometry.SampleMultiPolygonWithHole;
            fidField.NumericValue = fid++;
            shpWriter.Write();
        }

        return (shpStream.Length, fid - 1);
    }
}
