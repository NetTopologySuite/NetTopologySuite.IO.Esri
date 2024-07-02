using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Dbf.Fields
{

    /// <summary>
    /// Date field definition.
    /// </summary>
    public class DbfDateField : DbfField
    {
        private const int FieldLength = 8;           // This width is fixed and cannot be changed
        private static readonly string DateFormat = "yyyyMMdd";

        /// <summary>
        ///  Initializes a new instance of the field class.
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <param name="length">Field length.</param>
        public DbfDateField(string name, int length = FieldLength)
            : base(name, DbfType.Date, length, 0)
        {
        }

        /// <summary>
        /// Date representation of current field value.
        /// </summary>
        public DateTime? DateValue { get; set; }

        /// <inheritdoc/>
        public override object Value
        {
            get { return DateValue; }
            set { DateValue = (DateTime?)value; }
        }

        /// <inheritdoc/>
        public override bool IsNull => DateValue == null;


        internal override void ReadValue(Stream stream)
        {
            var valueText = stream.ReadString(Length, Encoding.ASCII)?.Trim();
            if (string.IsNullOrEmpty(valueText) || valueText == "00000000")
            {
                DateValue = null;
            }
            else
            {
                DateValue = DateTime.ParseExact(valueText, DateFormat, CultureInfo.InvariantCulture);
            }
        }

        internal override void WriteValue(Stream stream)
        {
            if (DateValue.HasValue)
            {
                stream.WriteString(DateValue.Value.ToString(DateFormat, CultureInfo.InvariantCulture), Length, Encoding.ASCII);
            }
            else
            {
                // ArcMap 10.6 can create different null date representation in one .shp file!
                // My test file pt_utf8.shp have field named 'date' with such binary data:
                // === record 0     Stream.Position: 673
                // date    BinaryBuffer.Position: 183
                // ReadString(191): '▬▬▬▬▬▬▬▬'                  // '▬' == char.MinValue == (char)0 
                // === record 1     Stream.Position: 1145
                // date    BinaryBuffer.Position: 183
                // ReadString(191): '        '

                // Some libraries, instead of zeros (null) values use '00000000', so the #48 ASCII code used for represening zero.

                // According to https://desktop.arcgis.com/en/arcmap/latest/manage-data/shapefiles/geoprocessing-considerations-for-shapefile-output.htm
                // Null value substitution for Date field is 'Stored as zero'. Storing zero (null) values is also consistent with Numeric and Float field. 

                //stream.WriteBytes((byte)' ', Length);
                stream.WriteNullBytes(Length);
            }
        }
    }


}
