using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.ShapeFile.Extended
{
    internal sealed class TempFileWriter : IDisposable
    {
        private List<Stream> OpenedStreams = new List<Stream>();
        private bool IsDisposed;

        public TempFileWriter(string ext, byte[] data)
        {
            this.Path = System.IO.Path.GetFullPath(System.IO.Path.ChangeExtension(TestContext.CurrentContext.Test.ID, ext));
            File.WriteAllBytes(this.Path, data);
        }

        public TempFileWriter(string ext, string testFile)
        {
            string file = System.IO.Path.ChangeExtension(testFile, ext);
            string path = TestShapefiles.PathTo(file);
            Assert.That(File.Exists(path), Is.True);

            this.Path = System.IO.Path.GetFullPath(System.IO.Path.ChangeExtension(TestContext.CurrentContext.Test.ID, ext));
            byte[] data = File.ReadAllBytes(path);
            File.WriteAllBytes(this.Path, data);
        }

        public string Path { get; }

        public Stream OpenRead()
        {
            var stream = File.OpenRead(Path);
            OpenedStreams.Add(stream);
            return stream;
        }

        private void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            foreach (var stream in OpenedStreams)
            {
                stream.Dispose();
            }

            try
            {
                File.Delete(this.Path);
            }
            catch
            {
            }

            IsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~TempFileWriter()
        {
            Dispose(disposing: false);
        }
    }
}
