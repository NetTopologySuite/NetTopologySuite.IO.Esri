using System.Linq;
using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test.Various
{
    /// <summary>
    ///
    /// </summary>
    [TestFixture]
    public class NormalizeTest
    {
        protected GeometryFactory Factory { get; private set; }

        protected WKTReader Reader { get; private set; }

        private Polygon _polygon = null;
        private LinearRing _shell = null;
        private LinearRing _hole = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="NormalizeTest"/> class.
        /// </summary>
        public NormalizeTest() : base() { }

        /// <summary>
        /// Method called prior to every test in this fixture
        /// </summary>
        [SetUp]
        public void Init()
        {
            this.Factory = new GeometryFactory();
            this.Reader = new WKTReader();

            // NOTE: Shell is created with not correct order of coordinates (should be clockwise order)
            _shell = Factory.CreateLinearRing(new Coordinate[] {    new Coordinate(100,100),
                                                                    new Coordinate(200,100),
                                                                    new Coordinate(200,200),
                                                                    new Coordinate(100,200),
                                                                    new Coordinate(100,100), });

            _hole = Factory.CreateLinearRing(new Coordinate[] {      new Coordinate(120,120),
                                                                    new Coordinate(180,120),
                                                                    new Coordinate(180,180),
                                                                    new Coordinate(120,180),
                                                                    new Coordinate(120,120), });
            _polygon = Factory.CreatePolygon(_shell, new LinearRing[] { _hole, });
        }

        /// <summary>
        ///
        /// </summary>
        [Test]
        public void NotNormalizedGDBOperation()
        {
            var shpPath = TestShapefiles.GetTempShpPath();
            using (var shpWriter = Shapefile.OpenWrite(shpPath, new(ShapeType.Polygon)))
            {
                shpWriter.Geometry = new MultiPolygon(new[] { _polygon });
                shpWriter.Write();
            }
            var firstGeometry = Shapefile.ReadAllGeometries(shpPath).First();
            var test = firstGeometry.GetGeometryN(0) as Polygon;
            Assert.IsNotNull(test);

            Assert.IsTrue(test is IPolygonal);
            Assert.IsFalse(_polygon.EqualsExact(test)); // SHP shells and holes are always written with correct coordinate order


        }

        /// <summary>
        ///
        /// </summary>
        [Test]
        public void NormalizedGDBOperation()
        {
            _polygon.Normalize();

            var shpPath = TestShapefiles.GetTempShpPath();
            using (var shpWriter = Shapefile.OpenWrite(shpPath, new(ShapeType.Polygon)))
            {
                shpWriter.Geometry = new MultiPolygon(new[] { _polygon });
                shpWriter.Write();
            }
            var firstGeometry = Shapefile.ReadAllGeometries(shpPath).First();
            var test = firstGeometry.GetGeometryN(0) as Polygon;
            Assert.IsNotNull(test);

            Assert.IsNotNull(test);
            Assert.IsTrue(test is IPolygonal);
            Assert.IsTrue(_polygon.EqualsExact(test));
        }
    }
}
