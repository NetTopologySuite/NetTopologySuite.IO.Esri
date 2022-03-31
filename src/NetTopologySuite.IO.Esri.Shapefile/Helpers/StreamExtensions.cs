using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace NetTopologySuite.IO.Esri
{
    internal static class StreamExtensions
    {

        /// <summary>
        /// Moves data processing ahead a specified number of items.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="count"></param>
        public static void Advance(this Stream stream, int count)
        {
            // stream.Position += count;
            stream.Seek(count, SeekOrigin.Current);
        }

        /// <summary>
        /// Clears the content of the stream and loads content
        /// from <paramref name="source"/> stream. Copying begins at the current position
        /// in the <paramref name="source"/> stream and ends after
        /// specifed <paramref name="count"/> of bytes.
        /// </summary>
        public static void AssignFrom(this MemoryStream destination, Stream source, int count)
        {
            destination.Seek(0, SeekOrigin.Begin);

            var buffer = ArrayPool<byte>.Shared.Rent(count);
            try
            {
                source.Read(buffer, 0, count);
                destination.Write(buffer, 0, count);
                destination.Seek(0, SeekOrigin.Begin);
                destination.SetLength(count);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public static void Clear(this MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
        }

        public static void WriteAllBytes(this Stream destination, Stream bytes)
        {
            bytes.Seek(0, SeekOrigin.Begin);
            bytes.CopyTo(destination);
            destination.Seek(0, SeekOrigin.End);
        }

        public static void WriteBytes(this Stream stream, byte value, int count)
        {
            for (int i = 0; i < count; i++)
            {
                stream.WriteByte(value);
            }
        }
        public static void WriteNullBytes(this Stream stream, int count)
        {
            stream.WriteBytes(byte.MinValue, count);
        }

        public static UInt16 ReadUInt16LittleEndian(this Stream stream)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(sizeof(UInt16));
            try
            {
                stream.Read(bytes, 0, sizeof(UInt16));
                return BinaryPrimitives.ReadUInt16LittleEndian(bytes);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        public static void WriteUInt16LittleEndian(this Stream stream, UInt16 value)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(sizeof(UInt16));
            try
            {
                BinaryPrimitives.WriteUInt16LittleEndian(bytes, value);
                stream.Write(bytes, 0, sizeof(UInt16));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        public static UInt32 ReadUInt32LittleEndian(this Stream stream)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(sizeof(UInt32));
            try
            {
                stream.Read(bytes, 0, sizeof(UInt32));
                return BinaryPrimitives.ReadUInt32LittleEndian(bytes);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        public static void WriteUInt32LittleEndian(this Stream stream, UInt32 value)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(sizeof(UInt32));
            try
            {
                BinaryPrimitives.WriteUInt32LittleEndian(bytes, value);
                stream.Write(bytes, 0, sizeof(UInt32));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        public static Int32 ReadInt32LittleEndian(this Stream stream)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(sizeof(Int32));
            try
            {
                stream.Read(bytes, 0, sizeof(Int32));
                return BinaryPrimitives.ReadInt32LittleEndian(bytes);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        public static void WriteInt32LittleEndian(this Stream stream, Int32 value)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(sizeof(Int32));
            try
            {
                BinaryPrimitives.WriteInt32LittleEndian(bytes, value);
                stream.Write(bytes, 0, sizeof(Int32));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }

        }

        public static Int32 ReadInt32BigEndian(this Stream stream)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(sizeof(Int32));
            try
            {
                stream.Read(bytes, 0, sizeof(Int32));
                return BinaryPrimitives.ReadInt32BigEndian(bytes);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        public static void WriteInt32BigEndian(this Stream stream, Int32 value)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(sizeof(Int32));
            try
            {
                BinaryPrimitives.WriteInt32BigEndian(bytes, value);
                stream.Write(bytes, 0, sizeof(Int32));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        public unsafe static double ReadDoubleLittleEndian(this Stream stream)
        {
            var bytes = ArrayPool<byte>.Shared.Rent(sizeof(double));
            try
            {
                stream.Read(bytes, 0, sizeof(double));
                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bytes, 0, sizeof(double));
                }
                return BitConverter.ToDouble(bytes, 0);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bytes);
            }
        }

        public unsafe static void WriteDoubleLittleEndian(this Stream stream, double value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes, 0, sizeof(double));
            }
            stream.Write(bytes, 0, sizeof(double));
        }

        public static char ReadByteChar(this Stream stream)
        {
            return (char)stream.ReadByte();
        }

        public static string ReadString(this Stream stream, int byteCount, Encoding encoding)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(byteCount);
            try
            {
                stream.Read(buffer, 0, byteCount);
                var s = encoding.GetString(buffer, 0, byteCount); // because of ArrayPool byteCount must be used here
                if (IsNullString(s))
                {
                    return null;
                }
                return s.Trim(char.MinValue);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        /// <summary>
        /// Writes string to BinaryDataWriter.
        /// </summary>
        /// <param name="stream">Data stream.</param>
        /// <param name="s">String value to write.</param>
        /// <param name="bytesCount">Bytes count to be written in BinaryDataWriter.</param>
        /// <param name="encoding">Encoding used to translate string to bytes.</param>
        public static void WriteString(this Stream stream, string s, int bytesCount, Encoding encoding)
        {
            s = s ?? string.Empty;

            if (s.Length > bytesCount)
            {
                s = s.Substring(0, bytesCount);
            }

            // Specific encoding can add some extra bytes for national characters. Check it.

            var bytes = encoding.GetBytes(s);
            while (bytes.Length > bytesCount)
            {
                s = s.Substring(0, s.Length - 1);
                bytes = encoding.GetBytes(s);
            }

            if (bytes.Length < bytesCount)
            {
                var fixedBytes = new byte[bytesCount]; // Filled with '\0' by default
                Array.Copy(bytes, fixedBytes, bytes.Length);
                bytes = fixedBytes; // NULL terminated fixed length string
            }

            stream.Write(bytes, 0, bytes.Length);
        }

        private static bool IsNullString(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != char.MinValue)
                    return false;
            }
            return true;
        }
    }
}
