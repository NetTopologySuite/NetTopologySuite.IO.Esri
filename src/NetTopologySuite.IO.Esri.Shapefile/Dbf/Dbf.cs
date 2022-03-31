namespace NetTopologySuite.IO.Esri.Dbf
{
    internal static class Dbf
    {
        public readonly static int TableDescriptorSize = 32; // Number of bytes in the table header 

        internal readonly static int FieldDescriptorSize = 32; // Number of bytes in the field descriptor
        internal readonly static int MaxFieldCount = 255;
        public readonly static byte Dbase3Version = 0x03;      // dBASE III
        public readonly static byte HeaderTerminatorMark = 0x0D;

        public readonly static byte DeletedRecordMark = 0x2A; // '*'
        public readonly static byte ValidRecordMark = 0x20;   // ' '
        public readonly static byte EndOfFileMark = 0x1A;

        public static readonly int MaxFieldNameLength = 10;
    }
}
