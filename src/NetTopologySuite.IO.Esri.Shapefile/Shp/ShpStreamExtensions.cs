using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shx;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp
{


    internal static class ShpStramExtensions
    {
        public static void WriteShpFileHeader(this Stream stream, ShapeType type, int fileLength, ShpExtent extent, bool hasZ, bool hasM)
        {
            stream.WriteInt32BigEndian(Shapefile.FileCode);
            stream.WriteNullBytes(20);

            stream.Write16BitWords(fileLength);  // in 16-bit words, including the header
            stream.WriteInt32LittleEndian(Shapefile.Version);
            stream.WriteGeometryType(type);

            stream.WriteXYBoundingBox(extent);

            if (hasZ)
            {
                stream.WriteZRange(extent.Z.Min, extent.Z.Max);
            }
            else
            {
                stream.WriteZRange(0.0, 0.0); // ArcMap uses zero as default.
            }

            if (hasM)
            {
                stream.WriteMRange(extent.M.Min, extent.M.Max);
            }
            else
            {
                stream.WriteMRange(0.0, 0.0); // ArcMap uses zero as default.
            }
        }

        public static void ReadShpFileHeader(this Stream stream, out ShapeType type, out int fileLength)
        {
            var fileCode = stream.ReadInt32BigEndian();
            if (fileCode != Shapefile.FileCode)
                throw new FileLoadException("Invalid shapefile format.");

            stream.Advance(20);

            fileLength = stream.Read16BitWords();  // in 16-bit words, including the header
            var version = stream.ReadInt32LittleEndian();
            type = stream.ReadShapeType();

            stream.ReadXYBoundingBox();
            stream.ReadZRange();
            stream.ReadMRange();

            Debug.Assert(version == Shapefile.Version, "Shapefile version", $"Ivalid SHP version: {version} (expected: 1000).");
        }


        public static void WriteShpRecordHeader(this Stream stream, int recrodNumber, int contentLength)
        {
            stream.WriteInt32BigEndian(recrodNumber);
            stream.Write16BitWords(contentLength);
        }
        public static (int RecrodNumber, int ContentLength) ReadShpRecordHeader(this Stream stream)
        {
            var recrodNumber = stream.ReadInt32BigEndian();
            var contentLength = stream.Read16BitWords();
            return (recrodNumber, contentLength);
        }


        public static void WriteGeometryType(this Stream stream, ShapeType type)
        {
            stream.WriteInt32LittleEndian((int)type);
        }
        public static ShapeType ReadShapeType(this Stream stream)
        {
            return (ShapeType)stream.ReadInt32LittleEndian();
        }


        public static (double x, double y) ReadXYCoordinates(this Stream stream)
        {
            var x = stream.ReadDoubleLittleEndian();
            var y = stream.ReadDoubleLittleEndian();
            return (x, y);
        }
        private static void WriteXYCoordinate(this Stream stream, double x, double y)
        {
            // Avoid performance costs (if you trying to pas NaN as X,Y then you're wrong).
            // x = x.ToValidShpOrdinate(0.0);
            // x = x.ToValidShpOrdinate(0.0);

            stream.WriteDoubleLittleEndian(x);
            stream.WriteDoubleLittleEndian(y);
        }
        public static void WriteXYOrdinate(this Stream stream, double x, double y, ShpExtent shpExtent)
        {
            stream.WriteXYCoordinate(x, y);
            shpExtent.X.Expand(x);
            shpExtent.Y.Expand(y);
        }



        public static void ReadXYCoordinates(this Stream stream, CoordinateSequence pointSequence)
        {
            for (int i = 0; i < pointSequence.Count; i++)
            {
                var (x, y) = stream.ReadXYCoordinates();
                pointSequence.SetX(i, x);
                pointSequence.SetY(i, y);
            }
        }
        public static void WriteXYCoordinates(this Stream stream, CoordinateSequence pointSequence)
        {
            for (int i = 0; i < pointSequence.Count; i++)
            {
                var x = pointSequence.GetX(i);
                var y = pointSequence.GetY(i);
                stream.WriteXYCoordinate(x, y);
            }
        }


        public static double ReadZCoordinate(this Stream stream)
        {
            return stream.ReadDoubleLittleEndian();
        }
        private static void WriteZCoordinate(this Stream stream, double z)
        {
            stream.WriteDoubleLittleEndian(z.ToValidShpCoordinate());
        }


        public static (double min, double max) ReadZRange(this Stream stream)
        {
            var min = stream.ReadZCoordinate();
            var max = stream.ReadZCoordinate();
            return (min, max);
        }
        public static void WriteZRange(this Stream stream, double min, double max)
        {
            stream.WriteZCoordinate(min);
            stream.WriteZCoordinate(max);
        }


        public static void ReadZCoordinates(this Stream stream, CoordinateSequence pointSequence)
        {
            for (int i = 0; i < pointSequence.Count; i++)
            {
                pointSequence.SetZ(i, stream.ReadZCoordinate());
            }
        }
        public static void WriteZCoordinates(this Stream stream, CoordinateSequence pointSequence)
        {
            for (int i = 0; i < pointSequence.Count; i++)
            {
                stream.WriteZCoordinate(pointSequence.GetZ(i));
            }
        }


        public static double ReadMValue(this Stream stream)
        {
            var m = stream.ReadDoubleLittleEndian();
            if (m < Shapefile.MeasureMinValue)
                return double.NaN;

            return m;
        }
        private static void WriteMValue(this Stream shpRecordData, double m)
        {
            shpRecordData.WriteDoubleLittleEndian(m.ToValidShpMeasure());
        }


        public static (double min, double max) ReadMRange(this Stream stream)
        {
            var min = stream.ReadMValue();
            var max = stream.ReadMValue();
            return (min, max);
        }
        public static void WriteMRange(this Stream stream, double min, double max)
        {
            stream.WriteMValue(min);
            stream.WriteMValue(max);
        }


        public static void ReadMValues(this Stream stream, CoordinateSequence pointSequence)
        {
            for (int i = 0; i < pointSequence.Count; i++)
            {
                pointSequence.SetM(i, stream.ReadMValue());

            }
        }
        public static void WriteMValues(this Stream stream, CoordinateSequence pointSequence)
        {
            for (int i = 0; i < pointSequence.Count; i++)
            {
                stream.WriteMValue(pointSequence.GetM(i));
            }
        }

        public static (double minX, double maxX, double minY, double maxY) ReadXYBoundingBox(this Stream stream)
        {
            var (minX, minY) = stream.ReadXYCoordinates();
            var (maxX, maxY) = stream.ReadXYCoordinates();
            return (minX, maxX, minY, maxY);
        }
        public static void WriteXYBoundingBox(this Stream stream, ShpExtent shpExtent)
        {
            stream.WriteXYCoordinate(shpExtent.X.Min.ToValidShpCoordinate(), shpExtent.Y.Min.ToValidShpCoordinate());
            stream.WriteXYCoordinate(shpExtent.X.Max.ToValidShpCoordinate(), shpExtent.Y.Max.ToValidShpCoordinate());
        }


        public static int ReadPartCount(this Stream stream)
        {
            return stream.ReadInt32LittleEndian();
        }
        public static void WritePartCount(this Stream stream, int count)
        {
            stream.WriteInt32LittleEndian(count);
        }


        public static int ReadPointCount(this Stream stream)
        {
            return stream.ReadInt32LittleEndian();
        }
        public static void WritePointCount(this Stream stream, int count)
        {
            stream.WriteInt32LittleEndian(count);
        }


        public static void ReadPartOfsets(this Stream stream, int partCount, List<int> offsets)
        {
            offsets.Clear();
            for (int partIndex = 0; partIndex < partCount; partIndex++)
            {
                offsets.Add(stream.ReadInt32LittleEndian());
            }
        }
        public static void WritePartOffsets(this Stream stream, List<int> offsets)
        {
            for (int i = 0; i < offsets.Count; i++)
            {
                stream.WriteInt32LittleEndian(offsets[i]);
            }
        }


        public static void ReadPoint(this Stream stream, CoordinateSequence pointSequence)
        {
            var (x, y) = stream.ReadXYCoordinates();
            pointSequence.SetX(0, x);
            pointSequence.SetY(0, y);

            if (pointSequence.HasZ)
            {
                var z = stream.ReadZCoordinate();
                pointSequence.SetZ(0, z);
            }
            if (pointSequence.HasM)
            {
                var m = stream.ReadMValue();
                pointSequence.SetM(0, m);
            }
        }
        public static void WritePoint(this Stream stream, CoordinateSequence pointSequence)
        {
            stream.WriteXYCoordinate(pointSequence.GetX(0), pointSequence.GetY(0));

            if (pointSequence.HasZ)
            {
                stream.WriteZCoordinate(pointSequence.GetZ(0));
            }

            if (pointSequence.HasM)
            {
                stream.WriteMValue(pointSequence.GetM(0));
            }
        }

    }



}
