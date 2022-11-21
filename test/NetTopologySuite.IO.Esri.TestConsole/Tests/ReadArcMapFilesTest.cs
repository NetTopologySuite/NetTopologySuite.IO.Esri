using System.IO;

namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public class ReadArcMapFilesTest : Test
    {
        public override void Run()
        {
            var filePath = GetTestFilePath("arcmap/shp/point.shp");
            var shpDataDir = Path.GetDirectoryName(filePath) ?? "shp_dir";

            foreach (var shpFilePath in Directory.GetFiles(shpDataDir, "*.shp"))
            {
                var fileName = "arcmap/shp/" + Path.GetFileName(shpFilePath);
                PrintSectionTitle(fileName);

                using var shapefile = Shapefile.OpenRead(shpFilePath);
                PrintFeatures(shapefile);
            }
        }
    }
}
