using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test.Streams
{
    [TestFixture]
    public class ManagedStreamProviderFixture
    {
        [TestCase("This is sample text", 1252)]
        [TestCase("Dies sind deutsche Umlaute: Ää. Öö, Üü, ß", 1252)]
        [TestCase("Dies sind deutsche Umlaute: Ää. Öö, Üü, ß", 850)]
        [TestCase("Dies sind deutsche Umlaute: Ää. Öö, Üü, ß", 437)]
        public void TestConstructorText(string constructorText, int codepage)
        {
            var encoding = CodePagesEncodingProvider.Instance.GetEncoding(codepage);

            var textField = new Dbf.Fields.DbfCharacterField("test");
            var fields = new Dbf.Fields.DbfField[] { textField };
            using var ms = new MemoryStream();

            using var dbfWriter = new Dbf.DbfWriter(ms, fields, encoding);
            textField.Value = constructorText;
            dbfWriter.Write();

            using var dbfReader = new Dbf.DbfReader(ms, encoding);
            dbfReader.Read();
            var readText = dbfReader.Fields[textField.Name].Value;
            Assert.That(readText, Is.EqualTo(constructorText));
        }

        [Test]
        public void TestWithReadonlyStream()
        {
            byte[] sourceData = CreateDbfData(50);
            using var sourceStream = new MemoryStream(sourceData, 0, sourceData.Length, false);
            Assert.That(!sourceStream.CanWrite);

            using var dbfReader = new Dbf.DbfReader(sourceStream);
            TestReading(dbfReader, sourceData);

            Assert.That(() => new Dbf.DbfWriter(sourceStream, dbfReader.Fields), Throws.InstanceOf<NotSupportedException>());
        }

        [Test]
        public void TestWithWritableStream()
        {
            byte[] sourceData = CreateDbfData(50);
            using var sourceStream = new MemoryStream();
            sourceStream.Write(sourceData);
            sourceStream.Position = 0;
            Assert.That(sourceStream.CanWrite);

            using var dbfReader = new Dbf.DbfReader(sourceStream);
            TestReading(dbfReader, sourceData);
        }

        [Test]
        public void TestTruncate() {
            // TODO: Remove no longer relevant test
            /*
            string test = "truncate string";

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(Encoding.ASCII.GetBytes(test));
                memoryStream.Position = 0;
                var bsp = new ExternallyManagedStreamProvider("Test", memoryStream);
                var stream = bsp.OpenWrite(true);

                Assert.That(stream.Length, Is.EqualTo(0));
            }
            */
        }

        [Test]
        public void TestTruncateNonSeekableStream()
        {
            // TODO: Remove no longer relevant test
            /*
            string test = "truncate string";

            using (var memoryStream = new NonSeekableStream())
            {
                try
                {
                    memoryStream.Write(Encoding.ASCII.GetBytes(test));
                    memoryStream.Position = 0;
                    var bsp = new ExternallyManagedStreamProvider("Test", memoryStream);
                    var stream = bsp.OpenWrite(true);
                }
                catch (InvalidOperationException ex)
                {
                    Assert.AreEqual(ex.Message, "The underlying stream doesn't support seeking! You are unable to truncate the data.");
                }

            }
            */
        }

        private static void TestReading(Dbf.DbfReader dbfReader, byte[] sourceData)
        {
            using var ms = new MemoryStream();
            using var dbfWriter = new Dbf.DbfWriter(ms, dbfReader.Fields, dbfReader.Encoding);
            while (dbfReader.Read())
            {
                // We are reusing the fields from dbfReader.
                // DbfField.Value is Read() from dbfReader and then the same value
                // from the same field is written to dbfWriter.
                // So bellow assignment is unnecessary:
                // dbfWriter.Fields[0].Value = dbfReader.Fields[0].Value.
                dbfWriter.Write();
            }
            dbfWriter.Dispose(); // Flush changes.
            Assert.That(ms.ToArray(), Is.EqualTo(sourceData));
        }

        private static byte[] CreateDbfData(int length)
        {
            var textField = new Dbf.Fields.DbfCharacterField("field_name");
            var fields = new Dbf.Fields.DbfField[] { textField };
            using var ms = new MemoryStream();

            using var dbfWriter = new Dbf.DbfWriter(ms, fields);
            for (int i = 0; i < length; i++)
            {
                textField.Value = "field_value_" + i;
                dbfWriter.Write();
            }
            dbfWriter.Dispose(); // Write header to underlying stream

            return ms.ToArray();
        }

        private class NonSeekableStream : MemoryStream
        {
            public override bool CanSeek => false;
        }
    }
}
