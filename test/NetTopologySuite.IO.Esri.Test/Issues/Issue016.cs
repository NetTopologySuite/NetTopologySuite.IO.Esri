using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.Esri.Dbf.Fields;
using NetTopologySuite.IO.Esri.Shapefiles.Writers;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    /// <summary>
    /// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/16
    /// </summary>
    internal class Issue016
    {
        [Test]
        public void StoreProjectionUsingWriter()
        {
            var geoReader = new GeoJsonReader();
            var features = geoReader.Read<FeatureCollection>(GeoJsonData);

            var prop0 = new DbfCharacterField("prop0");
            var prop1 = new DbfCharacterField("prop1");
            var options = new ShapefileWriterOptions(ShapeType.Polygon, prop0, prop1)
            {
                Projection = @"GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]]"
            };

            var shpPath = TestShapefiles.GetTempShpPath(); // "C:\\Temp\\Issue016.shp";
            using (var shpWriter = Shapefile.OpenWrite(shpPath, options))
            {
                shpWriter.Write(features);
            }

            using (var shpReader = Shapefile.OpenRead(shpPath))
            {
                Assert.AreEqual(features.Count, shpReader.RecordCount);
                Assert.AreEqual(options.Projection, shpReader.Projection);
            }

            TestShapefiles.DeleteShp(shpPath);
        }

        [Test]
        public void StoreProjectionWriteAllFeaturesMethod()
        {
            var geoReader = new GeoJsonReader();
            var features = geoReader.Read<FeatureCollection>(GeoJsonData);

            var projection = @"GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]]";
            var shpPath = TestShapefiles.GetTempShpPath();
            Shapefile.WriteAllFeatures(features, shpPath, projection);

            using (var shpReader = Shapefile.OpenRead(shpPath))
            {
                Assert.AreEqual(features.Count, shpReader.RecordCount);
                Assert.AreEqual(projection, shpReader.Projection);
            }

            TestShapefiles.DeleteShp(shpPath);
        }

        #region Data

        /// <summary>
        /// Credits to Wikipedia.
        /// <seealso cref="https://en.wikipedia.org/wiki/GeoJSON"/>
        /// </summary>
        private static readonly string GeoJsonData = @"
                {
                  ""type"": ""FeatureCollection"",
                  ""features"": [
                    {
                      ""type"": ""Feature"",
                      ""geometry"": {
                        ""type"": ""MultiPolygon"",
                        ""coordinates"": [
                            [
                                [[40.0, 40.0], [20.0, 45.0], [45.0, 30.0], [40.0, 40.0]]
                            ], 
                            [
                                [[20.0, 35.0], [10.0, 30.0], [10.0, 10.0], [30.0, 5.0], [45.0, 20.0], [20.0, 35.0]], 
                                [[30.0, 20.0], [20.0, 15.0], [20.0, 25.0], [30.0, 20.0]]
                            ]
                        ]
                      },
                      ""properties"": {
                        ""prop0"": ""value0"",
                        ""prop1"": ""that"" 
                      }
                    }
                  ]
                }
            ";

        #endregion
    }
}
