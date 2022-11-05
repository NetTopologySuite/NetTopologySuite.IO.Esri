using NUnit.Framework;
using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.Attributes
{
    /// <summary>
    ///
    /// </summary>
    [TestFixture]
    public class DbfDateTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NormalizeTest"/> class.
        /// </summary>
        public DbfDateTest()
        {

        }

        /// <summary>
        ///
        /// </summary>
        [Test]
        public void ReadDbfDate()
        {
            string file = TestShapefiles.PathTo("date.dbf");

            if (!File.Exists(file))
                throw new FileNotFoundException("file not found at " + Path.GetDirectoryName(file));

            using var reader = new Dbf.DbfReader(file);
            var record = reader.FirstOrDefault();

            Assert.IsNotNull(record);
            Assert.AreEqual(2, record.Count);

            foreach (object value in record.GetValues())
                Assert.IsNotNull(value);

            var dateFieldName = reader.Fields[1].Name;
            var date = (DateTime)record[dateFieldName];

            Assert.AreEqual(10, date.Day);
            Assert.AreEqual(3, date.Month);
            Assert.AreEqual(2006, date.Year);
        }
    }
}
