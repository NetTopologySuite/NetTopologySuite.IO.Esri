using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test.Streams
{
    [TestFixture]
    public class ByteStreamProviderFixture
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
            textField.StringValue = constructorText;
            dbfWriter.Write();

            using var dbfReader = new Dbf.DbfReader(ms, encoding);
            dbfReader.Read();
            var readText = dbfReader.Fields[textField.Name].Value;
            Assert.That(readText, Is.EqualTo(constructorText));
            
        }

        [TestCase(50, 50, true)]
        [TestCase(50, 100, true)]
        [TestCase(50, 50, false)]
        [TestCase(50, 100, false)]
        public void TestConstructor(int length, int maxLength, bool @readonly)
        {
            // TODO: Remove no longer relevant test
            /*
            var bsp = new ByteStreamProvider("Test", CreateData(length), maxLength, @readonly);
            Assert.That(bsp.UnderlyingStreamIsReadonly, Is.EqualTo(@readonly));
            Assert.That(bsp.Length, Is.EqualTo(length));
            Assert.That(bsp.MaxLength, Is.EqualTo(maxLength));

            using (var ms = (MemoryStream)bsp.OpenRead())
            {
                byte[] data = ms.ToArray();
                Assert.That(data, Is.Not.Null);
                Assert.That(data.Length, Is.EqualTo(length));
                for (int i = 0; i < length; i++)
                    Assert.That(data[i], Is.EqualTo(bsp.Buffer[i]));
            }

            try
            {
                using (var ms = (MemoryStream)bsp.OpenWrite(false))
                {
                    var sw = new BinaryWriter(ms);
                    sw.BaseStream.Position = 50;
                    for (int i = 0; i < 10; i++)
                        sw.Write((byte)i);
                    sw.Flush();
                    Assert.That(ms.Length, Is.EqualTo(length+10));
                    Assert.That(bsp.Length, Is.EqualTo(length+10));
                    Assert.That(bsp.Buffer[59], Is.EqualTo(9));
                }
            }
            catch (Exception ex)
            {
                if (ex is AssertionException)
                    throw;

                if (!@readonly)
                {
                    Assert.That(ex, Is.TypeOf(typeof(NotSupportedException)));
                    Assert.That(length, Is.EqualTo(maxLength));
                }
            }
            */
        }

        private static byte[] CreateData(int length)
        {
            var rnd = new Random();

            byte[] res = new byte[length];
            for (int i = 0; i < length; i++)
                res[i] = (byte) rnd.Next(0, 255);
            return res;
        }
    }
}
