using NetTopologySuite.Geometries;
using NUnit.Framework;
using System;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test
{
    [TestFixture]
    public class Issue178Fixture
    {
        [SetUp]
        public void SetUp()
        {
            // Set current dir to shapefiles dir
            Environment.CurrentDirectory = CommonHelpers.TestShapefilesDirectory;
        }

        [Test]
        public void TestCorruptedShapeFile()
        {
            var factory = GeometryFactory.Default;
            const string filename = "christchurch-canterbury-h.shp";
            Assert.Throws<ShapefileException>(() =>
            {
                var reader = new ShapefileReader(filename, factory);
                Assert.Fail("Invalid file: code should be unreachable");
            });

            // ensure file isn't locked
            string path = Path.Combine(Environment.CurrentDirectory, filename);
            bool ok;
            using (var file = File.OpenRead(path))
            {
                using (var reader = new BinaryReader(file))
                {
                    // read a value
                    int val = reader.Read();
                    Console.WriteLine("read a value: " + val);
                    ok = true;
                }
            }
            Assert.That(ok, Is.True);
        }
    }
}
