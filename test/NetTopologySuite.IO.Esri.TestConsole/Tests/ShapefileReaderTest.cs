namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public class ShapefileReaderTest : FileTest
    {

        public ShapefileReaderTest(string path) : base(path)
        {
            Title = "Read SHP file";
        }

        public override void Run()
        {
            var fullPath = GetTestFilePath(Path);

            using (var shp = Shapefile.OpenRead(fullPath))
            {
                PrintFeatures(shp);
            }
        }
    }
}
