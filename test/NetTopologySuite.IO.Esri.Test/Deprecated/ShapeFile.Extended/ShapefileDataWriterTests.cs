using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;
using NetTopologySuite.IO.Esri.Dbf;
using NUnit.Framework;
using System;
using System.Linq;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.Deprecated.Extended
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
            var feature = new Feature(new Point(0, 0), new AttributesTable());
            feature.Attributes.Add("c_long", (long)12345678900000000);
            feature.Attributes.Add("c_ulong", (ulong)12345678900000000);
            feature.Attributes.Add("c_int", int.MaxValue);
            feature.Attributes.Add("c_uint", uint.MinValue);
            feature.Attributes.Add("c_short", short.MaxValue);
            feature.Attributes.Add("c_ushort", ushort.MaxValue);
            feature.Attributes.Add("c_string", string.Empty);
            feature.Attributes.Add("c_double", double.MinValue);
            feature.Attributes.Add("c_bool", false);
            feature.Attributes.Add("c_datetime", new DateTime(1999, 01, 01));

            var shpPath = TestShapefiles.GetTempShpPath();
            Shapefile.WriteAllFeatures(Enumerable.Repeat(feature, 1), shpPath);
            using var shapefile = Shapefile.OpenRead(shpPath);
            var fields = shapefile.Fields;

            Assert.IsNotNull(fields);
            Assert.AreEqual(10, fields.Count);

            var field = fields["c_long"];
            Assert.IsNotNull(field);
            Assert.AreEqual(DbfType.Numeric, field.FieldType);
            Assert.AreEqual(0, field.NumericScale);
            Assert.AreEqual(19, field.Length);

            field = fields["c_ulong"];
            Assert.IsNotNull(field);
            Assert.AreEqual(DbfType.Numeric, field.FieldType);
            Assert.AreEqual(0, field.NumericScale);
            Assert.AreEqual(19, field.Length);

            field = fields["c_int"];
            Assert.IsNotNull(field);
            Assert.AreEqual(DbfType.Numeric, field.FieldType);
            Assert.AreEqual(0, field.NumericScale);
            Assert.AreEqual(10, field.Length);

            field = fields["c_uint"];
            Assert.IsNotNull(field);
            Assert.AreEqual(DbfType.Numeric, field.FieldType);
            Assert.AreEqual(0, field.NumericScale);
            Assert.AreEqual(10, field.Length);

            field = fields["c_short"];
            Assert.IsNotNull(field);
            Assert.AreEqual(DbfType.Numeric, field.FieldType);
            Assert.AreEqual(0, field.NumericScale);
            Assert.AreEqual(10, field.Length);

            field = fields["c_ushort"];
            Assert.IsNotNull(field);
            Assert.AreEqual(DbfType.Numeric, field.FieldType);
            Assert.AreEqual(0, field.NumericScale);
            Assert.AreEqual(10, field.Length);

            field = fields["c_string"];
            Assert.IsNotNull(field);
            Assert.AreEqual(DbfType.Character, field.FieldType);
            Assert.AreEqual(0, field.NumericScale);
            Assert.AreEqual(254, field.Length);

            field = fields["c_double"];
            Assert.IsNotNull(field);
            Assert.AreEqual(DbfType.Float, field.FieldType);
            Assert.AreEqual(11, field.NumericScale);
            Assert.AreEqual(19, field.Length);

            field = fields["c_bool"];
            Assert.IsNotNull(field);
            Assert.AreEqual(DbfType.Logical, field.FieldType);
            Assert.AreEqual(0, field.NumericScale);
            Assert.AreEqual(1, field.Length);

            field = fields["c_datetime"];
            Assert.IsNotNull(field);
            Assert.AreEqual(DbfType.Date, field.FieldType);
            Assert.AreEqual(0, field.NumericScale);
            Assert.AreEqual(8, field.Length);

        }
    }
}
