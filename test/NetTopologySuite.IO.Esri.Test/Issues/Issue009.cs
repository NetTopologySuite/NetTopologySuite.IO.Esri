using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.IO.Esri.Dbf;
using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using NUnit.Framework;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    /// <summary>
    /// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/9
    /// </summary>
    internal class Issue009
    {

        [Test]
        public void Read_tmp3509()
        {
            var options = new ShapefileReaderOptions()
            {
                GeometryBuilderMode = GeometryBuilderMode.IgnoreInvalidShapes
            };
            var shpPath = TestShapefiles.PathTo("Issues/009/tmp3509/doc/Bonifiche Ferraresi-Jolanda_1-203-super urea.shp");
            var features = Shapefile.ReadAllFeatures(shpPath, options);
            Assert.AreEqual(1204, features.Count());

            Console.WriteLine("Invalid shape geometries:");
            foreach (var feature in features)
            {
                if (!feature.Geometry.IsValid)
                {
                    Console.WriteLine(string.Join(", ", feature.Attributes.GetValues()));
                }
            }
        }

        [Test]
        public void Read_tmpA6A6()
        {
            var options = new ShapefileReaderOptions()
            {
                GeometryBuilderMode = GeometryBuilderMode.IgnoreInvalidShapes
            };
            var shpPath = TestShapefiles.PathTo("Issues/009/tmpA6A6/Rx/PPPP29_0.shp");
            var features = Shapefile.ReadAllFeatures(shpPath, options);
            Assert.AreEqual(50, features.Count());

            Console.WriteLine("Invalid shape geometries:");
            foreach (var feature in features)
            {
                if (!feature.Geometry.IsValid)
                {
                    Console.WriteLine(string.Join(", ", feature.Attributes.GetValues()));
                }
            }
        }
    }
}
