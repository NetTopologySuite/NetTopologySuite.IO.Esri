using NetTopologySuite.IO.Esri.Dbf.Fields;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;



namespace NetTopologySuite.IO.Esri.Dbf
{


    /// <summary>
    ///     Class that allows records in a dbase file to be enumerated.
    /// </summary>
    public class DbfReader : ManagedDisposable, IEnumerable<IReadOnlyDictionary<string, object>>
    {
        private int HeaderSize;
        private int CurrentIndex = 0;
        private Stream DbfStream;
        private MemoryStream Buffer;

        /// <summary>
        /// Sets or returns the date this file was last updated.
        /// </summary>
        /// <returns></returns>
        public DateTime LastUpdateDate { get; private set; }


        /// <summary>
        /// Return the number of records in the file. 
        /// </summary>
        /// <returns></returns>
        /// <remarks>RecordCount is saved in DBF file as 32-bit unisigned integer. Record count is limited to 1 Billion (http://www.dbase.com/Knowledgebase/faq/dBASE_Limits_FAQ.html), so it is les than int.MaxValue </remarks>
        public int RecordCount { get; private set; }

        /// <summary>
        /// Return the length of the records in bytes.
        /// </summary>
        /// <returns></returns>
        public int RecordSize { get; private set; }

        /// <summary>
        /// Returns the fields in the dbase file.
        /// </summary>
        public DbfFieldCollection Fields { get; private set; }

        /// <summary>
        /// Encoding used by dBASE file.
        /// </summary>
        public Encoding Encoding { get; private set; }


        /// <summary>
        /// Initializes new instance of the reader.
        /// </summary>
        /// <param name="stream">Stream of source DBF file.</param>
        /// <param name="encoding">DBF file encoding or null if encoding should be resolved from DBF reserved bytes.</param>
        public DbfReader(Stream stream, Encoding encoding = null)
        {
            Initialize(stream, encoding);
        }

        /// <summary>
        /// Initializes new instance of the reader.
        /// </summary>
        /// <param name="dbfPath">Path to DBF file.</param>
        /// <param name="encoding">DBF file encoding or null if encoding should be resolved from DBF reserved bytes.</param>
        public DbfReader(string dbfPath, Encoding encoding = null)
        {
            encoding = encoding ?? GetEncoding(dbfPath);
            try
            {
                var dbfStream = OpenManagedFileStream(dbfPath, ".dbf", FileMode.Open);
                Initialize(dbfStream, encoding);
            }
            catch
            {
                DisposeManagedResources();
                throw;
            }
        }

        private void Initialize(Stream stream, Encoding encoding = null)
        {
            DbfStream = stream ?? throw new ArgumentNullException("Uninitialized dBASE stream.", nameof(stream));

            if (DbfStream.Position != 0)
                DbfStream.Seek(0, SeekOrigin.Begin);

            Buffer = new MemoryStream();
            AddManagedResource(Buffer);

            Buffer.AssignFrom(DbfStream, Dbf.TableDescriptorSize);

            var version = Buffer.ReadDbfVersion();
            if (version != Dbf.Dbase3Version)
                throw new NotSupportedException("Unsupported dBASE version: " + version);

            LastUpdateDate = Buffer.ReadDbfLastUpdateDate();
            RecordCount = Buffer.ReadDbfRecordCount();
            HeaderSize = Buffer.ReadDbfHeaderSize();   // 2 bytes (table descriptor header + field descriptor headers + header terminator char 0x0d)
            RecordSize = Buffer.ReadDbfRecordSize();
            Buffer.Advance(17);

            Encoding = encoding ?? Buffer.ReadDbfEncoding() ?? Encoding.UTF8; // null => Try to read encoding from DBF's reserved bytes
            Buffer.Advance(2);

            // --- File header is done, read field descriptor header now ---

            var fieldsHeaderSize = HeaderSize - Dbf.TableDescriptorSize - 1; // Header ends with header terminator char 0x0d
            var fieldCount = fieldsHeaderSize / Dbf.FieldDescriptorSize;

            if (fieldsHeaderSize % Dbf.FieldDescriptorSize != 0)
                throw new NotSupportedException("Invalid dBASE III file format.");

            //Binary.TraceToConsole("File Header", 0, Dbf.TableDescriptorSize);

            Buffer.AssignFrom(DbfStream, fieldsHeaderSize);
            Fields = new DbfFieldCollection(fieldCount);
            for (int i = 0; i < fieldCount; i++)
            {
                Fields.Add(Buffer.ReadDbaseFieldDescriptor(Encoding));
                //Binary.TraceToConsole("Field Header: " + Fields[Fields.Count -1].Name, i * Dbf.FieldDescriptorSize, Dbf.FieldDescriptorSize);
            }

            // Last byte is a marker for the end of the DBF file header (including field definitions).
            // Trond Benum: This fails for some presumeably valid test shapefiles, so don't raise error.
            var headerTerminator = DbfStream.ReadByte();
            if (headerTerminator != Dbf.HeaderTerminatorMark)
            {
                Debug.WriteLine("Invalid dBASE III header terminator mark: " + headerTerminator);
            }

            if (DbfStream.Position != HeaderSize)
            {
                //reader.BaseStream.Seek(headerSize, SeekOrigin.Begin);
                throw new FileLoadException("Reading dBASE III file header failed.");
            }
        }


