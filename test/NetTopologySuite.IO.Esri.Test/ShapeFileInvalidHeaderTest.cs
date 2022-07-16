using NUnit.Framework;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test
{
    [TestFixture]
    [Ignore("Sample file(s) not published")]
    public class ShapeFileInvalidHeaderTest
    {
        private readonly string _invalidPath = Path.Combine(CommonHelpers.TestShapefilesDirectory, "invalidheader.shp");

        [Test]
        public void TestInvalidShapeFile()
        {
            /*
            var s = new NetTopologySuite.IO.ShapefileReader(_invalidPath);
            var sh = s.Header;
            var g = s.ReadAll();
            */
            string dbf = Path.ChangeExtension(_invalidPath, ".dbf");
            var d = new NetTopologySuite.IO.DbaseFileReader(dbf);

            var de = d.GetEnumerator();
            Assert.IsNull(de.Current);
            de.MoveNext();
            Assert.IsNotNull(de.Current);
        }
    }
}