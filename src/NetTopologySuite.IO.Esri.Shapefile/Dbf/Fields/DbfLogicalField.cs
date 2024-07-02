using System;
using System.IO;
using System.Linq;

namespace NetTopologySuite.IO.Esri.Dbf.Fields
{
    /// <summary>
    /// Logical field definition.
    /// </summary>
    public class DbfLogicalField : DbfField
    {
        private readonly static byte DefaultValue = (byte)' '; // (byte)'?'; Initialized to 0x20 (space) otherwise T or F (http://www.dbase.com/KnowledgeBase/int/db7_file_fmt.htm)
        private readonly static byte TrueValue = (byte)'T';
        private readonly static byte FalseValue = (byte)'F';
        private const int FieldLength = 1;        // This width is fixed and cannot be changed

        private readonly static string TrueValues = "TtYy";
        private readonly static string FalseValues = "FfNn";


        /// <summary>
        ///  Initializes a new instance of the field class.
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <param name="length">Field length.</param>
        public DbfLogicalField(string name, int length = FieldLength)
            : base(name, DbfType.Logical, length, 0)
        {
        }

        /// <summary>
        /// Logical representation of current field value.
        /// </summary>
        public bool? LogicalValue { get; set; }

        /// <inheritdoc/>
        public override object Value
        {
            get { return LogicalValue; }
            set { LogicalValue = (bool?)value; }
        }

        /// <inheritdoc/>
        public override bool IsNull => LogicalValue == null;

        internal override void ReadValue(Stream stream)
        {
            var logicalValue = stream.ReadByteChar();
            stream.Seek(stream.Position + Length - 1, SeekOrigin.Begin);
            if (TrueValues.Contains(logicalValue))
            {
                LogicalValue = true;
            }
            else if (FalseValues.Contains(logicalValue))
            {
                LogicalValue = false;
            }
            else
            {
                LogicalValue = null;
            }
        }


        internal override void WriteValue(Stream stream)
        {
            if (!LogicalValue.HasValue)
            {
                stream.WriteByte(DefaultValue);
            }
            else if (LogicalValue.Value)
            {
                stream.WriteByte(TrueValue);
            }
            else
            {
                stream.WriteByte(FalseValue);
            }
        }

    }



}
