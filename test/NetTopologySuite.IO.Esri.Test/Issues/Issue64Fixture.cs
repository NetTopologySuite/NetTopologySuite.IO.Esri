using NUnit.Framework;
using System.Collections;
using System.IO;

using NetTopologySuite.IO.Streams;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    [TestFixture]
    [ShapeFileIssueNumber(64)]
    public class Issue64Fixture
    {
        /// <summary>
        /// <see href="https://github.com/NetTopologySuite/NetTopologySuite.IO.ShapeFile/issues/64"/>
        /// </summary>
        [TestCase('L', 1)]
        [TestCase('D', 8)]
        [TestCase('F', 8)]
        [TestCase('N', 8)]
        [TestCase('C', 8)]
        public void Dbase_Read_Null(char fieldType, int fieldLength)
        {
            using var s = new MemoryStream();
            var provider = new ExternallyManagedStreamProvider(StreamTypes.Data, s);
            var reg = new ShapefileStreamProviderRegistry(null, provider);

            var header = new DbaseFileHeader();
            header.AddColumn("TestCol", fieldType, fieldLength, 0);
            header.NumRecords = 1;

            object[] values = new[] { (object)null };

            using (var writer = new DbaseFileWriter(reg))
            {
                writer.Write(header);
                writer.Write(values);
            }

            s.Position = 0;
            var reader = new DbaseFileReader(reg);

            reader.GetHeader();
            s.Position = 0;

            foreach (ArrayList readValues in reader)
            {
                Assert.AreEqual(values[0], readValues[0]);
            }
        }
    }
}
