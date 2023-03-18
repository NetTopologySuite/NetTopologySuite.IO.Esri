using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetTopologySuite.IO.Esri.Shp.Readers
{
    internal class ShpPolygonReader : ShpReader<MultiPolygon>
    {
        private static readonly LinearRing InvalidLinearRing = new LinearRing(new Coordinate[]
        {
            new Coordinate(),
            new Coordinate(),
            new Coordinate(),
            new Coordinate()
        });

        /// <inheritdoc/>
        public ShpPolygonReader(Stream shpStream, ShapefileReaderOptions options = null)
            : base(shpStream, options)
        {
            if (!ShapeType.IsPolygon())
                ThrowUnsupportedShapeTypeException();
        }

        internal override MultiPolygon GetEmptyGeometry()
        {
            return Factory.CreateMultiPolygon();
        }

        internal override bool ReadGeometry(Stream stream, out MultiPolygon geometry)
        {
            var bbox = stream.ReadXYBoundingBox();
            if (!IsInMbr(bbox))
            {
                geometry = null;
                return false;
            }

            // SHP Docs: A ring is a connected sequence of four or more points (page 8)
            var parts = new ShpMultiPartBuilder(1, 3);
            parts.ReadParts(stream, HasZ, HasM, CreateCoordinateSequence);
            geometry = BuildMultiPolygon(parts);

            if (GeometryBuilderMode == GeometryBuilderMode.SkipInvalidShapes && !geometry.IsValid)
            {
                return false;
            }
            if (!IsInMbr(geometry))
            {
                return false;
            }
            if (GeometryBuilderMode == GeometryBuilderMode.FixInvalidShapes)
            {
                geometry.Normalize();
            }
            return true;
        }

        private MultiPolygon BuildMultiPolygon(ShpMultiPartBuilder parts)
        {
            if (parts.Count == 0)
            {
                return MultiPolygon.Empty;
            }

            if (GeometryBuilderMode == GeometryBuilderMode.Strict)
            {
                var multiPolygon = BuildMultiPolygonIgnoringInvalidShapes(parts, false);
                if (!multiPolygon.IsValid)
                {
                    throw new ShapefileException("Invalid polygon geometry.");
                }
                return multiPolygon;
            }
            else if (GeometryBuilderMode == GeometryBuilderMode.IgnoreInvalidShapes || GeometryBuilderMode == GeometryBuilderMode.SkipInvalidShapes)
            {
                return BuildMultiPolygonIgnoringInvalidShapes(parts, GeometryBuilderMode == GeometryBuilderMode.IgnoreInvalidShapes);
            }
            else if (GeometryBuilderMode == GeometryBuilderMode.QuickFixInvalidShapes)
            {
                return BuildMultiPolygonQuickFixingInvalidShapes(parts);
            }
            else if (GeometryBuilderMode == GeometryBuilderMode.FixInvalidShapes)
            {
                return BuildMultiPolygonFixingInvalidShapes(parts);
            }
            else 
            {
                throw new ShapefileException($"Unsupported {nameof(GeometryBuilderMode)}: {GeometryBuilderMode}.");
            }
        }

        private MultiPolygon BuildMultiPolygonIgnoringInvalidShapes(ShpMultiPartBuilder parts, bool ingoreInvalidShapes)
        {
            var rings = parts
                .Select(p => CreateLinearRing(p, ingoreInvalidShapes))
                .ToList();

            var shell = rings[0];
            if (rings.Count == 1)
            {
                var polygon = Factory.CreatePolygon(shell);
                return Factory.CreateMultiPolygon(new[] { polygon });
            }

            var polygons = new List<Polygon>();
            var holes = new List<LinearRing>();

            for (int i = 1; i < parts.Count; i++)
            {
                // SHP Docs: Vertices of rings defining holes in polygons are in a counterclockwise direction.
                //           Vertices for a single, ringed polygon are, therefore, always in clockwise order.
                // Shell: !ring.IsCCW
                // Hole:   ring.IsCCW

                var ring = rings[i];
                if (ring.IsCCW)
                {
                    holes.Add(ring);
                }
                else
                {
                    // New polygon shell.

                    var polygon = Factory.CreatePolygon(shell, holes.ToArray());
                    polygons.Add(polygon);

                    shell = ring;
                    holes.Clear();
                }
            }

            var lastPolygon = Factory.CreatePolygon(shell, holes.ToArray());
            polygons.Add(lastPolygon);


            return Factory.CreateMultiPolygon(polygons.ToArray());
        }

        private MultiPolygon BuildMultiPolygonQuickFixingInvalidShapes(ShpMultiPartBuilder parts)
        {
            // 1. Do not sort rings but assume that polygons are serialized in the following order:
            //    Shell[, Holes][, Shell[, Holes][, ...]].
            // 2. Assume that the every ring that is not contained by previous polygon is the start of a new polygon.

            parts.CloseRings(CreateCoordinateSequence);

            var rings = parts
                .Where(p => p.Count == 0 || p.Count > 3)
                .Select(p => Factory.CreateLinearRing(p))
                .Where(lr => lr.IsValid)
                .ToList();

            if (rings.Count == 0)
            {
                return MultiPolygon.Empty;
            }

            var shell = !rings[0].IsCCW ? rings[0] : (LinearRing)rings[0].Reverse();
            if (rings.Count == 1)
            {
                var polygon = Factory.CreatePolygon(shell);
                return Factory.CreateMultiPolygon(new[] { polygon });
            }

            var polygons = new List<Polygon>();
            var holes = new List<LinearRing>();

            for (int i = 1; i < rings.Count; i++)
            {
                var ring = rings[i];

                // Do not check the whole ring. If one coordinate is inside the shell then all other should also be inside.
                if (shell.Contains(ring.Coordinate))
                {
                    // Shell: !ring.IsCCW
                    // Hole:   ring.IsCCW
                    ring = ring.IsCCW ? ring : (LinearRing)ring.Reverse();
                    holes.Add(ring);
                }
                else
                {
                    // New polygon shell.

                    var polygon = Factory.CreatePolygon(shell, holes.ToArray());
                    polygons.Add(polygon);

                    shell = ring.IsCCW ? (LinearRing)ring.Reverse() : ring;
                    holes.Clear();
                }
            }

            var lastPolygon = Factory.CreatePolygon(shell, holes.ToArray());
            polygons.Add(lastPolygon);

            return Factory.CreateMultiPolygon(polygons.ToArray());
        }

        private MultiPolygon BuildMultiPolygonFixingInvalidShapes(ShpMultiPartBuilder parts)
        {
            // 1. Sort all rings.
            // 2. Consider all rings as a potential shell, search the valid holes for any possible shell.
            // 3. Check if the ring is inside any shell. In that case it can be considered a potential hole for the shell.
            // 4. Check if the ring is inside any hole of the shell:
            //    - if true, this means that it is actually a shell of a distinct geometry,
            //    - if false, this means that it is a valid hole for the shell (a hole inside another hole is not allowed).

            parts.CloseRings(CreateCoordinateSequence);

            var rings = parts
                .Where(p => p.Count == 0 || p.Count > 3)
                .Select(p => Factory.CreateLinearRing(p))
                .Where(lr => lr.IsValid)
                .ToList();

            if (rings.Count == 0)
            {
                return MultiPolygon.Empty;
            }

            if (rings.Count == 1)
            {
                var shell = !rings[0].IsCCW ? rings[0] : (LinearRing)rings[0].Reverse();
                var polygon = Factory.CreatePolygon(shell);
                return Factory.CreateMultiPolygon(new[] { polygon });
            }

            rings.Sort(AscendingAreaComparison);
            var polygons = CreatePolygons(rings).ToArray();
            return Factory.CreateMultiPolygon(polygons);
        }

        private IEnumerable<Polygon> CreatePolygons(List<LinearRing> rings)
        {
            if (rings.Count < 1)
            {
                yield break;
            }

            if (rings.Count == 1)
            {
                yield return Factory.CreatePolygon(rings[0]);
            }

            while (rings.Count > 0)
            {
                // Shell: !ring.IsCCW
                // Hole:   ring.IsCCW
                var shell = rings.Extract(rings.Count - 1); // The largest (rings where already sorted)
                shell = !shell.IsCCW ? shell : (LinearRing)shell.Reverse();

                var innerRings = rings.ExtractInnerRings(shell);
                foreach (var polygon in CreateShellPolygonAndNestedPolygons(shell, innerRings))
                {
                    yield return polygon;
                }
            }
        }

        private IEnumerable<Polygon> CreateShellPolygonAndNestedPolygons(LinearRing shell, List<LinearRing> innerRings)
        {
            if (innerRings.Count <= 1)
            {
                yield return Factory.CreatePolygon(shell, innerRings.ToArray());
                yield break;
            }

            var holes = new List<LinearRing>();

            while (innerRings.Count > 0)
            {
                var hole = innerRings.Extract(innerRings.Count - 1); // The largest
                hole = hole.IsCCW ? hole : (LinearRing)hole.Reverse();
                holes.Add(hole);

                // Deeper nestings, aka island-in-a-lake-on-an-island-...
                // https://gis.stackexchange.com/a/147971/26684
                var nestedHoleRings = innerRings.ExtractInnerRings(hole);
                foreach (var nestedPolygon in CreatePolygons(nestedHoleRings))
                {
                    yield return nestedPolygon;
                }
            }
            yield return Factory.CreatePolygon(shell, holes.ToArray());
        }

        private static int AscendingAreaComparison(LinearRing ring1, LinearRing ring2)
        {
            var area1 = Area.OfRing(ring1.CoordinateSequence);
            var area2 = Area.OfRing(ring2.CoordinateSequence);
            return area1.CompareTo(area2);
        }

        private LinearRing CreateLinearRing(CoordinateSequence coordinateSequence, bool ignoreInvalid)
        {
            if (ignoreInvalid && coordinateSequence.Count > 0 && coordinateSequence.Count < 4)
            {
                return InvalidLinearRing;
            }

            if (ignoreInvalid && coordinateSequence.Count > 0)
            {
                var first = coordinateSequence.GetCoordinate(0);
                var last = coordinateSequence.GetCoordinate(coordinateSequence.Count - 1);
                if (!first.Equals(last))
                {
                    return InvalidLinearRing;
                }
            }

            return Factory.CreateLinearRing(coordinateSequence);
        }
    }
}
