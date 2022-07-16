using NetTopologySuite.Geometries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Readers
{

    /// <summary>
    /// Base class class for reading a fixed-length file header and variable-length records from a *.SHP file. 
    /// </summary>
    public abstract class ShpReader<T> : ShpReader where T : Geometry
    {
        private readonly Stream ShpStream;
        private readonly int ShpEndPosition;
        private readonly MemoryStream Buffer;
        private readonly Envelope MbrEnvelope = null;
        private readonly Geometry MbrGeometry = null;

        /// <summary>
        /// Shapefile Spec: <br/>
        /// The one-to-one relationship between geometry and attributes is based on record number.
        /// Attribute records in the dBASE file must be in the same order as records in the main file.
        /// </summary>
        /// <remarks>
        /// DBF does not have recor number attribute.
        /// </remarks>
        private int RecordNumber = 1;
        private readonly int DbfRecordCount;

        internal GeometryFactory Factory { get; }

        private readonly Envelope _boundingBox;
        /// <inheritdoc/>
        public override Envelope BoundingBox => _boundingBox.Copy(); // Envelope is not immutable

        /// <summary>
        /// SHP geometry.
        /// </summary>
        public T Shape { get; private set; }

        /// <inheritdoc/>
        public override Geometry Geometry => Shape;

        /// <summary>
        /// Initializes a new instance of the reader class.
        /// </summary>
        /// <param name="shpStream">SHP file stream.</param>
        /// <param name="factory">Geometry factory.</param>
        /// <param name="mbrFilter">The minimum bounding rectangle (BMR) used to filter out shapes located outside it.</param>
        /// <param name="dbfRecordCount">DBF record count.</param>
        internal ShpReader(Stream shpStream, GeometryFactory factory, Envelope mbrFilter, int dbfRecordCount)
            : base(Shapefile.GetShapeType(shpStream))
        {
            ShpStream = shpStream ?? throw new ArgumentNullException("Uninitialized SHP stream.", nameof(shpStream));
            Factory = factory ?? Geometry.DefaultFactory;

            if (mbrFilter?.IsNull == false)
            {
                MbrEnvelope = mbrFilter.Copy(); // Envelope is not immutable
            }
            if (MbrEnvelope != null)
            {
                MbrGeometry = Factory.ToGeometry(MbrEnvelope);
            }

            if (dbfRecordCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dbfRecordCount));
            }
            DbfRecordCount = dbfRecordCount;

            if (ShpStream.Position != 0)
                ShpStream.Seek(0, SeekOrigin.Begin);

            Buffer = new MemoryStream();
            AddManagedResource(Buffer);

            Buffer.AssignFrom(ShpStream, Shapefile.FileHeaderSize);
            Buffer.ReadShpFileHeader(out _, out var fileLength, out _boundingBox);
            ShpEndPosition = fileLength - 1;
        }

        internal override void Restart()
        {
            ShpStream.Seek(Shapefile.FileHeaderSize, SeekOrigin.Begin);
        }

        /// <inheritdoc/>
        public override bool Read()
        {
            return Read(out var _);
        }

        /// <inheritdoc/>
        internal bool Read(out int skippedCount)
        {
            skippedCount = 0;
            return ReadCore(ref skippedCount);
        }

        /// <inheritdoc/>
        internal bool ReadCore(ref int skippedCount)
        {
            if (ShpStream.Position >= ShpEndPosition)
            {
                Shape = null;
                return false;
            }
            if (RecordNumber > DbfRecordCount)
            {
                Shape = null;
                return false;
            }

            (var recordNumber, var contentLength) = ShpStream.ReadShpRecordHeader();
            Debug.Assert(recordNumber == RecordNumber, "Shapefile record", $"Unexpected SHP record number: {recordNumber} (expected {RecordNumber}).");
            Debug.Assert(contentLength >= sizeof(int), "Shapefile record", $"Unexpected SHP record content size: {contentLength} (expected >= {sizeof(int)}).");

            Buffer.AssignFrom(ShpStream, contentLength);
            RecordNumber++;

            var type = Buffer.ReadShapeType();
            if (type == ShapeType.NullShape)
            {
                Shape = GetEmptyGeometry();
                return true;
            }
            else if (type != ShapeType)
            {
                ThrowInvalidRecordTypeException(type);
            }

            if (!ReadGeometry(Buffer, out var geometry))
            {
                skippedCount++;
                return ReadCore(ref skippedCount); 
            }

            Shape = geometry;
            return true;
        }

        internal bool IsInMbr(Envelope boundingBox)
        {
            if (MbrEnvelope == null)
            {
                return true;
            }
            return MbrEnvelope.Intersects(boundingBox);
        }

        internal bool IsInMbr(Geometry geometry)
        {
            if (MbrGeometry == null)
            {
                return true;
            }
            return MbrGeometry.Intersects(geometry);
        }

        internal abstract T GetEmptyGeometry();

        internal abstract bool ReadGeometry(Stream stream, out T geometry);

        internal CoordinateSequence CreateCoordinateSequence(int size)
        {
            return Factory.CoordinateSequenceFactory.Create(size, HasZ, HasM);
        }

        internal void ThrowInvalidRecordTypeException(ShapeType shapeType)
        {
            throw new FileLoadException($"Ivalid shapefile record type. {GetType().Name} does not support {shapeType} shapes.");
        }

        internal void ThrowInvalidRecordException(string message)
        {
            throw new FileLoadException(message);
        }
    }


}
