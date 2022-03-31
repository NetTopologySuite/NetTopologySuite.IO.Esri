using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Dbf.Fields
{

    /// <summary>
    /// Numeric field definition.
    /// </summary>
    public abstract class DbfNumericField<T> : DbfField where T : struct, IConvertible, IFormattable
    {
        internal static readonly int MaxFieldLength = 19;    // Decimal point and any sign is included in field width, if present. 
        internal static readonly int MaxFieldPrecision = 17; // dBASE specs states it could be max 15, but Esri uses up to 17.

        internal DbfNumericField(string name, DbfType type, int length, int precision)
            : base(name, type, GetProperLength(length), GetProperPrecision(length, precision))
        {
        }

        /// <summary>
        ///  Initializes a new instance of the field class.
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <param name="length">Field length.</param>
        /// <param name="precision">Decmial places count.</param>
        protected DbfNumericField(string name, int length, int precision)
            : this(name, DbfType.Numeric, length, precision)
        {

        }

        /// <summary>
        /// Numeric representation of current field value.
        /// </summary>
        public T? NumericValue { get; set; }

        internal static int GetProperLength(int length)
        {
            if (length < 1)
                return 1;
            return Math.Min(length, MaxFieldLength);
        }

        private static int GetProperPrecision(int length, int precision)
        {
            if (precision < 0)
                return 0;

            length = GetProperLength(length);
            if (length < 2)
                return 0;

            precision = Math.Min(precision, length - 2);  // -0.12345 => legth: 8, decimalCount: 5 => should be (length - 3). But Esri allows decimalCount: 6 => (length - 2).
            return Math.Min(precision, MaxFieldPrecision);
        }

        internal override void ReadValue(Stream stream)
        {
            var valueText = stream.ReadString(Length, Encoding.ASCII)?.Trim();
            if (string.IsNullOrEmpty(valueText))
            {
                NumericValue = null;
                return;
            }

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

            // Length: 4
            // 1.2 => " 1.2";
            if (valueText.Length < this.Length)
            {
                stream.WriteString(valueText.PadLeft(this.Length, ' '), Length, Encoding.ASCII); // PadLeft for Values
            }
            else
            {
                if (valueText.Length > this.Length)
                {
                    throw GetFieldValueError(valueText, "Field value out of range.");
                }
                stream.WriteString(valueText, Length, Encoding.ASCII);
            }
        }


        /// <summary>
        /// Converts the number to its string representation.
        /// </summary>
        /// <param name="number">Number to convert.</param>
        /// <returns>A string that is equivalent to the specified numer.</returns>
        protected abstract string NumberToString(T number);

        /// <summary>
        /// Converts the string representation of a number to its number equivalent.
        /// </summary>
        /// <param name="number">A string that contains a number to convert.</param>
        /// <returns>A number that is equivalent to the specified string representation of numeric value.</returns>
        protected abstract T StringToNumber(string number);
    }



    /// <inheritdoc/>
    public class DbfNumericField : DbfNumericField<decimal>
    {
        internal static readonly int MaxInt32FieldLength = int.MaxValue.ToString().Length; // 10  (int.MinValue => -2147483648 => 11, but Esri uses 10)
        private readonly string NumberFormat = "0"; // For integers

        /// <inheritdoc/>
        public DbfNumericField(string name, int length, int precision)
            : base(name, DbfType.Numeric, length, precision)
        {
            if (NumericScale > 0)
                NumberFormat = "0." + new string('0', NumericScale);
        }

        /// <summary>
        ///  Initializes a new instance of the integer field class.
        /// </summary>
        /// <param name="name">Field name.</param>
        public DbfNumericField(string name)
            : this(name, MaxInt32FieldLength, 0)
        {
        }

        /// <inheritdoc/>
        protected override decimal StringToNumber(string number)
        {
            // https://desktop.arcgis.com/en/arcmap/latest/manage-data/shapefiles/geoprocessing-considerations-for-shapefile-output.htm
            // Null values are not supported in shapefiles. If a feature class containing nulls is converted to a shapefile, or a database table is converted to a dBASE file, the null values will be changed:
            // Number: -1.7976931348623158e+308 (IEEE standard for the maximum negative value)

            return decimal.Parse(number, CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        protected override string NumberToString(decimal number)
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
                    NumericValue = Convert.ToDecimal(value);
                }
            }
        }

        /// <summary>
        /// <see cref="int"/> representation of field value.
        /// </summary>
        public int? Int32Value
        {
            get => ConvertValue(Convert.ToInt32);
            set => NumericValue = ConvertToDecimal(value);
        }

        /// <summary>
        /// <see cref="double"/> representation of field value.
        /// </summary>
        public double? DoubleValue
        {
            get => ConvertValue(Convert.ToDouble);
            set => NumericValue = ConvertToDecimal(value);
        }

        private T? ConvertValue<T>(Func<decimal, T> converTo) where T : struct
        {
            if (NumericValue.HasValue)
                return converTo(NumericValue.Value);
            else
                return null;
        }

        private decimal? ConvertToDecimal(IConvertible value)
        {
            if (value == null)
                return null;
            else
                return value.ToDecimal(CultureInfo.InvariantCulture);
        }
    }


}
