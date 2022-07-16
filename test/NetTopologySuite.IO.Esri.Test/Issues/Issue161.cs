using NetTopologySuite.Geometries;
using NUnit.Framework;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    class Issue161
    {
        [Test(Description =
            "ShapefileDataReader error 'The output char buffer is too small to contain the decoded characters'")]
        public void TestIssue161()
        {
            //SETUP
            string filePath = Path.Combine(CommonHelpers.TestShapefilesDirectory, "LSOA_2011_EW_BGC.shp");
            if (!File.Exists(filePath)) Assert.Ignore("File '{0}' not present", filePath);

            //ATTEMPT
            using (var reader = new ShapefileDataReader(filePath, GeometryFactory.Default))
            {
                var header = reader.ShapeHeader;

                while (reader.Read())//&& count++ < 3)
                {
                    object val;
                    Assert.DoesNotThrow(() => val = reader["LSOA11CD"]);
                }
            }
        }
    }
}
