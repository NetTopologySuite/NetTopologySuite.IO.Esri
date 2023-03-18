using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Xml.Linq;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    /// <summary>
    /// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/25
    /// </summary>
    internal class Issue025
    {
        [TestCase("Issues/025/UKCS_Licences_WGS84.shp")]
        [TestCase("Issues/025/UKCS_Licensed_Blocks_WGS84.shp")]
        [TestCase("Issues/025/UKCS_SubAreas_WGS84.shp")]
        public void ReadInvalidPolygons(string shpName)
        {
            var shpPath = TestShapefiles.PathTo(shpName);

            var ignoreOpts = new ShapefileReaderOptions()
            {
                GeometryBuilderMode = GeometryBuilderMode.IgnoreInvalidShapes
            };
            var ingoreFeatures = Shapefile.ReadAllFeatures(shpPath, ignoreOpts);

            var skipOpts = new ShapefileReaderOptions()
            {
                GeometryBuilderMode = GeometryBuilderMode.SkipInvalidShapes
            };
            var skipFeatures = Shapefile.ReadAllFeatures(shpPath, skipOpts);

            Assert.IsNotNull(ingoreFeatures);
            Assert.IsNotNull(skipFeatures);
            Assert.AreNotEqual(ingoreFeatures.Length, skipFeatures.Length);
        }
    }
}
