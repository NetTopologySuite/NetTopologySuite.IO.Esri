using System;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Dbf.Fields
{

    /// <summary>
    /// Character field definition.
    /// </summary>
    public class DbfCharacterField : DbfField
    {
        // DBASE Limits -> Size of character fields: 254 (https://www.dbase.com/Knowledgebase/faq/dBASE_Limits_FAQ.html)
        // DBF limitations inherited by ArcIMS -> The maximum size of a string (or text) field in a .DBF file is 254 (https://support.esri.com/en-us/knowledge-base/what-are-some-of-the-dbf-limitations-inherited-by-arcim-000007920)
        private const int MaxFieldLength = 254;

        /// <summary>
        ///  Initializes a new instance of the field class.
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <param name="length">Field length.</param>
        public DbfCharacterField(string name, int length = MaxFieldLength) : this(name, length, null)
        {
        }

        internal DbfCharacterField(string name, int length, Encoding encoding)
            : base(name, DbfType.Character, length, 0)
        {
            _encoding = encoding;
        }

        /// <summary>
        /// String representation of current field value.
        /// </summary>
        public string StringValue { get; set; }

        /// <inheritdoc/>
        public override object Value
        {
            get { return StringValue; }
            set { StringValue = value?.ToString(); }
        }

        /// <inheritdoc/>
        public override bool IsNull => StringValue == null;

        internal override void ReadValue(Stream stream)
        {
            StringValue = stream.ReadString(Length, Encoding)?.TrimEnd();
        }

        internal override void WriteValue(Stream stream)
        {
            if (StringValue == null)
            {
                stream.WriteNullBytes(Length);
                //stream.WriteBytes((byte)' ', Length);
            }
            else if (StringValue.Length < this.Length)
            {
                // Length: 4
                // 1.2 => "1.2 ";
                stream.WriteString(StringValue.PadRight(this.Length, ' '), Length, Encoding); // PadRigth for text
            }
            else
            {
                stream.WriteString(StringValue, Length, Encoding);
            }
        }

        private Encoding _encoding = null;
        internal Encoding Encoding
        {
            get { return _encoding ?? Dbf.DefaultEncoding; }
            set
            {
                if (value == null)
                    throw new ArgumentException($"Field {GetType().Name} cannot have unassigned encoding.");

                if (_encoding != null && _encoding.CodePage != value.CodePage)
                    throw new ArgumentException($"Cannot change field {Name} ecnoding. It is already assinged to other {nameof(DbfWriter)} with different encoding.");

                _encoding = value;
            }
        }
    }

}
