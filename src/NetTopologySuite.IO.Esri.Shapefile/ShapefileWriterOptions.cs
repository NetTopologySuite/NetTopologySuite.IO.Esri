using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetTopologySuite.IO.Esri
{
    /// <summary>
    ///  Shapefile writer options
    /// </summary>
    public class ShapefileWriterOptions
    {
        /// <summary>
        /// Shape type.
        /// </summary>
        public ShapeType ShapeType { get; }

        /// <summary>
        /// Shapefile fields definitions.
        /// </summary>
        public List<DbfField> Fields { get; } = new List<DbfField>();

        private Encoding _encoding = Encoding.UTF8;
        /// <summary>
        /// DBF file encoding.
        /// </summary>
        public Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value ?? Encoding.UTF8;
        }

        /// <summary>
        /// Projection metadata for the shapefile (content of the PRJ file).
        /// </summary>
        public string Projection { get; set; } = null;

        /// <summary>
        /// Creates new instance of ShapefileWriterOptions class.
        /// </summary>
        /// <param name="type">Shape type.</param>
        /// <param name="fields">Shapefile fields definitions.</param>
        public ShapefileWriterOptions(ShapeType type, params DbfField[] fields)
        {
            ShapeType = type;
            if (fields != null)
            {
                Fields.AddRange(fields);
            }
        }

        /// <summary>
        /// Creates new instance of ShapefileWriterOptions class based on existing ShapefileReader.
        /// </summary>
        /// <param name="reader">Shape reader.</param>
        public ShapefileWriterOptions(ShapefileReader reader)
        {
            reader = reader ?? throw new ArgumentNullException(nameof(reader));
            ShapeType = reader.ShapeType;
            Fields.AddRange(reader.Fields);
            Encoding = reader.Encoding;
            Projection = reader.Projection;
        }

        /// <summary>
        /// Adds a DBF field (feature attribute).
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <param name="type">Field type.</param>
        /// <returns></returns>
        public DbfField AddField(string name, Type type)
        {
            return Fields.AddField(name, type);
        }

        /// <summary>
        /// Adds a DBF field.
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <returns></returns>
        public DbfField AddField<T>(string name)
            where T : struct, IComparable, IConvertible, IFormattable
        {
            return Fields.AddField<T>(name);
        }

        /// <summary>
        /// Adds a character string field (feature attribute).
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <param name="length">Field lenght.</param>
        /// <returns></returns>
        public DbfCharacterField AddCharacterField(string name, int length = 254)
        {
            return Fields.AddCharacterField(name, length);
        }

        /// <summary>
        /// Adds a date field (feature attribute).
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <returns></returns>
        public DbfDateField AddDateField(string name)
        {
            return Fields.AddDateField(name);
        }

        /// <summary>
        /// Adds a float field (feature attribute).
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <returns></returns>
        public DbfFloatField AddFloatField(string name)
        {
            return Fields.AddFloatField(name);
        }

        /// <summary>
        /// Adds a logical field (feature attribute).
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <returns></returns>
        public DbfLogicalField AddLogicalField(string name)
        {
            return Fields.AddLogicalField(name);
        }

        /// <summary>
        /// Adds an Int32 field (feature attribute).
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <returns></returns>
        public DbfNumericInt32Field AddNumericInt32Field(string name)
        {
            return Fields.AddNumericInt32Field(name);
        }

        /// <summary>
        /// Adds an Int64 field (feature attribute).
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <returns></returns>
        public DbfNumericInt64Field AddNumericInt64Field(string name)
        {
            return Fields.AddNumericInt64Field(name);
        }

        /// <summary>
        /// Adds a Double field (feature attribute).
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <returns></returns>
        public DbfNumericDoubleField AddNumericDoubleField(string name)
        {
            return Fields.AddNumericDoubleField(name);
        }
    }
}
