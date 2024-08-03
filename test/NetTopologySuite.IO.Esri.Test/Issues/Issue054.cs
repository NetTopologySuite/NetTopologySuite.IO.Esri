using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;
using NUnit.Framework;
using System.Collections.Generic;

namespace NetTopologySuite.IO.Esri.Test.Issues;

/// <summary>
/// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/54
/// </summary>
internal class Issue054
{
    [Test]
    public void CreateShapefile_LineString()
    {
        CreateAndTestShapefile(ShapeType.PolyLine, SampleGeometry.SampleLineString);
    }

    [Test]
    public void CreateShapefile_MultiLineString()
    {
        CreateAndTestShapefile(ShapeType.PolyLine, SampleGeometry.SampleMultiLineString);
    }

    [Test]
    public void CreateShapefile_Polygon()
    {
        CreateAndTestShapefile(ShapeType.Polygon, SampleGeometry.SamplePolygon);
    }

    [Test]
    public void CreateShapefile_MultiPolygon()
    {
        CreateAndTestShapefile(ShapeType.Polygon, SampleGeometry.SampleMultiPolygon);
    }

    private static void CreateAndTestShapefile(ShapeType shapeType, Geometry geometry)
    {
        var shpPath = TestShapefiles.GetTempShpPath();
        var fields = new List<DbfField>();
        var fidField = fields.AddNumericInt32Field("fid");

        var options = new ShapefileWriterOptions(shapeType, fields.ToArray());
        using (var shpWriter = Shapefile.OpenWrite(shpPath, options))
        {
            shpWriter.Geometry = geometry;
            fidField.NumericValue = 1;
            shpWriter.Write();
        }

        var features = Shapefile.ReadAllFeatures(shpPath);
        var storedGeometry = features[0].Geometry;
        Assert.AreEqual(storedGeometry.Area, geometry.Area, 1.0e-9);
        Assert.AreEqual(storedGeometry.Length, geometry.Length,1.0e-9);
        Assert.AreEqual(storedGeometry.Centroid, geometry.Centroid);

        TestShapefiles.DeleteShp(shpPath);
    }
}
