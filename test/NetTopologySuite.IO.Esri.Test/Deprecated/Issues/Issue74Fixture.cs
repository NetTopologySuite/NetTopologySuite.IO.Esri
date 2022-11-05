using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shp.Readers;
using NetTopologySuite.IO.Esri.Shp.Writers;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.Issues
{
    [NtsIssueNumber(74)]
    public class Issue74Fixture
    {
        [Test, Description("Max 'M' value is always set to double.PositiveInfinity")]
        public void ShapeHandler_correctly_writes_BBOX_info()
        {
            var factory = GeometryFactory.Default;
            var pMin = factory.CreatePoint(new CoordinateZM(-1, -1, -1, -1));
            var pMax = factory.CreatePoint(new CoordinateZM(2, 2, 2, 2));
            var coll = factory.CreateMultiPoint(new[] { pMin, pMax });
            byte[] bytes;

            using (var shpStream = new MemoryStream())
            using (var shxStream = new MemoryStream())
            {
                using (var writer = new ShpMultiPointWriter(shpStream, shxStream, ShapeType.MultiPointZM))
                {                
                    writer.Write(coll);
                }
                // Dispose the writer to save the header
                bytes = shpStream.ToArray();
            }

            using (var reader = new ShpMultiPointReader(new MemoryStream(bytes)))
            {
                var collRead = reader.First();
                var bbox = collRead.EnvelopeInternal;
                var pMinRead = collRead.GetGeometryN(0) as Point;
                var pMaxRead = collRead.GetGeometryN(1) as Point;

                Assert.AreEqual(ShapeType.MultiPointZM, reader.ShapeType);
                Assert.AreEqual(pMin.X, bbox.MinX); // MinX
                Assert.AreEqual(pMin.Y, bbox.MinY); // MinY
                Assert.AreEqual(pMax.X, bbox.MaxX); // MaxX
                Assert.AreEqual(pMax.Y, bbox.MaxY); // MaxY
                Assert.AreEqual(coll.NumGeometries, collRead.NumGeometries); 

                Assert.AreEqual(pMin.Z, pMinRead.Z); // MinZ
                Assert.AreEqual(pMax.Z, pMaxRead.Z); // MaxZ

                Assert.AreEqual(pMin.M, pMinRead.M); // MinM
                Assert.AreEqual(pMin.M, pMinRead.M); // MaxM
            }
        }
    }
}

