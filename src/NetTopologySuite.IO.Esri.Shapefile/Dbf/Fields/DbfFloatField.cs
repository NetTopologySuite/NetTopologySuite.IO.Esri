using System;
using System.Globalization;

namespace NetTopologySuite.IO.Esri.Dbf.Fields
{

    /// <summary>
    /// Numeric field definition.
    /// </summary>
    public class DbfFloatField : DbfNumericField<double>
    {
        // Exponential notation requires 8 additional digits:
        //      1 digit for plus/minus mark     "-"
        //      1 digit for first number digit  "1"
        //      1 digit for decimal separator   "."
        //      5 digits for exponent component "e+004"
        // -1.2346e+004 => Length: 12, DecimalDigits: 4   => 12 - 4 = 8
        private static readonly int ExponentialNotationDigitCount = 8;

        private static readonly int DefaultFieldLength = 19;    // That uses ArcMap 10.6 when creates 'Double' field. 
        private static readonly int DefaultFieldPrecision = 11; // That uses ArcMap 10.6 when creates 'Double' field.       

        private readonly string NumberFormat;


        /// <summary>
        ///  Initializes a new instance of the field class.
        /// </summary>
        /// <param name="name">Field name.</param>
        public DbfFloatField(string name) : this(name, DefaultFieldLength, DefaultFieldPrecision)
        {
        }

        internal DbfFloatField(string name, int length, int precision)
            : base(name, DbfType.Float, length, precision)
        {
            // Esri uses exponential notation for float fields:
            // -12345.6789 (E10) => -1.2345678900E+004
            // -12345.6789 (e4)  => -1.2346e+004

            // Esri writes specific precision to field definition but then ignores it by using exponential notation.
            // Exponential notation expresses numbers placing decimal separator always after first non-zero digit.
            NumberFormat = "e" + (Length - ExponentialNotationDigitCount).ToString();
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
                    NumericValue = Convert.ToDouble(value);
                }
            }
        }

        /// <inheritdoc/>
        protected override double StringToNumber(string number)
        {
            return double.Parse(number, CultureInfo.InvariantCulture);
        }

        /// <inheritdoc/>
        protected override string NumberToString(double number)
        {
            var numberString = number.ToString(NumberFormat, CultureInfo.InvariantCulture);
#if DEBUG
            // Esri uses two digits in exponential component "e+01" and ads one space at the begining.          " -1.11111000000e+02"
            // .NET always uses three digits in exponential component "e+001" (there is no space for space).    "-1.11111000000e+002"
            // Do it for testing purposes in order to make every byte exactly the same as Esri's output.
            numberString = numberString.Replace("e+0", "e+").Replace("e-0", "e-");
#endif
            return numberString;
        }
    }


}
