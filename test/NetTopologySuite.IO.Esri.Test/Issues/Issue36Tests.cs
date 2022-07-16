using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetTopologySuite.IO.Esri.Test.Issues
{
    /// <summary>
    /// see: https://github.com/NetTopologySuite/NetTopologySuite/issues/36
    /// </summary>
    [TestFixture]
    public class Issue36Tests
    {
        private int _numPassed;

        [SetUp]
        public void SetUp()
        {
            // Set current dir to shapefiles dir
            Environment.CurrentDirectory = CommonHelpers.TestShapefilesDirectory;

            _numPassed = 0;
        }

        [Test]
        public void ok_when_writing_shapefile_with_features()
        {
            var header = new DbaseFileHeader();
            header.AddColumn("X", 'C', 10, 0);
            var writer = new ShapefileDataWriter(@"issue36") { Header = header };

            IAttributesTable attributesTable = new AttributesTable();
            attributesTable.Add("X", "y");
            IFeature feature = new Feature(new Point(1, 2), attributesTable);

            IList<IFeature> features = new List<IFeature>();
            features.Add(feature);

            Assert.DoesNotThrow(() => writer.Write(features));

            _numPassed++;
        }

        [Test]
        public void ok_when_writing_shapefile_with_no_features()
        {
            var header = new DbaseFileHeader();
            header.AddColumn("X", 'C', 10, 0);
            var writer = new ShapefileDataWriter(@"issue36") { Header = header };

            IList<IFeature> features = new List<IFeature>();
            Assert.DoesNotThrow(() => writer.Write(features));

            _numPassed++;
        }

        [TearDown]
        public void TearDown()
        {
            if (_numPassed < 2) return;

            // Clean up!
            File.Delete("issue36.dbf");
            File.Delete("issue36.shp");
            File.Delete("issue36.shx");
        }
    }
}
