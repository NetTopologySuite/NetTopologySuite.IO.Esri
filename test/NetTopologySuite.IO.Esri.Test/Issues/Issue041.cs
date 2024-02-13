using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

    [Test]
    public void OpenWrite_StreamPt()
    {
        var nameField = new DbfCharacterField("name");
        var options = new ShapefileWriterOptions(ShapeType.Point, nameField);

        var shpPath = TestShapefiles.GetTempShpPath();
        var shxPath = Path.ChangeExtension(shpPath, ".shx"); 
        var dbfPath = Path.ChangeExtension(shpPath, ".dbf");

        using (var shpStream = File.OpenWrite(shpPath))
        using (var shxStream = File.OpenWrite(shxPath))
        using (var dbfStream = File.OpenWrite(dbfPath))
        using (var shpWriter = Shapefile.OpenWrite(shpStream, shxStream, dbfStream, options))
        {
            for (int i = 1; i <= 10; i++)
            {
                shpWriter.Geometry = new Point(i, i);
                shpWriter.Fields["name"].Value = $"Geom{i}";
                shpWriter.Write();
            }
        }

        var features = Shapefile.ReadAllFeatures(shpPath);
        Assert.AreEqual(features.Length, 10);

        var firstFeature = features[0];

        var firstName = firstFeature.Attributes["name"].ToString();
        Assert.AreEqual(firstName, "Geom1");

        var firstGeom = firstFeature.Geometry as Point;
        Assert.AreEqual(1.0, firstGeom.X);
        Assert.AreEqual(1.0, firstGeom.Y);
    }

    [Test]
    public void OpenWrite_StreamLn()
    {
        var nameField = new DbfCharacterField("name");
        var options = new ShapefileWriterOptions(ShapeType.PolyLine, nameField);

        var shpPath = TestShapefiles.GetTempShpPath();
        var shxPath = Path.ChangeExtension(shpPath, ".shx");
        var dbfPath = Path.ChangeExtension(shpPath, ".dbf");

        using (var shpStream = File.OpenWrite(shpPath))
        using (var shxStream = File.OpenWrite(shxPath))
        using (var dbfStream = File.OpenWrite(dbfPath))
        using (var shpWriter = Shapefile.OpenWrite(shpStream, shxStream, dbfStream, options))
        {
            for (int i = 1; i <= 10; i++)
            {
                var points = new[] { new Coordinate(i, i), new Coordinate(i + 1, i + 1) };
                var lineStrings = new[] { new LineString(points) };
                shpWriter.Geometry = new MultiLineString(lineStrings);
                shpWriter.Fields["name"].Value = $"Geom{i}";
                shpWriter.Write();
            }
        }

        var features = Shapefile.ReadAllFeatures(shpPath);
        Assert.AreEqual(features.Length, 10);

        var firstFeature = features[0];

        var firstName = firstFeature.Attributes["name"].ToString();
        Assert.AreEqual(firstName, "Geom1");

        var firstGeom = firstFeature.Geometry as MultiLineString;
        Assert.AreEqual(firstGeom.Length, Math.Sqrt(1.0 + 1.0));
    }

    [Test]
    public void OpenWrite_StreamPg()
    {
        var nameField = new DbfCharacterField("name");
        var options = new ShapefileWriterOptions(ShapeType.Polygon, nameField);

        var shpPath = TestShapefiles.GetTempShpPath();
        var shxPath = Path.ChangeExtension(shpPath, ".shx");
        var dbfPath = Path.ChangeExtension(shpPath, ".dbf");

        using (var shpStream = File.OpenWrite(shpPath))
        using (var shxStream = File.OpenWrite(shxPath))
        using (var dbfStream = File.OpenWrite(dbfPath))
        using (var shpWriter = Shapefile.OpenWrite(shpStream, shxStream, dbfStream, options))
        {
            for (int i = 1; i <= 10; i++)
            {
                var points = new[] { new Coordinate(i, i), new Coordinate(i + 1, i), new Coordinate(i, i + 1), new Coordinate(i, i) };
                var shell = new LinearRing(points);
                var polygons = new[] { new Polygon(shell) };
                shpWriter.Geometry = new MultiPolygon(polygons);
                shpWriter.Fields["name"].Value = $"Geom{i}";
                shpWriter.Write();
            }
        }

        var features = Shapefile.ReadAllFeatures(shpPath);
        Assert.AreEqual(features.Length, 10);

        var firstFeature = features[0];

        var firstName = firstFeature.Attributes["name"].ToString();
        Assert.AreEqual(firstName, "Geom1");

        var firstGeom = firstFeature.Geometry as MultiPolygon;
        Assert.AreEqual(firstGeom.Length, 1.0 + 1.0 + Math.Sqrt(1.0 + 1.0));
    }


    [Test]
    public void WriteAllFeatures_StreamPt()
    {
        var nameField = new DbfCharacterField("name");

        var shpPath = TestShapefiles.GetTempShpPath();
        var shxPath = Path.ChangeExtension(shpPath, ".shx");
        var dbfPath = Path.ChangeExtension(shpPath, ".dbf");

        var features = new List<Feature>();
        for (int i = 1; i <= 10; i++)
        {
            var feature = new Feature(new Point(i, i), new AttributesTable { { "name", $"Geom{i}" } });
            features.Add(feature);
        }


        using (var shpStream = File.OpenWrite(shpPath))
        using (var shxStream = File.OpenWrite(shxPath))
        using (var dbfStream = File.OpenWrite(dbfPath))
        {
            Shapefile.WriteAllFeatures(features, shpStream, shxStream, dbfStream);
        }

        features = Shapefile.ReadAllFeatures(shpPath).ToList();
        Assert.AreEqual(features.Count, 10);

        var firstFeature = features[0];

        var firstName = firstFeature.Attributes["name"].ToString();
        Assert.AreEqual(firstName, "Geom1");

        var firstGeom = firstFeature.Geometry as Point;
        Assert.AreEqual(1.0, firstGeom.X);
        Assert.AreEqual(1.0, firstGeom.Y);
    }

    [Test]
    public void WriteAllFeatures_StreamLn()
    {
        var nameField = new DbfCharacterField("name");

        var shpPath = TestShapefiles.GetTempShpPath();
        var shxPath = Path.ChangeExtension(shpPath, ".shx");
        var dbfPath = Path.ChangeExtension(shpPath, ".dbf");

        var features = new List<Feature>();
        for (int i = 1; i <= 10; i++)
        {
            var points = new[] { new Coordinate(i, i), new Coordinate(i + 1, i + 1) };
            var lineStrings = new[] { new LineString(points) };
            var feature = new Feature(new MultiLineString(lineStrings), new AttributesTable { { "name", $"Geom{i}" } });
            features.Add(feature);
        }

        using (var shpStream = File.OpenWrite(shpPath))
        using (var shxStream = File.OpenWrite(shxPath))
        using (var dbfStream = File.OpenWrite(dbfPath))
        {
            Shapefile.WriteAllFeatures(features, shpStream, shxStream, dbfStream);
        }

        features = Shapefile.ReadAllFeatures(shpPath).ToList();
        Assert.AreEqual(features.Count, 10);

        var firstFeature = features[0];

        var firstName = firstFeature.Attributes["name"].ToString();
        Assert.AreEqual(firstName, "Geom1");

        var firstGeom = firstFeature.Geometry as MultiLineString;
        Assert.AreEqual(firstGeom.Length, Math.Sqrt(1.0 + 1.0));
    }

    [Test]
    public void WriteAllFeatures_StreamPg()
    {
        var nameField = new DbfCharacterField("name");

        var shpPath = TestShapefiles.GetTempShpPath();
        var shxPath = Path.ChangeExtension(shpPath, ".shx");
        var dbfPath = Path.ChangeExtension(shpPath, ".dbf");

        var features = new List<Feature>();
        for (int i = 1; i <= 10; i++)
        {
            var points = new[] { new Coordinate(i, i), new Coordinate(i + 1, i), new Coordinate(i, i + 1), new Coordinate(i, i) };
            var shell = new LinearRing(points);
            var polygons = new[] { new Polygon(shell) };
            var feature = new Feature(new MultiPolygon(polygons), new AttributesTable { { "name", $"Geom{i}" } });
            features.Add(feature);
        }

        using (var shpStream = File.OpenWrite(shpPath))
        using (var shxStream = File.OpenWrite(shxPath))
        using (var dbfStream = File.OpenWrite(dbfPath))
        {
            Shapefile.WriteAllFeatures(features, shpStream, shxStream, dbfStream);
        }

        features = Shapefile.ReadAllFeatures(shpPath).ToList();
        Assert.AreEqual(features.Count, 10);

        var firstFeature = features[0];

        var firstName = firstFeature.Attributes["name"].ToString();
        Assert.AreEqual(firstName, "Geom1");

        var firstGeom = firstFeature.Geometry as MultiPolygon;
        Assert.AreEqual(firstGeom.Length, 1.0 + 1.0 + Math.Sqrt(1.0 + 1.0));
    }
}
