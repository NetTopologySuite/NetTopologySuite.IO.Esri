using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NetTopologySuite.IO.Esri
{
    /// <summary>
    /// Base class witha a mechanism for releasing managed resources.
    /// </summary>
    public abstract class ManagedDisposable : IDisposable
    {
        private bool IsDisposed = false;
        private readonly List<IDisposable> ManagedResources = new List<IDisposable>();


        internal Stream OpenManagedFileStream(string path, string ext, FileMode mode)
        {
            path = Path.ChangeExtension(path, ext);

            if (mode != FileMode.Open && mode != FileMode.Create)
                throw new ArgumentException(nameof(OpenManagedFileStream) + "() suports only " + nameof(FileMode.Open) + " and " + nameof(FileMode.Create) + " file modes.", nameof(mode));

            if (mode == FileMode.Open && !File.Exists(path))
                throw new FileNotFoundException("File not found.", path);

            FileAccess access = (mode == FileMode.Open) ? FileAccess.Read : FileAccess.Write;
            FileShare share = (mode == FileMode.Open) ? FileShare.Read : FileShare.None;
            FileOptions options = (mode == FileMode.Open) ? FileOptions.SequentialScan : FileOptions.None;


            var stream = new FileStream(path, mode, access, share, 4096, options);
            AddManagedResource(stream);
            return stream;
        }

        private void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                // Dispose managed state (managed objects)
                DisposeManagedResources();
            }

            IsDisposed = true;
        }

        /// <summary>
        /// Adds a resource which will be disposed at the end of life of this instance.
        /// </summary>
        /// <param name="resource"></param>
        protected void AddManagedResource(IDisposable resource)
        {
            ManagedResources.Add(resource);
        }


        /// <summary>
        /// Disposes managed objects assosiated with this instance.
        /// </summary>
        protected virtual void DisposeManagedResources()
        {
            for (int i = ManagedResources.Count - 1; i >= 0; i--)
            {
                try
                {
                    ManagedResources[i]?.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(GetType().Name + " failed to dispose managed resources.");
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
