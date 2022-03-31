using NetTopologySuite.IO.Esri.Dbf.Fields;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetTopologySuite.IO.Esri.Dbf
{


    /// <summary>
    ///  Class that allows records in a dbase file to be enumerated.
    /// </summary>
    public class DbfWriter : ManagedDisposable
    {
        private Stream DbfStream;

        /// <summary>
        /// Returns the fields in the dbase file.
        /// </summary>
        public DbfFieldCollection Fields { get; private set; }

        /// <summary>
        /// Encoding of dBASE file
        /// </summary>
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// Return the length of the records in bytes.
        /// </summary>
        /// <returns></returns>
        public int RecordSize { get; private set; }


        /// <summary>
        /// Return the number of records in the file. 
        /// </summary>
        /// <returns></returns>
        /// <remarks>RecordCount is saved in DBF file as 32-bit unisigned integer. Record count is limited to 1 Billion (http://www.dbase.com/Knowledgebase/faq/dBASE_Limits_FAQ.html), so it is les than int.MaxValue </remarks>
        public int RecordCount { get; private set; } = 0;


        /// <summary>
        /// Initializes new DbaseStreamReader
        /// </summary>
        /// <param name="stream">Stream of source DBF file.</param>
        /// <param name="fields">dBASE field definitions.</param>
        /// <param name="encoding">DBF file encoding or null if encoding should be resolved from DBF reserved bytes.</param>
        public DbfWriter(Stream stream, IReadOnlyList<DbfField> fields, Encoding encoding)
        {
            Encoding = encoding ?? Encoding.UTF8;
            IntializeFields(fields);
            WriteHeader(stream);
        }


        /// <summary>
        /// Initializes new DbaseStreamReader
        /// </summary>
        /// <param name="dbfPath">Path to DBF file.</param>
        /// <param name="fields">dBASE field definitions.</param>
        /// <param name="encoding">DBF file encoding or null if encoding should be resolved from DBF reserved bytes.</param>
        public DbfWriter(string dbfPath, IReadOnlyList<DbfField> fields, Encoding encoding)
        {
            Encoding = encoding ?? Encoding.UTF8;
            IntializeFields(fields);
            WriteCpgEncoding(dbfPath, encoding);
            try
            {
                WriteHeader(OpenManagedFileStream(dbfPath, ".dbf", FileMode.Create));
            }
            catch
            {
                DisposeManagedResources();
                throw;
            }
        }

        private void IntializeFields(IReadOnlyList<DbfField> fields)
        {
            if (fields == null || fields.Count < 1)
                throw new ArgumentException("dBASE file must contain at least one field.", nameof(fields));

            if (fields.Count > Dbf.MaxFieldCount)
                throw new ArgumentException($"dBASE file must contain no more than {Dbf.MaxFieldCount} fields.", nameof(fields));

            Fields = new DbfFieldCollection(fields.Count);
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i] is DbfCharacterField textField)
                    textField.Encoding = Encoding;
                Fields.Add(fields[i]);
            }
        }

        private void WriteHeader(Stream stream)
        {
            DbfStream = stream ?? throw new ArgumentNullException("Uninitialized dBASE stream.", nameof(stream));
            if (DbfStream.Position != 0)
                DbfStream.Seek(0, SeekOrigin.Begin);

            var headerSize = Dbf.TableDescriptorSize + Dbf.FieldDescriptorSize * Fields.Count + 1;  // 2 bytes (table descriptor header + field descriptor headers + header terminator char 0x0d)
            if (headerSize > UInt16.MaxValue)
                throw new InvalidDataException("dBASE III header size exceeded " + UInt16.MaxValue.ToString() + " bytes.");

            RecordSize = Fields.Sum(f => f.Length) + 1;  // Sum of lengths of all fields + 1 (deletion flag)

            // Table descriptor
            stream.WriteDbfVersion(Dbf.Dbase3Version);
            stream.WriteDbfLastUpdateDate(DateTime.Now); //.WriteBytes(GetLastUpdateDate());

            stream.WriteDbfRecordCount(1);  // Write dummy recordCount. This will be replaced at the end of writing.
            stream.WriteDbfHeaderSize(headerSize);
            stream.WriteDbfRecordSize(RecordSize);
            stream.WriteNullBytes(17);
            stream.WriteDbfEncoding(Encoding);
            stream.WriteNullBytes(2);

            // write field description array
            foreach (var field in Fields)
            {
                stream.WriteDbaseFieldDescriptor(field, Encoding);
            }

            // Now header BinaryDataWriter should be at last byte position
            stream.WriteByte(Dbf.HeaderTerminatorMark);
        }


        private void WriteCpgEncoding(string dbfPath, Encoding encoding)
        {
            if (encoding == null)
                return;

            string cpgPath = Path.ChangeExtension(dbfPath, ".cpg");
            var cpgText = encoding.WebName.ToUpperInvariant();
            File.WriteAllText(cpgPath, cpgText);
        }


        /// <summary>
        /// Writes field values to the underlying stream and advances the position to the next record.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element;
        /// false if the enumerator has passed the end of the collection.
        /// </returns>
        public void Write()
        {
            DbfStream.WriteByte(Dbf.ValidRecordMark);

            for (int i = 0; i < Fields.Count; i++)
            {
                Fields[i].WriteValue(DbfStream);
            }
            RecordCount++;
        }


        /// <summary>
        /// Writes record values to the underlying stream and advances the position to the next record.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element;
        /// false if the enumerator has passed the end of the collection.
        /// </returns>
        public void Write(IReadOnlyDictionary<string, object> values)
        {
            Fields.SetValues(values);
            Write();
        }

        private void FinalizeWriting()
        {
            if (DbfStream == null || DbfStream.Position < Dbf.TableDescriptorSize + Dbf.FieldDescriptorSize) // DBF must have at least one field
                return;

            DbfStream.WriteByte(Dbf.EndOfFileMark);

            DbfStream.Seek(4, SeekOrigin.Begin);
            DbfStream.WriteDbfRecordCount(RecordCount);
        }

        /// <inheritdoc/>
        protected override void DisposeManagedResources()
        {
            FinalizeWriting();
            base.DisposeManagedResources(); // FinalizeWriting() is using underlying file streams. Dispose them at the end.
        }
    }



}
