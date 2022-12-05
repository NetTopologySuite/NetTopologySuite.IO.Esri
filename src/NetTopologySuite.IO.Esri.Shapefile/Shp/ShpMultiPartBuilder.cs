using NetTopologySuite.Geometries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp
{
    internal class ShpMultiPartBuilder : IEnumerable<CoordinateSequence>
    {
        private readonly List<CoordinateSequence> Parts;
        private readonly List<int> Offsets;
        private readonly ShpExtent Extent;
        private int PointCount;
        private readonly int MinPointCount;

        public int Count => Parts.Count;

        public CoordinateSequence this[int index] => Parts[index];

        public ShpMultiPartBuilder(int capacity, int minPointCount)
        {
            Parts = new List<CoordinateSequence>(capacity);
            Offsets = new List<int>(capacity);
            Extent = new ShpExtent();
            PointCount = 0;
            MinPointCount = minPointCount;

        }

        public void Clear()
        {
            Parts.Clear();
            Offsets.Clear();
            Extent.Clear();
            PointCount = 0;
        }

        public void AddPart(CoordinateSequence part)
        {
            if (part == null || part.Count < MinPointCount)
            {
                return;
            }

            Parts.Add(part);
            Offsets.Add(PointCount); // For the first part it will add 0.
            Extent.Expand(part);
            PointCount += part.Count;
        }

        public void WriteParts(Stream stream, bool hasZ, bool hasM)
        {
            stream.WriteXYBoundingBox(Extent);
            stream.WritePartCount(Parts.Count);
            stream.WritePointCount(PointCount);
            stream.WritePartOffsets(Offsets);

            foreach (var part in Parts)
            {
                stream.WriteXYCoordinates(part);
            }

            if (hasZ)
            {
                stream.WriteZRange(Extent.Z.Min, Extent.Z.Max);
                foreach (var part in Parts)
                {
                    stream.WriteZCoordinates(part);
                }
            }

            if (hasM)
            {
                stream.WriteMRange(Extent.M.Min, Extent.M.Max);
                foreach (var part in Parts)
                {
                    stream.WriteMValues(part);
                }
            }
        }

        public virtual void ReadParts(Stream stream, bool hasZ, bool hasM, Func<int, CoordinateSequence> createCoordinateSequence)
        {
            Clear();
            var partCount = stream.ReadPartCount();
            PointCount = stream.ReadPointCount();
            stream.ReadPartOfsets(partCount, Offsets);

            Offsets.Add(PointCount);
            for (int partIndex = 0; partIndex < partCount; partIndex++)
            {
                var partPointCount = Offsets[partIndex + 1] - Offsets[partIndex];
                var part = createCoordinateSequence(partPointCount);
                Parts.Add(part);
            }
            Offsets.RemoveAt(Offsets.Count - 1);


            foreach (var part in Parts)
            {
                stream.ReadXYCoordinates(part);
            }

            if (hasZ)
            {
                stream.ReadZRange();
                foreach (var part in Parts)
                {
                    stream.ReadZCoordinates(part);
                }
            }

            if (hasM)
            {
                stream.ReadMRange();
                foreach (var part in Parts)
                {
                    stream.ReadMValues(part);
                }
            }
        }

        public void CloseRings(Func<int, CoordinateSequence> createCoordinateSequence)
        {
            for (int i = 0; i < Parts.Count; i++)
            {
                Parts[i] = CloseRing(Parts[i], createCoordinateSequence);
            }
        }

        private CoordinateSequence CloseRing(CoordinateSequence part, Func<int, CoordinateSequence> createCoordinateSequence)
        {
            if (part.Count < 3)
            {
                return part;
            }

            var firstCoordinate = part.GetCoordinate(0);
            var lastCoordinate = part.GetCoordinate(part.Count - 1);

            if (firstCoordinate.Equals(lastCoordinate))
            {
                return part;
            }

            var closedPart = createCoordinateSequence(part.Count + 1);
            for (int i = 0; i < part.Count; i++)
            {
                CopyCoordinate(part, closedPart, i, i);
            }
            CopyCoordinate(part, closedPart, 0, closedPart.Count - 1);

            return closedPart;
        }

        private void CopyCoordinate(CoordinateSequence part, CoordinateSequence closedPart, int partCoordinateIndex, int closedPartCoordinateIndex)
        {
            for (int ordinateIndex = 0; ordinateIndex < part.Dimension; ordinateIndex++)
            {
                var ordinate = part.GetOrdinate(partCoordinateIndex, ordinateIndex);
                closedPart.SetOrdinate(closedPartCoordinateIndex, ordinateIndex, ordinate);
            }
        }

        public void UpdateExtent(ShpExtent extent)
        {
            extent.Expand(Extent);
        }

        public IEnumerator<CoordinateSequence> GetEnumerator()
        {
            return Parts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Parts.GetEnumerator();
        }
    }
}
