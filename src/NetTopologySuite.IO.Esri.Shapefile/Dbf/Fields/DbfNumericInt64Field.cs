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
        internal static readonly int DefaultFieldLength = 19; // -9223372036854775808 to 9223372036854775807 (20..19 digits)

        /// <summary>
        /// Intializes new instance of the numerif field class.
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <param name="length">The number of digits.</param>
        public DbfNumericInt64Field(string name, int length)
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
