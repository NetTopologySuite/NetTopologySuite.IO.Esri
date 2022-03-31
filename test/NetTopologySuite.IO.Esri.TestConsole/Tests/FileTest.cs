namespace NetTopologySuite.IO.Esri.TestConsole.Tests
{
    public abstract class FileTest : Test
    {
        protected readonly string Path;

        public FileTest(string dbfPath)
        {
            Path = dbfPath;
        }

        public override string ToString()
        {
            return $"{Title}: {Path}";
        }
    }
}
