using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Dbf;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shp.Writers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Shapefiles.Writers
{

    /// <summary>
    /// Generic base class for writing a shapefile.
    /// </summary>
    public abstract class ShapefileWriter<T> : ShapefileWriter where T : Geometry
    {
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
        /// <param name="type">Shape type.</param>
        /// <param name="fields">Shapefile fields definitions.</param>
        /// <param name="encoding">DBF file encoding. If null encoding will be guess from related .CPG file or from reserved DBF bytes.</param>
        internal ShapefileWriter(Stream shpStream, Stream shxStream, Stream dbfStream, ShapeType type, IReadOnlyList<DbfField> fields, Encoding encoding)
            : base(new DbfWriter(dbfStream, fields, encoding))
        {
            ShapeType = type;
            ShpWriter = CreateShpWriter(shpStream, shxStream);
        }


        /// <summary>
        /// Initializes a new instance of the writer class.
        /// </summary>
        /// <param name="shpPath">Path to SHP file.</param>
        /// <param name="type">Shape type.</param>
        /// <param name="fields">Shapefile attribute definitions.</param>
        /// <param name="encoding">DBF file encoding. If null encoding will be guess from related .CPG file or from reserved DBF bytes.</param>
        /// <param name="projection">Projection metadata for the shapefile (.prj file).</param>
        internal ShapefileWriter(string shpPath, ShapeType type, IReadOnlyList<DbfField> fields, Encoding encoding, string projection)
            : base(new DbfWriter(Path.ChangeExtension(shpPath, ".dbf"), fields, encoding))
        {
            try
            {
                var shpStream = OpenManagedFileStream(shpPath, ".shp", FileMode.Create);
                var shxStream = OpenManagedFileStream(shpPath, ".shx", FileMode.Create);

                ShapeType = type;
                ShpWriter = CreateShpWriter(shpStream, shxStream); // It calls this.ShapeType

                if (!string.IsNullOrWhiteSpace(projection))
                    File.WriteAllText(Path.ChangeExtension(shpPath, ".prj"), projection);
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
        /// <param name="type">Shape type.</param>
        /// <param name="fields">Shapefile attribute definitions.</param>
        internal ShapefileWriter(string shpPath, ShapeType type, params DbfField[] fields) : this(shpPath, type, fields, null, null)
        {
        }

        internal abstract ShpWriter<T> CreateShpWriter(Stream shpStream, Stream shxStream);


        /// <summary>
        /// Wrties geometry and attributes into underlying SHP and DBF files.
        /// Attribute values must be set using Value property of DbfFiled(s) provided during initialization.
        /// </summary>
        public void Write()
        {
            ShpWriter.Write();
            DbfWriter.Write();
        }

        /// <inheritdoc/>
        public override void Write(IFeature feature)
        {
            foreach (var field in DbfWriter.Fields)
            {
                field.Value = feature.Attributes[field.Name];
            }
            Geometry = (T)feature.Geometry;
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
