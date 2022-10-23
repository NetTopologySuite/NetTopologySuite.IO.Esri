using NUnit.Framework;
using System.IO;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using System.Collections.Generic;
using NetTopologySuite.IO.Esri.Dbf;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    /// <summary>
    /// <see href="https://github.com/NetTopologySuite/NetTopologySuite.IO.ShapeFile/issues/64"/>
    /// </summary>
    [TestFixture]
    [ShapeFileIssueNumber(64)]
    public class Issue64Fixture
    {
        public void Dbase_Read_Null(DbfField field)
        {
            using var s = new MemoryStream();

            var fields = new List<DbfField>();
            fields.Add(field);

            var values = new Dictionary<string, object>()
            {
                { field.Name, null }
            };

            using (var writer = new DbfWriter(s, fields))
            {
                writer.Write(values);
            }

            s.Position = 0;
            using (var reader = new DbfReader(s))
            {
                foreach (var readValues in reader)
                {
                    Assert.AreEqual(values[field.Name], readValues[field.Name]);
                }
            }
        }

        [Test]
        public void Dbase_Read_Null_Logical()
        {
            Dbase_Read_Null(new DbfLogicalField("field_name"));
        }

        [Test]
        public void Dbase_Read_Null_Date()
        {
            Dbase_Read_Null(new DbfDateField("field_name"));
        }

        [Test]
        public void Dbase_Read_Null_Float()
        {
            Dbase_Read_Null(new DbfFloatField("field_name"));
        }

        [Test]
        public void Dbase_Read_Null_Numeric()
        {
            Dbase_Read_Null(new DbfNumericInt32Field("field_name"));
        }

        [Test]
        public void Dbase_Read_Null_Character()
        {
            Dbase_Read_Null(new DbfCharacterField("field_name"));
        }
    }
}
