using NetTopologySuite.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries.Implementation;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    [TestFixture]
    public class Issue4Fixture
    {
        private static string CreateShapefilePath()
        {
            string path = Path.GetTempFileName();
            path = Path.ChangeExtension(path, string.Empty).TrimEnd('.');
            return path;
        }

        private static Coordinate[] CreateCoords()
        {
            IList<Coordinate> coordinates = new List<Coordinate>();
            coordinates.Add(new Coordinate(0, 0));
            coordinates.Add(new Coordinate(1, 0));
            coordinates.Add(new Coordinate(1, 1));
            coordinates.Add(new Coordinate(0, 1));
            coordinates.Add(new Coordinate(0, 0));
            return coordinates.ToArray();
        }

        private static CoordinateSequence CopyToSequence(Coordinate[] coords, CoordinateSequence sequence)
        {
            for (int i = 0; i < coords.Length; i++)
            {
                sequence.SetOrdinate(i, Ordinate.X, coords[i].X);
                sequence.SetOrdinate(i, Ordinate.Y, coords[i].Y);
            }
            return sequence;
        }

        [Test]
        public void shapefile_with_empty_attributes_table_should_not_thrown_errors()
        {
            IFeature feature = new Feature(new Point(0, 0), new AttributesTable());
            IList<IFeature> features = new List<IFeature> { feature };

            string path = CreateShapefilePath();
            var header = ShapefileDataWriter.GetHeader(feature, features.Count);
            var writer = new ShapefileDataWriter(path) { Header = header };
            writer.Write(features);
            Assert.That(File.Exists(Path.ChangeExtension(path, ".shp")), Is.True);
        }

        [Test]
        public void create_xyonly_geom_using_sequence_and_dimension_two()
        {
            var coords = CreateCoords();

            var factory = CoordinateArraySequenceFactory.Instance;
            var sequence = CopyToSequence(coords, factory.Create(coords.Length, 2));

            var polygon = GeometryFactory.Default.CreatePolygon(sequence);
            Assert.That(polygon, Is.Not.Null);
            Assert.That(polygon.Shell, Is.Not.Null);
            Assert.That(polygon.Shell.CoordinateSequence.Dimension, Is.EqualTo(2));
        }

        [Test]
        public void create_xyonly_geom_using_sequence_and_ordinates_xy()
        {
            var coords = CreateCoords();

            var factory = CoordinateArraySequenceFactory.Instance;
            var sequence = CopyToSequence(coords, factory.Create(coords.Length, Ordinates.XY));

            var polygon = GeometryFactory.Default.CreatePolygon(sequence);
            Assert.That(polygon, Is.Not.Null);
            Assert.That(polygon.Shell, Is.Not.Null);
            Assert.That(polygon.Shell.CoordinateSequence.Dimension, Is.EqualTo(2));
        }
    }
}
