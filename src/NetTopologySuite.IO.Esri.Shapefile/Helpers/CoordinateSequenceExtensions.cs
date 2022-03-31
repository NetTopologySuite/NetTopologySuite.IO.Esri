using NetTopologySuite.Geometries;
using System;

namespace NetTopologySuite.IO.Esri
{
    internal static class CoordinateSequenceExtensions
    {
        public static CoordinateSequence Create(this CoordinateSequenceFactory factory, int size, bool hasZ, bool hasM)
        {
            if (hasM && hasZ)
            {
                return factory.Create(size, Ordinates.XYZM);
            }
            else if (hasM)
            {
                return factory.Create(size, Ordinates.XYM);
            }
            else if (hasZ)
            {
                return factory.Create(size, Ordinates.XYZ);
            }
            else
            {
                return factory.Create(size, Ordinates.XY);
            }
        }

        private static (double min, double max) GetRange(this CoordinateSequence sequence, int ordinateIndex)
        {
            double min = double.MaxValue;
            double max = double.MinValue;
            for (int i = 0; i < sequence.Count; i++)
            {
                min = Math.Min(min, sequence.GetOrdinate(i, ordinateIndex));
                max = Math.Max(max, sequence.GetOrdinate(i, ordinateIndex));
            }
            return (min, max);
        }

        public static (double min, double max) GetXRange(this CoordinateSequence sequence)
        {
            return GetRange(sequence, 0);
        }

        public static (double min, double max) GetYRange(this CoordinateSequence sequence)
        {
            return GetRange(sequence, 1);
        }

        public static (double min, double max) GetZRange(this CoordinateSequence sequence)
        {
            return GetRange(sequence, sequence.ZOrdinateIndex);
        }

        public static (double min, double max) GetMRange(this CoordinateSequence sequence)
        {
            return GetRange(sequence, sequence.MOrdinateIndex);
        }
    }
}
