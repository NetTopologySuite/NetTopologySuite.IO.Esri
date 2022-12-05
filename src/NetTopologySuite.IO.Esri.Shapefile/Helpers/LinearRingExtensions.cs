using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Net.NetworkInformation;

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
                if (shell.Contains(ringPoint))
                {
                    innerRings.Insert(0, ring); // Keep sort order (we are iterating backward)
                    rings.RemoveAt(i);
                }
            }
            return innerRings;
        }

        public static bool Contains(this LinearRing ring, Coordinate p)
        {
            if (!ring.IsValid)
            {
                return false;
            }

            if (!ring.EnvelopeInternal.Contains(p)) // Fast, but not precise.
            {
                return false;
            }

            return PointLocation.IsInRing(p, ring.CoordinateSequence);
        }
    }
}
