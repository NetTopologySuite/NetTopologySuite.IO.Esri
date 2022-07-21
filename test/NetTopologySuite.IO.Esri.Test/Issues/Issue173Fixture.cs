using NetTopologySuite.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries.Implementation;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    [NtsIssueNumber(173)]
    public class Issue173Fixture
    {
        [Test, Description("The NetTopologySuite.IO.GeoTools class method ShapeFile.GetGeometryType(Geometry geom) will always returns ShapeGeometryType.PointZM making all shapefile geometry GeometryZM.")]
        public void Test()
        {
            var features = new List<IFeature>();
            var seq = DotSpatialAffineCoordinateSequenceFactory.Instance.Create(1, Ordinates.XY);
            seq.SetOrdinate(0, Ordinate.X, -91.0454);
            seq.SetOrdinate(0, Ordinate.Y, 32.5907);
            var pt = new GeometryFactory(DotSpatialAffineCoordinateSequenceFactory.Instance).CreatePoint(seq);
            var attr = new AttributesTable();
            attr.Add("FirstName", "John");
            attr.Add("LastName", "Doe");
            features.Add(new Feature(pt, attr));

            string fileName = Path.GetTempFileName();
            fileName = fileName.Substring(0, fileName.Length - 4);
            Shapefile.WriteAllFeatures(features, fileName);

            bool isTrue = true;
            using (var reader = Shapefile.OpenRead(fileName))
                isTrue = isTrue && reader.ShapeType.ToString() == "Point";

            foreach (string file in Directory.GetFiles(Path.GetTempPath(), Path.GetFileName(fileName) + ".*"))
            {
                File.Delete(file);
            }

            Assert.IsTrue(@isTrue);
        }
    }
}