        internal void Restart()
        {
            DbfStream.Seek(HeaderSize, SeekOrigin.Begin);
        }


        private Encoding GetEncoding(string dbfPath)
        {
            var cpgText = ReadFileText(dbfPath, ".cpg"); // Esri 

            if (string.IsNullOrEmpty(cpgText))
                cpgText = ReadFileText(dbfPath, ".cst"); // GeoServer.org

            if (string.IsNullOrEmpty(cpgText))
                return null;

            try
            {
                return Encoding.GetEncoding(cpgText);
            }
            catch (Exception)
            {
                DbfEncoding.WriteMissingEncodingMessage(cpgText);
                return null;
            }
        }

        private string ReadFileText(string filePath, string fileExtension)
        {
            filePath = Path.ChangeExtension(filePath, fileExtension);
            if (!File.Exists(filePath))
                return null;

            return File.ReadAllText(filePath).Trim();
        }

        /// <summary>
        /// Reads field values from underlying stream and advances the enumerator to the next record.
        /// </summary>
        /// <param name="deleted">Indicates if the record was marked as deleted.</param>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next record;
        /// false if the enumerator has passed the end of the table.
        /// </returns>
        public bool Read(out bool deleted)
        {
            if (CurrentIndex >= RecordCount)
            {
                deleted = false;
                return false;
            }

            Buffer.AssignFrom(DbfStream, RecordSize);
            var deletedFlag = Buffer.ReadByte();
            deleted = deletedFlag == Dbf.DeletedRecordMark;

            for (int i = 0; i < Fields.Count; i++)
            {
                Fields[i].ReadValue(Buffer);
            }

            CurrentIndex++;
            return true;
        }


        /// <summary>
        /// Reads values from underlying stream and advances the enumerator to the next record.
        /// </summary>
        /// <param name="values">Array of field values. If null specified new array will be created.</param>
        /// <param name="deleted">Indicates if the reacord was marked as deleted.</param>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element;
        /// false if the enumerator has passed the end of the collection.
        /// </returns>
        public bool Read(out IReadOnlyDictionary<string, object> values, out bool deleted)
        {
            if (!Read(out deleted))
            {
                //values = null; // This would cause recreating array in next iteration
                values = DbfField.EmptyFieldValues;
                return false;
            }

            values = Fields.GetValues();
            return true;
        }

        #region IEnumerable

        IEnumerator<IReadOnlyDictionary<string, object>> IEnumerable<IReadOnlyDictionary<string, object>>.GetEnumerator()
        {
            return new DbfEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DbfEnumerator(this);
        }

        private class DbfEnumerator : IEnumerator<IReadOnlyDictionary<string, object>>
        {
            private readonly DbfReader Owner;
            public IReadOnlyDictionary<string, object> Current { get; private set; }
            object IEnumerator.Current => Current;

            public DbfEnumerator(DbfReader owner)
            {
                Owner = owner;
            }

            public void Reset()
            {
                Owner.Restart();
            }

            public bool MoveNext()
            {
                var succeed = Owner.Read(out var fieldValues, out var deleted);
                if (deleted)
                {
                    return MoveNext();
                }

                Current = fieldValues;
                return succeed;
            }

            public void Dispose()
            {
                // Nothing to dispose
            }
        }

        #endregion
    }


}
