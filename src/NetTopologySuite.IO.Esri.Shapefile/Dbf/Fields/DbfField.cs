using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace NetTopologySuite.IO.Esri.Dbf.Fields
{


    /// <summary>
    /// dBASE field definition.
    /// </summary>
    public abstract class DbfField
    {
        // TODO: Spit DbfField into DbfFieldDefinition (Without Value property) and DbfFieldValue

        /// <summary>
        /// Field Name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Field Type.
        /// </summary>
        public DbfType FieldType { get; }

        /// <summary>
        /// Length of the data in bytes or total number of digits in the numeric value.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Number of digits after the decimal point in the numeric value.
        /// </summary>
        public int NumericScale { get; }


        /// <summary>
        /// Initializes a new instance of the field class.
        /// </summary>
        /// <param name="name">Field name (max 10 characters).</param>
        /// <param name="type">Field type.</param>
        /// <param name="length">Field length.</param>
        /// <param name="precision">Decimal places count.</param>
        internal DbfField(string name, DbfType type, int length, int precision)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Empty dBASE field name.", nameof(name));

            if (name.Length > Dbf.MaxFieldNameLength)
                throw new ArgumentException($"dBASE III field name cannot be longer than {Dbf.MaxFieldNameLength} characters.", nameof(name));

            // ArcMap does support number at the begining.
            //var beginsWithLetter = IsValidFieldNameLetter(name[0]);
            //if (!beginsWithLetter)
            //    throw new ArgumentNullException($"Invalid dBASE field name: {name}. Field name must begin with a letter.", nameof(name));


            /*
            foreach (var c in name)
            {
                if (!IsValidFieldNameChar(c))
                    throw new ArgumentNullException($"Invalid dBASE field name: {name}. Field name must contain only letter, number or undersocre (_) character.", nameof(name));
            }
            */

            if (length < 1)
                throw new ArgumentException($"Ivalid dBASE field length: {length}.", nameof(length));

            if (precision < 0)
                precision = 0; // throw new ArgumentException($"Ivalid dBASE III field decimal places count: {precision}.", nameof(precision));

            Name = name;
            FieldType = type;
            Length = length;
            NumericScale = precision;
        }

        private bool IsValidFieldNameChar(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || (c >= '0' && c <= '9');
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0} | {1} ({2}, {3})", Name.PadLeft(10), FieldType, Length, NumericScale);
        }

        internal Exception GetFieldValueError(object value, string additionalDescription = "")
        {
            return new ArgumentException($"Invalid {Name} [{FieldType}:{Length}] field value: {value}." + additionalDescription);
        }

        internal abstract void ReadValue(Stream stream);
        internal abstract void WriteValue(Stream stream);

        /// <summary>
        /// Current field value.
        /// </summary>
        /// <remarks>
        /// <see cref="Value"/> is used by shapefile readers and writers to hold current record field value.
        /// </remarks>
        public abstract object Value { get; set; }



        internal static readonly IReadOnlyDictionary<string, object> EmptyFieldValues = new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());

        /// <summary>
        /// Creates dBASE field determined using specified value. 
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <param name="type">Field value type.</param>
        /// <returns>dBase field definition.</returns>
        public static DbfField Create(string name, Type type)
        {
            if (type == null)
                throw new ArgumentException("Cannot determine dBASE field type for <null> value.");

            if (type == typeof(string))
                return new DbfCharacterField(name);

            if (type == typeof(bool) || type == typeof(bool?))
                return new DbfLogicalField(name);

            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return new DbfDateField(name);

            if (type == typeof(sbyte) || type == typeof(sbyte?) || type == typeof(byte) || type == typeof(byte?))
                return new DbfNumericField(name, 4, 0);

            if (type == typeof(short) || type == typeof(short?) || type == typeof(ushort) || type == typeof(ushort?))
                return new DbfNumericField(name, 6, 0);

            if (type == typeof(int) || type == typeof(int?) || type == typeof(uint) || type == typeof(uint?))
                return new DbfNumericField(name, 11, 0);

            if (type == typeof(long) || type == typeof(long?) || type == typeof(ulong) || type == typeof(ulong?))
                return new DbfNumericField(name, DbfNumericField.MaxFieldLength, 0);

            if (type == typeof(decimal) || type == typeof(decimal?))
                return new DbfNumericField(name, DbfNumericField.MaxFieldLength, DbfNumericField.MaxFieldPrecision);

            if (type == typeof(double) || type == typeof(float) || type == typeof(double?) || type == typeof(float?))
                return new DbfFloatField(name);

            throw new ArgumentException($"Unsupported dBASE field value: {type} ({type.GetType().Name})");
        }

    }

}
