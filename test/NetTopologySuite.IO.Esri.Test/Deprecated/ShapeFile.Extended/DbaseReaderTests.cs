﻿using System;
using System.IO;
using System.Linq;
using NetTopologySuite.Features;
using NetTopologySuite.IO.Esri.Dbf;
using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.ShapeFile.Extended
{
    /// <summary>
    /// Summary description for DbfFileReaderTests
    /// </summary>
    [TestFixture]
    public class DbfReaderTests
    {
        private static readonly DateTime DATE_SAVED_IN_DBF = new DateTime(1990, 1, 1);

        private DbfReader m_Reader;
        private TempFileWriter m_TmpFile;

        [Test]
        public void Ctor_SendNullPath_ShouldThrowException()
        {
            // Act.
            Assert.Catch<FileNotFoundException>(() =>
            {
                m_Reader = new DbfReader((string)null);
            });
        }

        [Test]
        public void Ctor_SendEmptyString_ShouldThrowException()
        {
            // Act.
            Assert.Catch<FileNotFoundException>(() =>
            {
                m_Reader = new DbfReader(string.Empty);
            });
        }

        [Test]
        public void Ctor_SendWhitespaceString_ShouldThrowException()
        {
            // Act.
            Assert.Catch<FileNotFoundException>(() =>
            {
                m_Reader = new DbfReader("    \t  ");
            });
        }

        [Test]
        public void Ctor_SendNonExistantPath_ShouldThrowException()
        {
            string path = Path.Combine("this", "is", "sheker", "path", "should", "never", "exist", "on", "ur", "pc");
            path = (Environment.OSVersion.Platform == PlatformID.Win32NT ? "C:\\" : "/" )+ path;

            // Act.
            Assert.Catch<IOException>(() =>
            {
                m_Reader = new DbfReader(path);
            });
        }

        [Test]
        public void Ctor_SendValidParameters_ShouldReturnNotNull()
        {
            // Arrange
            m_TmpFile = new TempFileWriter(".dbf", DbfFiles.Read("line_ed50_geo"));

            // Act.
            m_Reader = new DbfReader(m_TmpFile.Path);

            // Assert.
            Assert.IsNotNull(m_Reader);
        }

        [Test]
        public void ReadEntry_SendNegativeIndex_ShouldThrowException()
        {
            // Arrange
            m_TmpFile = new TempFileWriter(".dbf", DbfFiles.Read("point_ed50_geo"));
            m_Reader = new DbfReader(m_TmpFile.Path);

            // Act.
            Assert.Catch<ArgumentException>(() =>
            {
                m_Reader.ReadEntry(-1);
            });
        }

        [Test]
        public void ReadEntry_SendOutOfBoundIndex_ShouldThrowException()
        {
            // Arrange
            m_TmpFile = new TempFileWriter(".dbf", DbfFiles.Read("point_ed50_geo"));
            m_Reader = new DbfReader(m_TmpFile.Path);

            // Act.
            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                m_Reader.ReadEntry(3);
            });
        }

        [Test]
        public void ReadEntry_TryReadAfterDisposed_ShouldThrowException()
        {
            // Arrange
            m_TmpFile = new TempFileWriter(".dbf", DbfFiles.Read("point_ed50_geo"));
            m_Reader = new DbfReader(m_TmpFile.Path);

            m_Reader.Dispose();

            // Act.
            Assert.Catch<InvalidOperationException>(() =>
            {
                m_Reader.ReadEntry(1);
            });
        }

        [Test]
        public void ReadEntry_ReadEntryValues_ShoudReturnCorrectValues()
        {
            // Arrange
            m_TmpFile = new TempFileWriter(".dbf", DbfFiles.Read("point_ed50_geo"));
            m_Reader = new DbfReader(m_TmpFile.Path);

            var expectedTable = new
            {
                Ids = new int[]
                {
                    3, 2, 1
                },
                Strings = new string[]
                {
                    "str3", "str2", "str1"
                },
                WholeNums = new int[]
                {
                    3, 2, 1
                },
                DecNums = new int[]
                {
                    3, 2, 1
                },
            };

            // Act.
            var results = new IAttributesTable[]
            {
                m_Reader.ReadEntry(0),
                m_Reader.ReadEntry(1),
                m_Reader.ReadEntry(2)
            };

            // Assert.
            int currResIndex = 0;
            foreach (var res in results)
            {
                object id = res["id"];
                object str = res["str"];
                object wholeNum = res["wholeNum"];
                object decNum = res["decNum"];
                object date = res["dt"];

                Assert.IsNotNull(id);
                Assert.IsNotNull(str);
                Assert.IsNotNull(wholeNum);
                Assert.IsNotNull(decNum);
                Assert.IsNotNull(date);

                Assert.IsInstanceOf<int>(id);
                Assert.IsInstanceOf<string>(str);
                Assert.IsInstanceOf<int>(wholeNum);
                Assert.IsInstanceOf<int>(decNum);
                Assert.IsInstanceOf<DateTime>(date);

                Assert.AreEqual(id, expectedTable.Ids[currResIndex]);
                Assert.AreEqual(str, expectedTable.Strings[currResIndex]);
                Assert.AreEqual(wholeNum, expectedTable.WholeNums[currResIndex]);
                Assert.AreEqual(decNum, expectedTable.DecNums[currResIndex]);
                Assert.AreEqual(date, DATE_SAVED_IN_DBF);

                currResIndex++;
            }
        }

        [Test]
        public void ReadEntry_ReadNonExistantKeyFromEntry_ShoudReturnCorrectValues()
        {
            // Arrange
            m_TmpFile = new TempFileWriter(".dbf", DbfFiles.Read("point_ed50_geo"));
            m_Reader = new DbfReader(m_TmpFile.Path);

            var results = m_Reader.ReadEntry(0);

            // Act.
            Assert.Catch<ArgumentException>(() =>
            {
                object a = results["a"];
            });
        }

        [Test]
        public void ForEachIteration_ReadEntryValues_ShoudReturnCorrectValues()
        {
            // Arrange
            m_TmpFile = new TempFileWriter(".dbf", DbfFiles.Read("point_ed50_geo"));
            m_Reader = new DbfReader(m_TmpFile.Path);

            var expectedTable = new
            {
                Ids = new int[]
                {
                    3, 2, 1
                },
                Strings = new string[]
                {
                    "str3", "str2", "str1"
                },
                WholeNums = new int[]
                {
                    3, 2, 1
                },
                DecNums = new int[]
                {
                    3, 2, 1
                },
            };

            // Act.
            var results = m_Reader.ToArray();

            Assert.AreEqual(results.Length, 3);

            // Assert.
            int currResIndex = 0;
            foreach (var res in results)
            {
                object id = res["id"];
                object str = res["str"];
                object wholeNum = res["wholeNum"];
                object decNum = res["decNum"];
                object date = res["dt"];

                Assert.IsNotNull(id);
                Assert.IsNotNull(str);
                Assert.IsNotNull(wholeNum);
                Assert.IsNotNull(decNum);
                Assert.IsNotNull(date);

                Assert.IsInstanceOf<int>(id);
                Assert.IsInstanceOf<string>(str);
                Assert.IsInstanceOf<int>(wholeNum);
                Assert.IsInstanceOf<int>(decNum);
                Assert.IsInstanceOf<DateTime>(date);

                Assert.AreEqual(id, expectedTable.Ids[currResIndex]);
                Assert.AreEqual(str, expectedTable.Strings[currResIndex]);
                Assert.AreEqual(wholeNum, expectedTable.WholeNums[currResIndex]);
                Assert.AreEqual(decNum, expectedTable.DecNums[currResIndex]);
                Assert.AreEqual(date, DATE_SAVED_IN_DBF);

                currResIndex++;
            }
        }

        [TearDown]
        public void TestCleanup()
        {
            if (m_Reader != null)
            {
                m_Reader.Dispose();
                m_Reader = null;
            }

            if (m_TmpFile != null)
            {
                m_TmpFile.Dispose();
                m_TmpFile = null;
            }
        }
    }

    static class DbfFiles
    {
        public static byte[] Read(string filename)
        {
            string file = Path.ChangeExtension(filename, ".dbf");
            string path = TestShapefiles.PathTo(file);
            Assert.That(File.Exists(path), Is.True);
            return File.ReadAllBytes(path);
        }
    }
}
