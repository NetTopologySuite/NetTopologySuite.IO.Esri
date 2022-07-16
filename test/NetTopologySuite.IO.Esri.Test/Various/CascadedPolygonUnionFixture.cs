using NetTopologySuite.Geometries;
using NetTopologySuite.Operation.Overlay;
using NetTopologySuite.Operation.Overlay.Snap;
using NetTopologySuite.Operation.Union;
using NUnit.Framework;
using System;
using System.Linq;

namespace NetTopologySuite.IO.Esri.Test.Various
{
    [TestFixture]
    public class CascadedPolygonUnionFixture
    {
        protected GeometryFactory Factory { get; private set; }

        protected WKTReader Reader { get; private set; }

        public CascadedPolygonUnionFixture()
        {
            // Set current dir to shapefiles dir
            Environment.CurrentDirectory = CommonHelpers.TestShapefilesDirectory;

            this.Factory = new GeometryFactory();
            this.Reader = new WKTReader();
        }

        [Test]
        public void PerformCascadedPolygonUnion()
        {
            var reader = new ShapefileReader("tnp_pol.shp");
            var collection = reader.ReadAll().Where(e => e is Polygon).ToList();
            var u1 = collection[0];
            for (int i = 1; i < collection.Count; i++)
                u1 = SnapIfNeededOverlayOp.Overlay(u1, collection[i], SpatialFunction.Union);
            var u2 = CascadedPolygonUnion.Union(collection);
            if (!u1.EqualsTopologically(u2))
                Assert.Fail("failure");
        }
    }
}
