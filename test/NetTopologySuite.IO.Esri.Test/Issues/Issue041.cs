using NUnit.Framework;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test.Issues;

/// <summary>
/// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/41
/// </summary>
internal class Issue041
{
    [Test]
    public void OpenRead_StreamPt()
    {
        var shpPath = TestShapefiles.PathToCountriesPt();
        var dbfPath = TestShapefiles.PathToCountriesPt(".dbf");

        using var shpStream = File.OpenRead(shpPath);
        using var dbfStream = File.OpenRead(dbfPath);
        using var shpReader = Shapefile.OpenRead(shpStream, dbfStream);

        Assert.AreEqual(shpReader.ShapeType, ShapeType.Point);

        while (shpReader.Read())
        {
            var countryName = shpReader.Fields["CNTR_NAME"].Value.ToString();
            Assert.IsNotEmpty(countryName);
            Assert.IsFalse(shpReader.Geometry.IsEmpty);
        }
    }

    [Test]
    public void OpenRead_StreamLn()
    {
        var shpPath = TestShapefiles.PathToCountriesLn();
        var dbfPath = TestShapefiles.PathToCountriesLn(".dbf");

        using var shpStream = File.OpenRead(shpPath);
        using var dbfStream = File.OpenRead(dbfPath);
        using var shpReader = Shapefile.OpenRead(shpStream, dbfStream);

        Assert.AreEqual(shpReader.ShapeType, ShapeType.PolyLine);

        while (shpReader.Read())
        {
            Assert.IsFalse(shpReader.Geometry.IsEmpty);
        }
    }

    [Test]
    public void OpenRead_StreamPg()
    {
        var shpPath = TestShapefiles.PathToCountriesPg();
        var dbfPath = TestShapefiles.PathToCountriesPg(".dbf");

        using var shpStream = File.OpenRead(shpPath);
        using var dbfStream = File.OpenRead(dbfPath);
        using var shpReader = Shapefile.OpenRead(shpStream, dbfStream);

        Assert.AreEqual(shpReader.ShapeType, ShapeType.Polygon);

        while (shpReader.Read())
        {
            var countryName = shpReader.Fields["CNTR_NAME"].Value.ToString();
            Assert.IsNotEmpty(countryName);
            Assert.IsFalse(shpReader.Geometry.IsEmpty);
        }
    }


    [Test]
    public void ReadAllFeatures_StreamPt()
    {
        var shpPath = TestShapefiles.PathToCountriesPt();
        var dbfPath = TestShapefiles.PathToCountriesPt(".dbf");

        using var shpStream = File.OpenRead(shpPath);
        using var dbfStream = File.OpenRead(dbfPath);
        var features = Shapefile.ReadAllFeatures(shpStream, dbfStream);

        foreach (var feature in features)
        {
            var countryName = feature.Attributes["CNTR_NAME"].ToString();
            Assert.IsNotEmpty(countryName);
            Assert.IsFalse(feature.Geometry.IsEmpty);
        }
    }

    [Test]
    public void ReadAllFeatures_StreamLn()
    {
        var shpPath = TestShapefiles.PathToCountriesLn();
        var dbfPath = TestShapefiles.PathToCountriesLn(".dbf");

        using var shpStream = File.OpenRead(shpPath);
        using var dbfStream = File.OpenRead(dbfPath);
        var features = Shapefile.ReadAllFeatures(shpStream, dbfStream);

        foreach (var feature in features)
        {
            Assert.IsFalse(feature.Geometry.IsEmpty);
        }
    }

    [Test]
    public void ReadAllFeatures_StreamPg()
    {
        var shpPath = TestShapefiles.PathToCountriesPg();
        var dbfPath = TestShapefiles.PathToCountriesPg(".dbf");

        using var shpStream = File.OpenRead(shpPath);
        using var dbfStream = File.OpenRead(dbfPath);
        var features = Shapefile.ReadAllFeatures(shpStream, dbfStream);

        foreach (var feature in features)
        {
            var countryName = feature.Attributes["CNTR_NAME"].ToString();
            Assert.IsNotEmpty(countryName);
            Assert.IsFalse(feature.Geometry.IsEmpty);
        }
    }
}
