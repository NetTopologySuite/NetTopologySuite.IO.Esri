using System;
using System.IO;
using System.Text;
using NetTopologySuite.IO.Streams;
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

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(encoding.GetBytes(constructorText));
                memoryStream.Position = 0;
                var bsp = new ExternallyManagedStreamProvider("Test", memoryStream);

                Assert.That(bsp.UnderlyingStreamIsReadonly, Is.False);

                using (var streamreader = new StreamReader(bsp.OpenRead(), encoding))
                {
                    string streamText = streamreader.ReadToEnd();
                    Assert.That(streamText, Is.EqualTo(constructorText));
                }
            }
        }

        [Test]
        public void TestWithReadonlyStream()
        {
            byte[] sourceData = CreateData(50);
            using var sourceStream = new MemoryStream(sourceData, 0, sourceData.Length, false);
            var provider = new ExternallyManagedStreamProvider("Test", sourceStream);
            Assert.That(provider.UnderlyingStreamIsReadonly);

            TestReading(provider, sourceData);

            Assert.That(() => provider.OpenWrite(false), Throws.InstanceOf<InvalidOperationException>());
            Assert.That(() => provider.OpenWrite(true), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void TestWithWritableStream()
        {
            byte[] sourceData = CreateData(50);
            using var sourceStream = new MemoryStream();
            sourceStream.Write(sourceData);
            sourceStream.Position = 0;
            var provider = new ExternallyManagedStreamProvider("Test", sourceStream);
            Assert.That(!provider.UnderlyingStreamIsReadonly);

            TestReading(provider, sourceData);

            // source stream position is now at the end.
            byte[] extraSourceData = CreateData(10);
            using var targetStream = provider.OpenWrite(false);
            targetStream.Write(extraSourceData);
            Assert.That(sourceStream, Has.Length.EqualTo(sourceData.Length + extraSourceData.Length));
            targetStream.Position = sourceData.Length;
            byte[] extraTargetData = new byte[extraSourceData.Length];
            int off = 0;
            while (off < extraTargetData.Length)
            {
                off += sourceStream.Read(extraTargetData.AsSpan(off));
            }

            Assert.That(extraTargetData, Is.EqualTo(extraSourceData));

            // original data shouldn't have been clobbered by that.
            sourceStream.Position = 0;
            sourceStream.SetLength(sourceData.Length);
            TestReading(provider, sourceData);
        }

        [Test]
        public void TestTruncate() {
            string test = "truncate string";

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(Encoding.ASCII.GetBytes(test));
                memoryStream.Position = 0;
                var bsp = new ExternallyManagedStreamProvider("Test", memoryStream);
                var stream = bsp.OpenWrite(true);

                Assert.That(stream.Length, Is.EqualTo(0));
            }
        }

        [Test]
        public void TestTruncateNonSeekableStream()
        {
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
        }

        private static void TestReading(ExternallyManagedStreamProvider provider, byte[] sourceData)
        {
            using var copyTargetStream = new MemoryStream();
            using var copySourceStream = provider.OpenRead();
            copySourceStream.CopyTo(copyTargetStream);
            Assert.That(copyTargetStream.ToArray(), Is.EqualTo(sourceData));
        }

        private static byte[] CreateData(int length)
        {
            byte[] res = new byte[length];
            new Random().NextBytes(res);
            return res;
        }

        private class NonSeekableStream : MemoryStream
        {
            public override bool CanSeek => false;
        }
    }
}
