using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NetTopologySuite.IO.Esri.Dbf.Fields
{
    /// <summary>
    /// Int32 field definition.
    /// </summary>
    public class DbfNumericInt32Field : DbfNumericField<int>
    {
        internal static readonly int DefaultFieldLength = 10; //-2147483648..2147483647 => 11, but Esri uses 10

        /// <summary>
        /// Intializes new instance of the numerif field class.
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <param name="length">The number of digits.</param>
        public DbfNumericInt32Field(string name, int length)
            : base(name, DbfType.Numeric, length, 0)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the field class.
        /// </summary>
        /// <param name="name">Field name.</param>
        public DbfNumericInt32Field(string name) : this(name,  DefaultFieldLength)
        {
        }

        /// <inheritdoc/>
        protected override int StringToNumber(string s)
        {
            return int.Parse(s, CultureInfo.InvariantCulture);
        }
    }
}
