using NetTopologySuite.Geometries;
using NUnit.Framework;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.Issues
{
    class Issue161
    {
        [Test(Description =
            "ShapefileDataReader error 'The output char buffer is too small to contain the decoded characters'")]
        public void TestIssue161()
        {
            //SETUP
            // https://webarchive.nationalarchives.gov.uk/ukgwa/20160110200248/http://www.ons.gov.uk/ons/guide-method/geography/products/census/spatial/2011/index.html
            string filePath = TestShapefiles.PathTo("LSOA_2011_EW_BGC.shp");
            if (!File.Exists(filePath)) Assert.Ignore("File '{0}' not present", filePath);

            //ATTEMPT
            using (var reader = Shapefile.OpenRead(filePath))
            {
                while (reader.Read(out bool deleted))//&& count++ < 3)
                {
                    object val;
                    Assert.DoesNotThrow(() => val = reader.Fields["LSOA11CD"].Value);
                }
            }
        }
    }
}
