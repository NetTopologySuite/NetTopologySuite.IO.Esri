using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Readers
{
    internal class ShpPolygonReader : ShpReader<MultiPolygon>
    {
        public ShpPolygonReader(Stream shpStream, GeometryFactory factory) : base(shpStream, factory)
        {
            if (!ShapeType.IsPolygon())
                ThrowUnsupportedShapeTypeException();
        }

        internal override MultiPolygon GetEmptyGeometry()
        {
            return MultiPolygon.Empty;
        }

        internal override MultiPolygon ReadGeometry(Stream shapeBinary)
        {
            // SHP Docs: A ring is a connected sequence of four or more points (page 8)
            var partsBuilder = new ShpMultiPartBuilder(1, 4);
            partsBuilder.ReadParts(shapeBinary, HasZ, HasM, CreateCoordinateSequence);

            var polygons = new List<Polygon>();
            var holes = new List<LinearRing>();

            var shell = Factory.CreateLinearRing(partsBuilder[0]);
            if (shell.IsCCW)
            {
                ThrowInvalidRecordException("Invalid Shapefile polygon - shell coordinates are not in in clockwise order.");
            }
            for (int partIndex = 1; partIndex < partsBuilder.Count; partIndex++)
            {
                // SHP Docs: Vertices of rings defining holes in polygons are in a counterclockwise direction.
                //           Vertices for a single, ringed polygon are, therefore, always in clockwise order.
                // Shell: !ring.IsCCW
                // Hole:   ring.IsCCW

                var ring = Factory.CreateLinearRing(partsBuilder[partIndex]);
                if (ring.IsCCW)
                {
                    holes.Add(ring);
                }
                else
                {
                    // New polygon shell
                    polygons.AddRange(CreatePolygons(shell, holes));
                    shell = ring;
                    holes.Clear();
                }
            }
            polygons.AddRange(CreatePolygons(shell, holes));
            return Factory.CreateMultiPolygon(polygons.ToArray());
        }

        private IEnumerable<Polygon> CreatePolygons(LinearRing shell, List<LinearRing> innerRings)
        {
            if (innerRings.Count <= 1)
            {
                yield return Factory.CreatePolygon(shell, innerRings.ToArray());
                yield break;
            }

            // Sort holes - consider pair of island-in-a-lake-on-an-island inside the shell.
            innerRings.Sort(CompareLinearRingAreasAsc);
            var holes = new List<LinearRing>();

            while (innerRings.Count > 0)
            {
                var hole = innerRings.Extract(innerRings.Count - 1); // The largest
                holes.Add(hole);

                // Deeper nestings, aka island-in-a-lake-on-an-island-...
                // https://gis.stackexchange.com/a/147971/26684
                var nestedHoleRings = innerRings.ExtractInnerRings(hole);
                foreach (var nestedPolygon in CreateNestedPolygons(nestedHoleRings))
                {
                    yield return nestedPolygon;
                }
            }
            yield return Factory.CreatePolygon(shell, holes.ToArray());
        }

        private IEnumerable<Polygon> CreateNestedPolygons(List<LinearRing> nestedHoleRings)
        {
            if (nestedHoleRings.Count < 1)
            {
                yield break;
            }
            if (nestedHoleRings.Count == 1)
            {
                yield return Factory.CreatePolygon(nestedHoleRings[0]);
            }
            while (nestedHoleRings.Count > 0)
            {
                var shell = nestedHoleRings.Extract(nestedHoleRings.Count - 1);
                var innerRings = nestedHoleRings.ExtractInnerRings(shell);

                foreach (var polygon in CreatePolygons(shell, innerRings))
                {
                    yield return polygon;
                }
            }
        }

        private static int CompareLinearRingAreasAsc(LinearRing ring1, LinearRing ring2)
        {
            var area1 = Area.OfRing(ring1.CoordinateSequence);
            var area2 = Area.OfRing(ring2.CoordinateSequence);
            return area1.CompareTo(area2);
        }

    }
}
