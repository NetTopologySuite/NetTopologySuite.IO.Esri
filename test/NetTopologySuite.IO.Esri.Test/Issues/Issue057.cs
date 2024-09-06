using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test.Issues;

/// <summary>
/// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/57
/// </summary>
internal class Issue057
{
    [Test]
    public void Decimal0_OpenShapefile()
    {
        var shpPath = TestShapefiles.PathTo("Issues/057/deter-nf-deter-public.shp");
        var options = new ShapefileReaderOptions
        {
            GeometryBuilderMode = GeometryBuilderMode.FixInvalidShapes
        };
        using var shpReader = Shapefile.OpenRead(shpPath, options);

        Assert.AreEqual(shpReader.ShapeType, ShapeType.Polygon);

        while (shpReader.Read())
        {
            var dosis = shpReader.Fields["AREA_KM"].Value.ToString();
            Assert.IsNotEmpty(dosis);
            Assert.IsFalse(shpReader.Geometry.IsEmpty);
        }
    }
}
