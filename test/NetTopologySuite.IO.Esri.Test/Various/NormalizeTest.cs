using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace NetTopologySuite.IO.ShapeFile.Test.Various
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

            _shell = Factory.CreateLinearRing(new Coordinate[] {    new Coordinate(100,100),
                                                                    new Coordinate(200,100),
                                                                    new Coordinate(200,200),
                                                                    new Coordinate(100,200),
                                                                    new Coordinate(100,100), });
            // NOTE: Hole is created with not correct order for holes
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
            byte[] bytes = new GDBWriter().Write(_polygon);
            var test = new GDBReader().Read(bytes);

            //This is no longer true
            //Assert.IsNull(test);
            Assert.IsTrue(test.IsEmpty);
            Assert.IsTrue(test is IPolygonal);
        }

        /// <summary>
        ///
        /// </summary>
        [Test]
        public void NormalizedGDBOperation()
        {
            _polygon.Normalize();

            byte[] bytes = new GDBWriter().Write(_polygon);
            var test = new GDBReader().Read(bytes);

            Assert.IsNotNull(test);
            Assert.IsTrue(_polygon.EqualsExact(test));
        }
    }
}
