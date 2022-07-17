using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NetTopologySuite.IO.Esri.Dbf.Fields
{
    /// <summary>
    /// Int64 field definition.
    /// </summary>
    public class DbfNumericDoubleField : DbfNumericField<double>
    {
        internal DbfNumericDoubleField(string name, int length, int precision)
            : base(name, DbfType.Numeric, length, precision)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the field class.
        /// </summary>
        /// <param name="name">Field name.</param>
        public DbfNumericDoubleField(string name) : this(name, MaxFieldLength, MaxFieldPrecision)
        {
        }

        /// <inheritdoc/>
        protected override double StringToNumber(string s)
        {
            return double.Parse(s, CultureInfo.InvariantCulture);
        }
    }
}
