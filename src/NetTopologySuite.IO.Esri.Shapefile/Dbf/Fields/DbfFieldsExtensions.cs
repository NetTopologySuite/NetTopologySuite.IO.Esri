using System;
using System.Collections.Generic;
using System.Text;

namespace NetTopologySuite.IO.Esri.Dbf.Fields
{
    /// <summary>
    /// DbfField collection extensions.
    /// </summary>
    public static class DbfFieldsExtensions
    {
        /// <summary>
        /// Adds a DBF field.
        /// </summary>
        /// <param name="fields">The field list.</param>
        /// <param name="name">Field name.</param>
        /// <param name="type">Field type.</param>
        /// <returns></returns>
        public static DbfField AddField(this ICollection<DbfField> fields, string name, Type type)
        {
            var field = DbfField.Create(name, type);
            fields.Add(field);
            return field;
        }

        /// <summary>
        /// Adds a DBF field.
        /// </summary>
        /// <param name="fields">The field list.</param>
        /// <param name="name">Field name.</param>
        /// <returns></returns>
        public static DbfField AddField<T>(this ICollection<DbfField> fields, string name)
            where T : struct, IComparable, IConvertible, IFormattable
        {
            return AddField(fields, name, typeof(T));
        }

        /// <summary>
        /// Adds a character string field.
        /// </summary>
        /// <param name="fields">The field list.</param>
        /// <param name="name">Field name.</param>
        /// <param name="length">Field lenght.</param>
        /// <returns></returns>
        public static DbfCharacterField AddCharacterField(this ICollection<DbfField> fields, string name, int length = 254)
        {
            var field = new DbfCharacterField(name, length);
            fields.Add(field);
            return field;
        }

        /// <summary>
        /// Adds a date field.
        /// </summary>
        /// <param name="fields">The field list.</param>
        /// <param name="name">Field name.</param>
        /// <returns></returns>
        public static DbfDateField AddDateField(this ICollection<DbfField> fields, string name)
        {
            var field = new DbfDateField(name);
            fields.Add(field);
            return field;
        }

        /// <summary>
        /// Adds a logical field.
        /// </summary>
        /// <param name="fields">The field list.</param>
        /// <param name="name">Field name.</param>
        /// <returns></returns>
        public static DbfLogicalField AddLogicalField(this ICollection<DbfField> fields, string name)
        {
            var field = new DbfLogicalField(name);
            fields.Add(field);
            return field;
        }

        /// <summary>
        /// Adds an Int32 field.
        /// </summary>
        /// <param name="fields">The field list.</param>
        /// <param name="name">Field name.</param>
        /// <returns></returns>
        public static DbfNumericInt32Field AddNumericInt32Field(this ICollection<DbfField> fields, string name)
        {
            var field = new DbfNumericInt32Field(name);
            fields.Add(field);
            return field;
        }

        /// <summary>
        /// Adds an Int64 field.
        /// </summary>
        /// <param name="fields">The field list.</param>
        /// <param name="name">Field name.</param>
        /// <returns></returns>
        public static DbfNumericInt64Field AddNumericInt64Field(this ICollection<DbfField> fields, string name)
        {
            var field = new DbfNumericInt64Field(name);
            fields.Add(field);
            return field;
        }

        /// <summary>
        /// Adds a Double field.
        /// </summary>
        /// <param name="fields">The field list.</param>
        /// <param name="name">Field name.</param>
        /// <returns></returns>
        public static DbfNumericDoubleField AddNumericDoubleField(this ICollection<DbfField> fields, string name)
        {
            var field = new DbfNumericDoubleField(name);
            fields.Add(field);
            return field;
        }

        /// <summary>
        /// Adds a float field.
        /// </summary>
        /// <param name="fields">The field list.</param>
        /// <param name="name">Field name.</param>
        /// <returns></returns>
        public static DbfFloatField AddFloatField(this ICollection<DbfField> fields, string name)
        {
            var field = new DbfFloatField(name);
            fields.Add(field);
            return field;
        }
    }
}
