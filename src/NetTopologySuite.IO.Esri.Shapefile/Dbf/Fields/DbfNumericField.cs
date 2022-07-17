using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Dbf.Fields
{

    /// <summary>
    /// Numeric field definition.
    /// </summary>
    public abstract class DbfNumericField : DbfField
    {
        internal static readonly int MaxFieldLength = 19;    // Decimal point and any sign is included in field width, if present. 
        internal static readonly int MaxFieldPrecision = 17; // dBASE specs states it could be max 15, but Esri uses up to 17.

        internal DbfNumericField(string name, DbfType type, int length, int precision)
            : base(name, type, length, precision)
        {
            // -0.12345 => legth: 8, decimalCount: 5 => should be (length - 3). But Esri allows decimalCount: 6 => (length - 2).
        }

        internal static DbfField Create(string name, int length, int precision)
        {
            // Consider writing a field based on `Byte` value.
            // Such field would get the length=3 (Byte.MaxValue=255). 
            // Now consider you are reading the the same field.
            // From the DBF point of view you know only that it has length=3 (and precision=0).
            // What type would you use for such field?
            // - The `Byte` type? (because it's you who create that field)
            // - The `Int16` type? (because in the meantime one was able to write there 999 or even -3)

            // For above reasons use only very basic numeric types - Int32, Int64 and Double.

            if (precision <= 0 && length <= DbfNumericInt32Field.DefaultFieldLength)
            {
                return new DbfNumericInt32Field(name, length);
            }

            if (precision <= 0)
            {
                return new DbfNumericInt64Field(name, length);
            }

            return new DbfNumericDoubleField(name, length, precision);
        }

    }

    /// <summary>
    /// Numeric field definition.
    /// </summary>
    public abstract class DbfNumericField<T> : DbfNumericField where T : struct, IConvertible, IFormattable
    {
        private readonly string NumberFormat;

        internal DbfNumericField(string name, DbfType type, int length, int precision)
            : base(name, type, length, precision)
        {
            NumberFormat = precision > 0
                ? "0." + new string('#', precision)
                : "0";
        }

        /// <summary>
        /// Numeric representation of current field value.
        /// </summary>
        public T? NumericValue { get; set; }

        /// <inheritdoc/>
        public override object Value
        {
            get { return NumericValue; }
            set
            {
                if (value == null)
                {
                    NumericValue = null;
                }
                else
                {
                    NumericValue = (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
                }
            }
        }

        /// <inheritdoc/>
        public override bool IsNull => NumericValue == null;

        internal override void ReadValue(Stream stream)
        {
            var valueText = stream.ReadString(Length, Encoding.ASCII)?.Trim();
            if (string.IsNullOrEmpty(valueText))
            {
                NumericValue = null;
                return;
            }

            // https://desktop.arcgis.com/en/arcmap/latest/manage-data/shapefiles/geoprocessing-considerations-for-shapefile-output.htm
            // Null values are not supported in shapefiles. If a feature class containing nulls is converted to a shapefile, or a database table is converted to a dBASE file, the null values will be changed:
            // Number: -1.7976931348623158e+308 (IEEE standard for the maximum negative value)

            NumericValue = StringToNumber(valueText);
        }

        internal override void WriteValue(Stream stream)
        {
            if (!NumericValue.HasValue)
            {
                stream.WriteNullBytes(Length);
                return;
            }

            var valueText = NumberToString(NumericValue.Value);
            if (valueText.Length > this.Length)
            {
                throw GetFieldValueError(valueText, "Field value out of range.");
            }

            // Length: 4
            // 1.2 => " 1.2";
            if (valueText.Length < this.Length)
            {
                valueText = valueText.PadLeft(this.Length, ' '); // PadLeft for Values
            }

            stream.WriteString(valueText, Length, Encoding.ASCII);
        }


        /// <summary>
        /// Converts the number to its string representation.
        /// </summary>
        /// <param name="number">Number to convert.</param>
        /// <returns>A string that is equivalent to the specified numer.</returns>
        protected virtual string NumberToString(T number)
        {
            var valueString = number.ToString(NumberFormat, CultureInfo.InvariantCulture);

            var decSepPos = valueString.IndexOf('.');

            // Length: 4
            // 12345.6 
            if (decSepPos >= Length)
            {
                throw GetFieldValueError(valueString, "Field value out of range.");
            }

            // Length: 4
            // 123456
            if (decSepPos < 0 && valueString.Length > Length)
            {
                throw GetFieldValueError(valueString, "Field value out of range.");
            }

            if (valueString.Length > Length)
            {
                valueString = valueString.Substring(0, Length);
            }

            return valueString;
        }

        /// <summary>
        /// Converts the string representation of a number to its number equivalent.
        /// </summary>
        /// <param name="s">A string that contains a number to convert.</param>
        /// <returns>A number that is equivalent to the specified string representation of numeric value.</returns>
        protected abstract T StringToNumber(string s);

    }


}
