using System.IO;

namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public class CopyAndCompareArcMapShapefilesTest : CompareShapefilesTest
    {
        public override void Run()
        {
            var filePath = GetTestFilePath("arcmap/shp/point.shp");
            var shpDataDir = Path.GetDirectoryName(filePath);

            foreach (var shpFilePath in Directory.GetFiles(shpDataDir, "*.shp"))
            {
                //if (!shpFilePath.Contains("\\point_m"))
                //    continue;

                var copyFilePath = CopyShapefile(shpFilePath);
                CompareShapefiles(shpFilePath, copyFilePath);
            }
        }
    }
}
