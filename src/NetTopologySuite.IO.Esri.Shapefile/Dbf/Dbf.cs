using System.Text;

namespace NetTopologySuite.IO.Esri.Dbf
{
    /// <summary>
    /// Manages configurations and constants specific to the structure and operation of DBF files in the dBASE III format.
    /// </summary>
    internal static class Dbf
    {
        internal readonly static int TableDescriptorSize = 32; // Number of bytes in the table header 

        internal readonly static int FieldDescriptorSize = 32; // Number of bytes in the field descriptor
        internal readonly static int MaxFieldCount = 255;
        internal readonly static byte Dbase3Version = 0x03;      // dBASE III
        internal readonly static byte HeaderTerminatorMark = 0x0D;

        internal readonly static byte DeletedRecordMark = 0x2A; // '*'
        internal readonly static byte ValidRecordMark = 0x20;   // ' '
        internal readonly static byte EndOfFileMark = 0x1A;

        internal static readonly int MaxFieldNameLength = 10;

        internal readonly static Encoding DefaultEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    }
}
