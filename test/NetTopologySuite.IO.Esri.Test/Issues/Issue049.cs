using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test.Issues;

/// <summary>
/// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/49
/// </summary>
internal class Issue049
{
    [Test]
    public void Decimal0_OpenShapefile()
    {
        var shpPath = TestShapefiles.PathTo("Issues/049/221-1_28.03.202411-16.shp");
        var options = new ShapefileReaderOptions
        {
            GeometryBuilderMode = GeometryBuilderMode.FixInvalidShapes
        };
        using var shpReader = Shapefile.OpenRead(shpPath, options);

        Assert.AreEqual(shpReader.ShapeType, ShapeType.Polygon);

        while (shpReader.Read())
        {
            var dosis = shpReader.Fields["DOSIS"].Value.ToString();
            Assert.IsNotEmpty(dosis);
            Assert.IsFalse(shpReader.Geometry.IsEmpty);
        }
    }
}
