using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test.Issues;

/// <summary>
/// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/39
/// </summary>
internal class Issue039
{
    [Test]
    public void ReadCharacterFieldWithLength255()
    {
        string shpPath = TestShapefiles.PathTo(@"Issues/039/Line/Line.shp");
        var features = Shapefile.ReadAllFeatures(shpPath);
        Assert.IsNotNull(features);
    }
}
