using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    /// <summary>
    /// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/11
    /// </summary>
    internal class Issue011
    {
        [Test]
        public void ReadNumericIdField()
        {
            var options = new ShapefileReaderOptions()
            {
                GeometryBuilderMode = GeometryBuilderMode.IgnoreInvalidShapes
            };
            var shpPath = TestShapefiles.PathTo("Issues/011/WallaceMonument.shp");
            var features = Shapefile.ReadAllFeatures(shpPath, options);
            Assert.AreEqual(1, features.Length);

            var firstFeature = features.First();
            Assert.IsNull(firstFeature.Attributes["id"]);
        }
    }
}
