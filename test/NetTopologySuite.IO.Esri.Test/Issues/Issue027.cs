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
    internal class Issue027
    {
        [Test]
        public void ReadInvalidDate()
        {
            var shpPath = TestShapefiles.PathTo("Issues/027/faults_bounddaries.shp");
            var features = Shapefile
                .ReadAllFeatures(shpPath)
                .ToList(); // Chekc if all attributes have been read corectly.

            var firstFeature = features.FirstOrDefault();

            Assert.IsNotNull(firstFeature);
            Assert.IsNull(firstFeature.Attributes["MAPDATE"]);
        }
    }
}
