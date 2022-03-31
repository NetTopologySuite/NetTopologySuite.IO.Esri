using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NetTopologySuite.IO.Esri.Dbf.Fields
{
    /// <summary>
    /// Collection of dBASE field definitions.
    /// </summary>
    public class DbfFieldCollection : IReadOnlyList<DbfField>
    {
        internal DbfFieldCollection(int count)
        {
            Fields = new List<DbfField>(count);
        }

        private readonly List<DbfField> Fields;
        private readonly Dictionary<string, DbfField> FieldDictionary = new Dictionary<string, DbfField>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the field at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the field to get.</param>
        /// <returns> The field at the specified index.</returns>
        public DbfField this[int index] => Fields[index];

        /// <summary>
        /// Gets the field with specified name.
        /// </summary>
        /// <param name="name">The of the field.</param>
        /// <returns>The field with specified name or null if name was not found.</returns>
        public DbfField this[string name]
        {
            get
            {
                if (FieldDictionary.TryGetValue(name, out var field))
                    return field;
                return null;
            }
        }


        /// <summary>
        /// Gets the number of fields contained in the collection.
        /// </summary>
        public int Count => Fields.Count;

        IEnumerator<DbfField> IEnumerable<DbfField>.GetEnumerator()
        {
            return Fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Fields.GetEnumerator();
        }

        internal void Add(DbfField field)
        {
            Fields.Add(field);
            FieldDictionary.Add(field.Name, field);
        }

        /// <summary>
        /// Reads current fields values.
        /// </summary>
        /// <returns>Array of field values.</returns>
        public IReadOnlyDictionary<string, object> GetValues()
        {
            var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var field in Fields)
            {
                values[field.Name] = field.Value;
            }
            return new ReadOnlyDictionary<string, object>(values);
        }

        /// <summary>
        /// Sets current filed values.
        /// </summary>
        /// <param name="values"></param>
        public void SetValues(IReadOnlyDictionary<string, object> values)
        {
            if (values == null)
                throw new ArgumentNullException("dBASE record must contain values.", nameof(values));

            if (values.Count < Fields.Count)
                throw new ArgumentNullException("Invalid dBASE record value count: " + values.Count + ". Expected: " + Fields.Count, nameof(values));

            foreach (var field in Fields)
            {
                if (values.TryGetValue(field.Name, out var value))
                {
                    field.Value = value;
                }
                else
                {
                    throw new ArgumentException("Required dBASE attribute value is missing for '" + field.Name + "' field.");
                }
            }
        }
    }
}
