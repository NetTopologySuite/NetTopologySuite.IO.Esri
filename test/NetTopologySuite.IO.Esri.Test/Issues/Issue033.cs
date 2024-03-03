using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetTopologySuite.IO.Esri.Test.Issues;

/// <summary>
/// https://github.com/NetTopologySuite/NetTopologySuite.IO.Esri/issues/33
/// </summary>
internal class Issue033
{
    [Test]
    public void WriteAllFeatures_NullAttributeValue()
    {
        string shpPath = TestShapefiles.PathTo(@"Issues/03/sample/ne_10m_admin_0_countries_fra.shp");
        string shpCopyPath = TestShapefiles.GetTempShpPath();

        var config = new ShapefileReaderOptions()
        {
            GeometryBuilderMode = GeometryBuilderMode.IgnoreInvalidShapes
        };

        var features = Shapefile.ReadAllFeatures(shpPath, config);
        Shapefile.WriteAllFeatures(features, shpCopyPath);
    }
}
