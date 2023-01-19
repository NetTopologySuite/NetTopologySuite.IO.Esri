using NetTopologySuite.Features;
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
    /// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/14
    /// </summary>
    internal class Issue014
    {
        [Test]
        public void CreateShapefileFromPlygonWkt()
        {
            var features = new List<Feature>();
            var wktReader = new WKTReader();
            var geometry = wktReader.Read(PolygonWkt);

            var attributes = new AttributesTable
            {
                { "Date", new DateTime(2022, 1, 1) },
                { "Content", $"I am No. 1" }
            };

            var feature = new Feature(geometry, attributes);
            features.Add(feature);

            var shpFile = TestShapefiles.GetTempShpPath();
            Shapefile.WriteAllFeatures(features, shpFile);
            TestShapefiles.DeleteShp(shpFile);
        }

        [Test]
        public void CreateShapefileFromLineStringWkt()
        {
            var features = new List<Feature>();
            var wktReader = new WKTReader();
            var geometry = wktReader.Read(LineStringWkt);

            var attributes = new AttributesTable
            {
                { "Date", new DateTime(2022, 1, 1) },
                { "Content", $"I am No. 1" }
            };

            var feature = new Feature(geometry, attributes);
            features.Add(feature);

            var shpFile = TestShapefiles.GetTempShpPath();
            Shapefile.WriteAllFeatures(features, shpFile);
            TestShapefiles.DeleteShp(shpFile);
        }

        #region Data

        private readonly string PolygonWkt = @"POLYGON((
            229884.458362927 2698919.1790506,229878.266318657 2698913.05244554,229872.637726088 2698914.0245398,
            229870.132848978 2698912.76299454,229868.585854503 2698911.60799823,229862.417875893 2698913.56799102,
            229859.466886255 2698913.2179918,229854.168905161 2698909.4280045,229832.918531065 2698900.75121021,
            229826.266555421 2698903.87487001,229815.242942057 2698911.30138916,229814.158096991 2698912.0322406,
            229802.736136414 2698917.5934203,229787.107090625 2698922.43900198,229770.41744862 2698926.49898628,
            229758.597689508 2698931.31876844,229754.622568952 2698933.63004125,229756.82934315 2698935.98290305,
            229758.154520219 2698937.39368305,229761.467578897 2698937.10864875,229772.427440716 2698935.38875568,
            229785.926793536 2698934.84915899,229801.796838123 2698933.66916467,229811.707603543 2698932.67086896,
            229818.576179578 2698931.9792722,229834.383317175 2698930.1646309,229840.615502711 2698929.44918301,
            229859.255237777 2698926.54949485,229875.612191821 2698922.46243583,229879.34506797 2698921.52971422,
            229884.458362927 2698919.1790506))";

        private readonly string LineStringWkt = @"LINESTRING(
            229884.458362927 2698919.1790506,229878.266318657 2698913.05244554,229872.637726088 2698914.0245398)";

        #endregion
    }
}
