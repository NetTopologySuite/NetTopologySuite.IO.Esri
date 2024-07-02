using NetTopologySuite.IO.Esri.Dbf.Fields;
using System;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri.Dbf
{


    internal static class DbfStreamExtensions
    {
        public static void WriteDbfVersion(this Stream stream, byte version)
        {
            stream.WriteByte(version);
        }
        public static byte ReadDbfVersion(this Stream stream)
        {
            return (byte)stream.ReadByte();
        }


        public static void WriteDbfLastUpdateDate(this Stream stream, DateTime date)
        {
            stream.WriteByte((byte)(date.Year - 1900));
            stream.WriteByte((byte)date.Month);
            stream.WriteByte((byte)date.Day);
        }
        public static DateTime ReadDbfLastUpdateDate(this Stream stream)
        {
            var y = stream.ReadByte();
            var m = stream.ReadByte();
            var d = stream.ReadByte();

            // Handle invalid DBF format too (last update date filled with zeros)
            m = Math.Max(m, 1);
            d = Math.Max(d, 1);

            return new DateTime(1900 + y, m, d);
        }


        public static void WriteDbfRecordCount(this Stream stream, int recordCount)
        {
            stream.WriteUInt32LittleEndian((uint)recordCount);
        }
        public static int ReadDbfRecordCount(this Stream stream)
        {
            return (int)stream.ReadUInt32LittleEndian();
        }


        public static void WriteDbfHeaderSize(this Stream stream, int headerSize)
        {
            stream.WriteUInt16LittleEndian((ushort)headerSize);
        }
        public static ushort ReadDbfHeaderSize(this Stream stream)
        {
            return stream.ReadUInt16LittleEndian();
        }


        public static void WriteDbfRecordSize(this Stream stream, int headerSize)
        {
            stream.WriteUInt16LittleEndian((ushort)headerSize);
        }
        public static ushort ReadDbfRecordSize(this Stream stream)
        {
            return stream.ReadUInt16LittleEndian();
        }


        public static void WriteDbfEncoding(this Stream stream, Encoding encoding)
        {
            var ldid = DbfEncoding.GetLanguageDriverId(encoding);
            stream.WriteByte(ldid);
        }
        public static Encoding ReadDbfEncoding(this Stream stream)
        {
            var ldid = (byte)stream.ReadByte();
            return DbfEncoding.GetEncodingForLanguageDriverId(ldid);
        }


        public static void WriteDbaseFieldDescriptor(this Stream stream, DbfField field, Encoding encoding)
        {
            encoding = encoding ?? Encoding.UTF8;
            var name = field.Name.PadRight(Dbf.MaxFieldNameLength, char.MinValue); // Field name must have empty space zero-filled 


            stream.WriteString(name, Dbf.MaxFieldNameLength, encoding);
            stream.WriteNullBytes(1);
            stream.WriteDbaseType(field.FieldType);
            stream.WriteNullBytes(4);
            stream.WriteByte((byte)field.Length);
            stream.WriteByte((byte)field.NumericScale);
            stream.WriteNullBytes(14);
        }
        public static DbfField ReadDbaseFieldDescriptor(this Stream stream, Encoding encoding)
        {
            encoding = encoding ?? Encoding.UTF8;

            var name = stream.ReadString(Dbf.MaxFieldNameLength, encoding)?.Trim();
            stream.Advance(1); // Reserved (field name terminator)
            var type = stream.ReadDbaseType();
            stream.Advance(4); // Reserved
            var length = stream.ReadByte();
            var precision = stream.ReadByte();
            stream.Advance(14); // Reserved

            if (type == DbfType.Character)
            {
                var textField = new DbfCharacterField(name, length, encoding);
                textField.Encoding = encoding;
                return textField;
            }
            else if (type == DbfType.Date)
            {
                return new DbfDateField(name, length);
            }
            else if (type == DbfType.Numeric)
            {
                return DbfNumericField.Create(name, length, precision);
            }
            else if (type == DbfType.Float)
            {
                return new DbfFloatField(name, length, precision);
            }
            else if (type == DbfType.Logical)
            {
                return new DbfLogicalField(name, length);
            }
            else
            {
                throw new InvalidDataException("Invalid dBASE III field type: " + type);
            }
        }


        private static DbfType ReadDbaseType(this Stream stream)
        {
            var type = stream.ReadByteChar();
            type = char.ToUpper(type);

            if (type == 'S')
                type = 'C';

            return (DbfType)type;
        }
        private static void WriteDbaseType(this Stream stream, DbfType type)
        {
            stream.WriteByte((byte)type);
        }
    }

}
