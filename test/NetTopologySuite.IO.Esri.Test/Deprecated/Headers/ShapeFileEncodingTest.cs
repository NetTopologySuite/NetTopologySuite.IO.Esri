using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.Headers
{
    [TestFixture]
    public class ShapeFileEncodingTest
    {
        [SetUp]
        public void Setup()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var options = new ShapefileWriterOptions(ShapeType.Point)
            {
                Encoding = CodePagesEncodingProvider.Instance.GetEncoding(1252)
            };
            options.AddNumericInt32Field("id");
            options.AddCharacterField("Test", 15);
            options.AddNumericInt32Field("Ålder");
            options.AddCharacterField("Ödestext");
            using var sfdr = Shapefile.OpenWrite("encoding_sample", options);

            var feats = new List<IFeature>();
            var at = new AttributesTable();
            at.Add("id", "0");
            at.Add("Test", "Testar");
            at.Add("Ålder", 10);
            at.Add("Ödestext", "Lång text med åäö etc");
            feats.Add(new Feature(new Point(0, 0), at));
            sfdr.Write(feats);
        }

        [Test]
        public void TestLoadShapeFileWithEncoding()
        {
            var reader = Shapefile.OpenRead("encoding_sample.shp");
            Assert.AreEqual(reader.Encoding, CodePagesEncodingProvider.Instance.GetEncoding(1252), "Invalid encoding!");

            Assert.AreEqual(reader.Fields[1].Name, "Test");
            Assert.AreEqual(reader.Fields[2].Name, "Ålder");
            Assert.AreEqual(reader.Fields[3].Name, "Ödestext");

            Assert.IsTrue(reader.Read(), "Error reading file");
            Assert.AreEqual(reader.Fields["Test"].Value, "Testar");
            Assert.AreEqual(reader.Fields["Ödestext"].Value, "Lång text med åäö etc");
        }
    }
}
