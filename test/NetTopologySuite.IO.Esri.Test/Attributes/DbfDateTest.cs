using NUnit.Framework;
using System;
using System.Collections;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test.Attributes
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
            string file = Path.Combine(TestShapefiles.TestShapefilesDirectory, "date.dbf");

            if (!File.Exists(file))
                throw new FileNotFoundException("file not found at " + Path.GetDirectoryName(file));

            var reader = new DbaseFileReader(file);
            var header = reader.GetHeader();
            var ienum = reader.GetEnumerator();
            ienum.MoveNext();
            var items = ienum.Current as ArrayList;

            Assert.IsNotNull(items);
            Assert.AreEqual(2, items.Count);

            foreach (object item in items)
                Assert.IsNotNull(item);

            var date = (DateTime)items[1];

            Assert.AreEqual(10, date.Day);
            Assert.AreEqual(3, date.Month);
            Assert.AreEqual(2006, date.Year);
        }
    }
}
