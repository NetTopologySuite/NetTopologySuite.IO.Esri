﻿using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test.Writers
{
    public class WKTReaderTests
    {
        private readonly WKTReader _wktReader;

        public WKTReaderTests()
        {
            _wktReader = new WKTReader(new GeometryFactory(new PrecisionModel(), 4326)) { IsOldNtsCoordinateSyntaxAllowed = false };
        }

        [Test]
        public void TestShapeType()
        {
            string wkt = "POLYGON ((-86.7605020509258 41.5101338613656, -86.7604972038273 41.5100611525915, -86.7604971708084 41.5100606308085, -86.7604611720717 41.5094596307695, -86.7604611426546 41.5094591103497, -86.7604291439208 41.5088571103154, -86.760429130715 41.508856853856, -86.7603991319814 41.5082548538241, -86.7603991259966 41.5082547317887, -86.7603701303631 41.5076537960468, -86.7603401446338 41.5070530565908, -86.7603071566895 41.5064532528163, -86.7603071500912 41.506453131098, -86.7602814240795 41.5059715533315, -86.7605549835241 41.5059607024218, -86.7605808466407 41.5064448078787, -86.760613844555 41.5070447469854, -86.7606138651484 41.5070451395365, -86.7606438664126 41.5076461395046, -86.7606438727239 41.5076462680791, -86.7606728710439 41.5082472070294, -86.7607028628788 41.5088490177453, -86.7607348434949 41.5094506292495, -86.7607708135428 41.5100511081057, -86.760776407335 41.5101350123382, -86.7605020509258 41.5101338613656))";
            var geometry = (Polygon)_wktReader.Read(wkt);

            Assert.IsTrue(geometry.Shell.CoordinateSequence.Ordinates == Ordinates.XY);
            Assert.IsTrue(Shapefile.GetShapeType(geometry) == ShapeType.Polygon);
        }
    }
}
