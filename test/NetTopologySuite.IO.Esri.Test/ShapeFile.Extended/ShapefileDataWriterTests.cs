using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using System;
using System.Linq;

namespace NetTopologySuite.IO.Tests.ShapeFile.Extended
{
    /// <summary>
    /// Contains tests for the shapefile data writer.
    /// </summary>
    [TestFixture]
    public class ShapefileDataWriterTests
    {
        /// <summary>
        /// Tests creating a header from a feature.
        /// </summary>
        [Test]
        public void TestGetHeaderFromFeature()
        {
            var feature = new Feature(new Point(0, 0),
                new AttributesTable());
            feature.Attributes.Add("c_long", (long)12345678900000000);
            feature.Attributes.Add("c_ulong", (ulong)12345678900000000);
            feature.Attributes.Add("c_int", int.MinValue);
            feature.Attributes.Add("c_uint", uint.MinValue);
            feature.Attributes.Add("c_short", short.MaxValue);
            feature.Attributes.Add("c_ushort", ushort.MaxValue);
            feature.Attributes.Add("c_string", string.Empty);
            feature.Attributes.Add("c_double", double.MinValue);
            feature.Attributes.Add("c_bool", false);
            feature.Attributes.Add("c_datetime", new DateTime(1999, 01, 01));

            var header = ShapefileDataWriter.GetHeader(feature, 1);

            Assert.IsNotNull(header);
            Assert.AreEqual(10, header.Fields.Length);
            var field = header.Fields.FirstOrDefault(x => x.Name == "c_long");
            Assert.IsNotNull(field);
            Assert.AreEqual(78, field.DbaseType);
            Assert.AreEqual(0, field.DecimalCount);
            Assert.AreEqual(18, field.Length);
            field = header.Fields.FirstOrDefault(x => x.Name == "c_ulong");
            Assert.IsNotNull(field);
            Assert.AreEqual(78, field.DbaseType);
            Assert.AreEqual(0, field.DecimalCount);
            Assert.AreEqual(18, field.Length);
            field = header.Fields.FirstOrDefault(x => x.Name == "c_int");
            Assert.IsNotNull(field);
            Assert.AreEqual(78, field.DbaseType);
            Assert.AreEqual(0, field.DecimalCount);
            Assert.AreEqual(10, field.Length);
            field = header.Fields.FirstOrDefault(x => x.Name == "c_uint");
            Assert.IsNotNull(field);
            Assert.AreEqual(78, field.DbaseType);
            Assert.AreEqual(0, field.DecimalCount);
            Assert.AreEqual(10, field.Length);
            field = header.Fields.FirstOrDefault(x => x.Name == "c_short");
            Assert.IsNotNull(field);
            Assert.AreEqual(78, field.DbaseType);
            Assert.AreEqual(0, field.DecimalCount);
            Assert.AreEqual(10, field.Length);
            field = header.Fields.FirstOrDefault(x => x.Name == "c_ushort");
            Assert.IsNotNull(field);
            Assert.AreEqual(78, field.DbaseType);
            Assert.AreEqual(0, field.DecimalCount);
            Assert.AreEqual(10, field.Length);
            field = header.Fields.FirstOrDefault(x => x.Name == "c_string");
            Assert.IsNotNull(field);
            Assert.AreEqual(67, field.DbaseType);
            Assert.AreEqual(0, field.DecimalCount);
            Assert.AreEqual(254, field.Length);
            field = header.Fields.FirstOrDefault(x => x.Name == "c_double");
            Assert.IsNotNull(field);
            Assert.AreEqual(78, field.DbaseType);
            Assert.AreEqual(8, field.DecimalCount);
            Assert.AreEqual(18, field.Length);
            field = header.Fields.FirstOrDefault(x => x.Name == "c_bool");
            Assert.IsNotNull(field);
            Assert.AreEqual(76, field.DbaseType);
            Assert.AreEqual(0, field.DecimalCount);
            Assert.AreEqual(1, field.Length);
            field = header.Fields.FirstOrDefault(x => x.Name == "c_datetime");
            Assert.IsNotNull(field);
            Assert.AreEqual(68, field.DbaseType);
            Assert.AreEqual(0, field.DecimalCount);
            Assert.AreEqual(8, field.Length);
        }
    }
}