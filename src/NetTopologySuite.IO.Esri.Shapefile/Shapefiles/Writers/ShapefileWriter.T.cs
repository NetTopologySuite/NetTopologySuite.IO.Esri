using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf;
using NetTopologySuite.IO.Esri.Shp.Writers;
using System;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shapefiles.Writers
{

    /// <summary>
    /// Generic base class for writing a shapefile.
    /// </summary>
    public abstract class ShapefileWriter<T> : ShapefileWriter where T : Geometry
    {
        private const int StreamBufferSize = 1024;
        private readonly ShpWriter<T> ShpWriter;

        /// <inheritdoc/>
        public override ShapeType ShapeType { get; }


        /// <summary>
        /// Shape geometry.
        /// </summary>
        public T Shape
        {
            get => ShpWriter.Geometry;
            set => ShpWriter.Geometry = value;
        }

        /// <inheritdoc/>
        public override Geometry Geometry
        {
            get => ShpWriter.Geometry;
            set => ShpWriter.Geometry = (T)value;
        }


        /// <summary>
        /// Initializes a new instance of the writer class.
        /// </summary>
        /// <param name="shpStream">SHP file stream.</param>
        /// <param name="shxStream">SHX file stream.</param>
        /// <param name="dbfStream">DBF file stream.</param>
        /// <param name="prjStream">PRJ file stream.</param>
        /// <param name="options">Writer options.</param>
        internal ShapefileWriter(Stream shpStream, Stream shxStream, Stream dbfStream, Stream prjStream, ShapefileWriterOptions options)
            : base(new DbfWriter(dbfStream, options?.Fields, options?.Encoding))
        {
            try
            {
                options = options ?? throw new ArgumentNullException(nameof(options));
                ShapeType = options.ShapeType;
                ShpWriter = CreateShpWriter(shpStream, shxStream);

                if (!string.IsNullOrWhiteSpace(options.Projection) && prjStream != null)
                {
                    using (var writer = new StreamWriter(prjStream, options.Encoding, StreamBufferSize, true))
                    {
                        writer.Write(options.Projection);
                    }
                }
            }
            catch
            {
                DisposeManagedResources();
                throw;
            }
        }


        /// <summary>
        /// Initializes a new instance of the writer class.
        /// </summary>
        /// <param name="shpPath">Path to SHP file.</param>
        /// <param name="options">Writer options.</param>
        internal ShapefileWriter(string shpPath, ShapefileWriterOptions options)
            : base(new DbfWriter(Path.ChangeExtension(shpPath, ".dbf"), options?.Fields, options?.Encoding))
        {
            try
            {
                options = options ?? throw new ArgumentNullException(nameof(options));
                var shpStream = OpenManagedFileStream(shpPath, ".shp", FileMode.Create);
                var shxStream = OpenManagedFileStream(shpPath, ".shx", FileMode.Create);

                ShapeType = options.ShapeType;
                ShpWriter = CreateShpWriter(shpStream, shxStream); // It calls this.ShapeType

                if (!string.IsNullOrWhiteSpace(options.Projection))
                    File.WriteAllText(Path.ChangeExtension(shpPath, ".prj"), options.Projection);
            }
            catch
            {
                DisposeManagedResources();
                throw;
            }

        }

        internal abstract ShpWriter<T> CreateShpWriter(Stream shpStream, Stream shxStream);


        /// <summary>
        /// Wrties geometry and attributes into underlying SHP and DBF files.
        /// Attribute values must be set using Value property of DbfFiled(s) provided during initialization.
        /// </summary>
        public override void Write()
        {
            ShpWriter.Write();
            DbfWriter.Write();
        }

        /// <inheritdoc/>
        public override void Write(IFeature feature)
        {
            foreach (var field in DbfWriter.Fields)
            {
                if (feature.Attributes?.Exists(field.Name) == true)
                {
                    field.Value = feature.Attributes[field.Name];
                }
            }
            Geometry = ShpWriter.GetShapeGeometry(feature.Geometry);
            Write();
        }

        /// <inheritdoc/>
        protected override void DisposeManagedResources()
        {
            ShpWriter?.Dispose();
            DbfWriter?.Dispose();

            base.DisposeManagedResources();  // This will dispose streams used by ShpWriter and DbfWriter. Do it at the end.
        }
    }


}
