using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test.Issues;

/// <summary>
/// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/63
/// </summary>
internal class Issue063
{
    [Test]
    public void ShapefileResetTest()
    {
        var shpPath = TestShapefiles.PathTo("nested_polygons.shp"); 
        using var shpReader = Shapefile.OpenRead(shpPath);

        // This test need a shapefile with one feature (RecordCount == 1)
        Assert.That(shpReader.RecordCount, Is.EqualTo(1));

        var readOk = shpReader.Read();
        Assert.IsTrue(readOk);
        shpReader.Restart();
        readOk = shpReader.Read();
        Assert.IsTrue(readOk);
    }
}
