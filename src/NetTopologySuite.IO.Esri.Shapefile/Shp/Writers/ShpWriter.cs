using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shx;
using System;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Writers
{

    /// <summary>
    /// Base class class for writing a fixed-length file header and variable-length records to a *.SHP file.
    /// </summary>
    public abstract class ShpWriter<T> : Shp where T : Geometry
    {
        private readonly Stream ShpStream;
        private readonly Stream ShxStream;
        private int RecordNumber = 1; // Shapefile specs: Record numbers begin at 1.
        private readonly MemoryStream Buffer;

        /// <summary>
        /// SHP geometry.
        /// </summary>
        public T Geometry { get; set; }

        internal ShpExtent Extent { get; private set; } = new ShpExtent();


        /// <summary>
        /// Initializes a new instance of the writer class.
        /// </summary>
        /// <param name="shpStream">SHP file stream.</param>
        /// <param name="shxStream">SHX file stream.</param>
        /// <param name="shapeType">Shape type.</param>
        public ShpWriter(Stream shpStream, Stream shxStream, ShapeType shapeType)
            : base(shapeType)
        {
            ShpStream = shpStream ?? throw new ArgumentNullException("Uninitialized SHP stream.", nameof(shpStream));
            ShxStream = shxStream ?? throw new ArgumentNullException("Uninitialized SHX stream.", nameof(shxStream));

            Buffer = new MemoryStream();
            AddManagedResource(Buffer);

            // At this stage ShpStream.Length and Extent are unknown.
            // Writing dummy data is done in order to advances streams position past header to to records start position
            WriteFileHeader(ShpStream);
            WriteFileHeader(ShxStream);
        }

        /// <summary>
        /// Writes the geometry to the underlying stream.
        /// </summary>
        public void Write()
        {
            Buffer.Clear();
            if (Geometry == null)
            {
                throw new ArgumentNullException(nameof(Geometry));
            }
            if (Geometry.IsEmpty)
            {
                Buffer.WriteGeometryType(ShapeType.NullShape);
            }
            else
            {
                Buffer.WriteGeometryType(ShapeType);
                WriteGeometry(Geometry, Buffer);
            }

            ShxStream.WriteShxRecord((int)ShpStream.Position, (int)Buffer.Length);

            ShpStream.WriteShpRecordHeader(RecordNumber, (int)Buffer.Length);
            ShpStream.WriteAllBytes(Buffer);

            RecordNumber++;
        }

        internal abstract void WriteGeometry(T geometry, Stream shapeBinary);


        private void WriteFileHeader(Stream stream)
        {
            if (stream == null)
                return;
            stream.Seek(0, SeekOrigin.Begin);
            stream.WriteShpFileHeader(ShapeType, (int)stream.Length, Extent, HasZ, HasM);
        }

        /// <inheritdoc/>
        protected override void DisposeManagedResources()
        {
            if (ShpStream != null && ShpStream.Position > Shapefile.FileHeaderSize)
            {
                WriteFileHeader(ShpStream);
                WriteFileHeader(ShxStream);
            }
            base.DisposeManagedResources(); // This will dispose owned ShpStream and ShxStream.
        }
    }


}
