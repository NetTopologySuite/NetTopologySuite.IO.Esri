using NetTopologySuite.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries.Implementation;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite.IO.ShapeFile.Test
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
            var shpWriter = new ShapefileDataWriter(fileName, features[0].Geometry.Factory)
            {
                Header = ShapefileDataWriter.GetHeader(features[0], features.Count)
            };
            shpWriter.Write(features);

            bool isTrue;
            using (var reader = new ShapefileDataReader(fileName, pt.Factory))
                @isTrue = reader.ShapeHeader.ShapeType.ToString() == "Point";

            foreach (string file in Directory.GetFiles(Path.GetTempPath(), Path.GetFileName(fileName) + ".*"))
            {
                File.Delete(file);
            }

            Assert.IsTrue(@isTrue);
        }
    }
}
