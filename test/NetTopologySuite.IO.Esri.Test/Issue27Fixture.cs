using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.IO.ShapeFile.Extended;
using NetTopologySuite.IO.ShapeFile.Extended.Entities;
using System.IO;

namespace NetTopologySuite.IO.ShapeFile.Test
{
    [TestFixture]
    [ShapeFileIssueNumber(27)]
    public class Issue27Fixture
    {
        /// <summary>
        /// <see href="https://github.com/NetTopologySuite/NetTopologySuite.IO.ShapeFile/issues/27"/>
        /// </summary>
        [Test]
        public void Data_should_be_readable_after_reader_dispose()
        {
            var crustal_test = Path.Combine(CommonHelpers.TestShapefilesDirectory, "crustal_test.shp");
            Assert.True(File.Exists(crustal_test));

            List<IShapefileFeature> data = null;
            using (var reader = new ShapeDataReader(crustal_test))
            {
                var mbr = reader.ShapefileBounds;
                data = reader.ReadByMBRFilter(mbr).ToList();
            }
            Assert.IsNotNull(data);
            Assert.IsNotEmpty(data);

            foreach (var item in data)
            {
                Assert.IsNotNull(item.Geometry);
                Assert.IsNotNull(item.Attributes["ID_GTR"]);
            }
        }
    }
}
