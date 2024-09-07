using NetTopologySuite.Geometries;
using NUnit.Framework;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace NetTopologySuite.IO.Esri.Test.Issues;

/// <summary>
/// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/56
/// </summary>
internal class Issue056
{
    [Test]
    public void ReadShpProjection()
    {
        //var shpPath = TestShapefiles.PathTo("D:\\GIS\\NTS\\cs_1992_projection.shp");
        var shpPath = TestShapefiles.PathTo("Issues/039/Line/Line.shp");
        using var shpReader = Shapefile.OpenRead(shpPath);

        var csFactory = new CoordinateSystemFactory();
        var sourceCs = csFactory.CreateFromWkt(shpReader.Projection);
        var targetCs = GeographicCoordinateSystem.WGS84;
        var targetCsEnvelope = GetWgs84Envelope();

        Assert.IsTrue(sourceCs is ProjectedCoordinateSystem);
        Assert.IsTrue(targetCs is GeographicCoordinateSystem);

        var transformationFactory = new CoordinateTransformationFactory();
        var transformation = transformationFactory.CreateFromCoordinateSystems(sourceCs, targetCs);        

        foreach (var feature in shpReader)
        {
            var sourcePoint = feature.Geometry.Centroid;
            var sourceCoords = new double[] { sourcePoint.X, sourcePoint.Y };
            var tergetCoords = transformation.MathTransform.Transform(sourceCoords);
            var targetPoint = new Point(tergetCoords[0], tergetCoords[1]);

            Assert.IsTrue(targetCsEnvelope.Contains(targetPoint));
        }
    }

    private Geometry GetWgs84Envelope()
    {
        double minX = -90.0;
        double minY = -180.0;
        double maxX = 90.0;
        double maxY = 180.0;

        var coordinates = new Coordinate[]
        {
            new(minX, minY), // Bottom-left
            new(minX, maxY), // Top-left
            new(maxX, maxY), // Top-right
            new(maxX, minY), // Bottom-right
            new(minX, minY)  // Closing the polygon
        };

        var linearRing = new LinearRing(coordinates);
        return new Polygon(linearRing);
    }
}
