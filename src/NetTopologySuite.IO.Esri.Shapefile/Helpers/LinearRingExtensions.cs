using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace NetTopologySuite.IO.Esri
{
    internal static class LinearRingExtensions
    {
        public static LinearRing Extract(this List<LinearRing> rings, int index)
        {
            var item = rings[index];
            rings.RemoveAt(index);
            return item;
        }

        public static List<LinearRing> ExtractInnerRings(this List<LinearRing> rings, LinearRing shell)
        {
            var innerRings = new List<LinearRing>();
            for (int i = rings.Count - 1; i >= 0; i--)
            {
                var ring = rings[i];
                if (ring == shell)
                {
                    continue;
                }

                var ringPoint = ring.CoordinateSequence.GetCoordinate(0);

                if (!shell.EnvelopeInternal.Contains(ringPoint)) // Fast, but not precise.
                {
                    continue;
                }
                if (PointLocation.IsInRing(ringPoint, shell.CoordinateSequence))
                {
                    innerRings.Add(ring);
                    rings.RemoveAt(i);
                }
            }
            return innerRings;
        }
    }
}
