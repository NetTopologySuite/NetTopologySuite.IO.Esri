using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shp.Readers;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Shapefiles.Readers
{

    /// <summary>
    /// Generic base class for reading a shapefile.
    /// </summary>
    public abstract class ShapefileReader<T> : ShapefileReader where T : Geometry
    {
        private protected readonly ShpReader<T> ShpReader;

        /// <inheritdoc/>
        public override ShapeType ShapeType => ShpReader.ShapeType;

        /// <summary>
        /// Shape geometry.
        /// </summary>
        public T Shape => ShpReader.Geometry;

        /// <inheritdoc/>
        public override Geometry Geometry => ShpReader.Geometry;



        /// <inheritdoc/>
        public override string Projection { get; } = null;




        /// <summary>
        /// Initializes a new instance of the reader class.
        /// </summary>
        /// <param name="shpStream">SHP file stream.</param>
        /// <param name="dbfStream">DBF file stream.</param>
        /// <param name="factory">Geometry factory.</param>
        /// <param name="encoding">DBF file encoding. If null encoding will be guess from related .CPG file or from reserved DBF bytes.</param>
        public ShapefileReader(Stream shpStream, Stream dbfStream, GeometryFactory factory, Encoding encoding)
            : base(new DbfReader(dbfStream, encoding))
        {
            ShpReader = CreateShpReader(shpStream, factory);

        }

        /// <summary>
        /// Initializes a new instance of the reader class.
        /// </summary>
        /// <param name="shpPath">Path to SHP file.</param>
        /// <param name="factory">Geometry factory.</param>
        /// <param name="encoding">DBF file encoding. If null encoding will be guess from related .CPG file or from reserved DBF bytes.</param>
        public ShapefileReader(string shpPath, GeometryFactory factory, Encoding encoding = null)
            : base(new DbfReader(Path.ChangeExtension(shpPath, ".dbf"), encoding))
        {
            try
            {
                var shpStream = OpenManagedFileStream(shpPath, ".shp", FileMode.Open);
                ShpReader = CreateShpReader(shpStream, factory);

                var prjFile = Path.ChangeExtension(shpPath, ".prj");
                if (File.Exists(prjFile))
                    Projection = File.ReadAllText(prjFile);
            }
            catch
            {
                DisposeManagedResources();
                throw;
            }
        }



        /// <summary>
        /// Reads feature geometry and attributes from underlying SHP and DBF files. 
        /// </summary>
        /// <param name="deleted">Indicates if the record was marked as deleted.</param>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next record;
        /// false if the enumerator has passed the end of the table.
        /// </returns>
        public override bool Read(out bool deleted)
        {
            var readShpSucceed = ShpReader.Read();
            var readDbfSucceed = DbfReader.Read(out deleted);

            if (readDbfSucceed != readShpSucceed)
            {
                throw new FileLoadException("Corrupted shapefile data. "
                    + "The dBASE table must contain feature attributes with one record per feature. "
                    + "There must be one-to-one relationship between geometry and attributes.");
            }
            return readDbfSucceed;
        }

        /// <inheritdoc/>
        public override bool Read(out bool deleted, out Feature feature)
        {
            var readSucceed = Read(out deleted);
            var attributes = new AttributesTable(Fields.GetValues());
            feature = new Feature(Shape, attributes);
            return readSucceed;
        }

        /// <inheritdoc/>
        public override void Restart()
        {
            DbfReader.Restart();
            ShpReader.Restart();
        }

        /// <inheritdoc/>
        protected override void DisposeManagedResources()
        {
            ShpReader?.Dispose();
            DbfReader?.Dispose();

            base.DisposeManagedResources(); // This will dispose streams used by ShpReader and DbfReader. Do it at the end.
        }

        internal abstract ShpReader<T> CreateShpReader(Stream shpStream, GeometryFactory factory);
    }



}
