using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NetTopologySuite.IO.Esri.Dbf.Fields
{
    /// <summary>
    /// Int64 field definition.
    /// </summary>
    public class DbfNumericInt64Field : DbfNumericField<long>
    {
        internal DbfNumericInt64Field(string name, int length)
            : base(name, DbfType.Numeric, length, 0)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the field class.
        /// </summary>
        /// <param name="name">Field name.</param>
        public DbfNumericInt64Field(string name) : this(name, MaxFieldLength)
        {
        }

        /// <inheritdoc/>
        protected override long StringToNumber(string s)
        {
            return long.Parse(s, CultureInfo.InvariantCulture);
        }
    }
}
