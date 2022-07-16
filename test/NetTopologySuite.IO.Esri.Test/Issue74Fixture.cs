using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Handlers;
using NUnit.Framework;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test
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
            var handler = new MultiPointHandler(ShapeGeometryType.MultiPointZM);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                using var writer = new BinaryWriter(stream);
                handler.Write(coll, writer, factory);
                bytes = stream.ToArray();
            }

            using var reader = new BinaryReader(new MemoryStream(bytes));
            Assert.AreEqual((int)ShapeGeometryType.MultiPointZM, reader.ReadInt32());
            Assert.AreEqual(pMin.X, reader.ReadDouble()); // MinX
            Assert.AreEqual(pMin.Y, reader.ReadDouble()); // MinY
            Assert.AreEqual(pMax.X, reader.ReadDouble()); // MaxX
            Assert.AreEqual(pMax.Y, reader.ReadDouble()); // MaxY
            Assert.AreEqual(coll.NumGeometries, reader.ReadInt32());
            for (int i = 0; i < 4; i++)
            {
                // Skip XY values
                reader.ReadDouble();
            }
            Assert.AreEqual(pMin.Z, reader.ReadDouble()); // MinZ
            Assert.AreEqual(pMax.Z, reader.ReadDouble()); // MaxZ
            for (int i = 0; i < 2; i++)
            {
                // Skip Z values
                reader.ReadDouble();
            }
            Assert.AreEqual(pMin.M, reader.ReadDouble()); // MinM
            Assert.AreEqual(pMax.M, reader.ReadDouble()); // MaxM
            for (int i = 0; i < 2; i++)
            {
                // Skip M values
                reader.ReadDouble();
            }
        }
    }
}

