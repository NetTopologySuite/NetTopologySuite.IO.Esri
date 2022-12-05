using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Index.Quadtree;
using NetTopologySuite.IO.Esri.Shapefiles.Readers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.Features
{

    public class UseOfIndexAndPreparedGeometry
    {
        [TestCase(@"eurostat/countries_pt.shp")]
        [TestCase(@"eurostat/countries_ln.shp")]
        [TestCase(@"eurostat/countries_pg.shp")]
        public void TestShapefile(string shapefile)
        {
            shapefile = TestShapefiles.PathTo(shapefile);
            if (!System.IO.File.Exists(shapefile))
                throw new Exception("file not present: " + shapefile);

            var featureCollection = new Collection<IFeature>();
            Envelope bbox = GetBoundingBox(shapefile);
            var options = new ShapefileReaderOptions()
            {
                MbrFilter = bbox
            };
            using var shp = Shapefile.OpenRead(shapefile, options);
            foreach (var shapefileFeature in shp)
            { 
                featureCollection.Add(shapefileFeature);
            }

            const double min1 = 0.4;
            const double min2 = 0.5 - min1;

            var rnd = new Random(8888);
            var queryBox = new Envelope(
                bbox.MinX + (min1 + rnd.NextDouble() * min2) * bbox.Width,
                bbox.MaxX - (min1 + rnd.NextDouble() * min2) * bbox.Width,
                bbox.MinY + (min1 + rnd.NextDouble() * min2) * bbox.Height,
                bbox.MaxY - (min1 + rnd.NextDouble() * min2) * bbox.Height);

            var shape = new SineStarFactory(GeometryFactory.Default) { Envelope = queryBox }.CreateSineStar();
            TestShapefilePlain(featureCollection, shape, "intersects");
            TestShapefilePlain(featureCollection, shape, "intersects");
            TestShapefilePrepared(featureCollection, shape, "intersects");
            TestShapefileIndexed(featureCollection, shape, "intersects");
        }

        private Envelope GetBoundingBox(string shapefile)
        {
            using var shp = Shapefile.OpenRead(shapefile);
            return shp.BoundingBox;
        }

        private void TestShapefilePrepared(ICollection<IFeature> features, Geometry queryGeom, string spatialPredicate)
        {
            System.Diagnostics.Debug.WriteLine("\nPrepared");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            System.Diagnostics.Debug.WriteLine("Setup collection: {0}ms", sw.ElapsedMilliseconds);
            sw.Restart();
            var prep = NetTopologySuite.Geometries.Prepared.PreparedGeometryFactory.Prepare(queryGeom);
            var lst = new List<IFeature>(2048);
            foreach (var feature in features)
            {
                if (prep.Intersects(feature.Geometry))
                    lst.Add(feature);
            }
            System.Diagnostics.Debug.WriteLine("Query collection: {0}ms", sw.ElapsedMilliseconds);
            System.Diagnostics.Debug.WriteLine("Queried {0} features of a set of {1}", lst.Count, features.Count);
        }

        private void TestShapefilePlain(ICollection<IFeature> features, Geometry queryGeom, string spatialPredicate)
        {
            System.Diagnostics.Debug.WriteLine("\nPlain");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            System.Diagnostics.Debug.WriteLine("Setup collection: {0}ms", sw.ElapsedMilliseconds);
            sw.Restart();
            var lst = new List<IFeature>(2048);
            foreach (var feature in features)
            {
                if (queryGeom.Intersects(feature.Geometry))
                    lst.Add(feature);
            }
            System.Diagnostics.Debug.WriteLine("Query collection: {0}ms", sw.ElapsedMilliseconds);
            System.Diagnostics.Debug.WriteLine("Queried {0} features of a set of {1}", lst.Count, features.Count);
        }

        private void TestShapefileIndexed(ICollection<IFeature> features, Geometry queryGeom, string spatialPredicate)
        {
            System.Diagnostics.Debug.WriteLine("\nIndexed");
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var idxFeatureCollection = new IndexedFeatureColection();
            foreach (var feature in features)
                idxFeatureCollection.Add(feature);
            System.Diagnostics.Debug.WriteLine("Setup collection: {0}ms", sw.ElapsedMilliseconds);
            sw.Restart();
            var lst = new List<IFeature>(idxFeatureCollection.Query(queryGeom, spatialPredicate));
            System.Diagnostics.Debug.WriteLine("Query collection: {0}ms", sw.ElapsedMilliseconds);
            System.Diagnostics.Debug.WriteLine("Queried {0} features of a set of {1}", lst.Count, features.Count);
        }

    }

    public class IndexedFeatureColection : Collection<IFeature>, ICollection<IFeature>
    {
        private Quadtree<int> _featuresIndex;

        private class FeatureComparer : EqualityComparer<IFeature>
        {
            public override bool Equals(IFeature x, IFeature y)
            {
                if (x == null || y == null) throw new ArgumentNullException();

                if (x.Attributes == null && y.Attributes != null)
                    return false;

                if (x.Attributes != null && y.Attributes == null)
                    return false;

                if (x.Attributes.Count != y.Attributes.Count)
                    return false;

                string[] names = x.Attributes.GetNames();
                foreach (string name in names)
                {
                    object v1 = x.Attributes[name];
                    object v2 = y.Attributes[name];
                    if (!v1.Equals(v2)) return false;
                }

                if (!x.Geometry.EqualsTopologically(y.Geometry))
                    return false;

                return true;
            }

            public override int GetHashCode(IFeature obj)
            {
                int res = obj.Geometry.GetHashCode();
                if (obj.Attributes != null)
                {
                    foreach (object value in obj.Attributes.GetValues())
                        res ^= value.GetHashCode();
                }
                return res;
            }
        }

        private readonly FeatureComparer _featureComparer = new FeatureComparer();

        public IndexedFeatureColection()
        {
            _featuresIndex = new Quadtree<int>();
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            _featuresIndex = new Quadtree<int>();
        }

        protected override void InsertItem(int index, IFeature item)
        {
            base.InsertItem(index, item);
            _featuresIndex.Insert(item.BoundingBox, index);
        }

        protected override void RemoveItem(int index)
        {
            _featuresIndex.Remove(this[index].BoundingBox, index);
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, IFeature item)
        {
            var oldItem = this[index];
            _featuresIndex.Remove(oldItem.Geometry.EnvelopeInternal, index);
            base.SetItem(index, item);
            _featuresIndex.Insert(item.Geometry.EnvelopeInternal, index);
        }

        public new bool Contains(IFeature item)
        {
            var indices = _featuresIndex.Query(item.BoundingBox);
            foreach (int tmpItem in indices)
            {
                var feature = this[tmpItem];
                if (_featureComparer.Equals(item, feature))
                    return true;
            }
            return false;
        }

        public IEnumerable<IFeature> Query(Geometry geom, string spatialPredicate)
        {
            if (geom == null)
                throw new ArgumentNullException("geom");

            var prepgeom = NetTopologySuite.Geometries.Prepared.PreparedGeometryFactory.Prepare(geom);
            switch (spatialPredicate.Trim().ToLowerInvariant())
            {
                case "intersects":
                    return QueryInternal(geom.EnvelopeInternal, prepgeom.Intersects);
                case "contains":
                    return QueryInternal(geom.EnvelopeInternal, prepgeom.Contains);
                case "containsproperly":
                    return QueryInternal(geom.EnvelopeInternal, prepgeom.ContainsProperly);
                case "coveredby":
                    return QueryInternal(geom.EnvelopeInternal, prepgeom.CoveredBy);
                case "covers":
                    return QueryInternal(geom.EnvelopeInternal, prepgeom.Covers);
                case "crosses":
                    return QueryInternal(geom.EnvelopeInternal, prepgeom.Crosses);
                case "disjoint":
                    return QueryInternal(geom.EnvelopeInternal, prepgeom.Disjoint);
                case "overlaps":
                    return QueryInternal(geom.EnvelopeInternal, prepgeom.Overlaps);
                case "within":
                    return QueryInternal(geom.EnvelopeInternal, prepgeom.Within);
                case "touches":
                    return QueryInternal(geom.EnvelopeInternal, prepgeom.Touches);

            }
            return null;
        }

        private IEnumerable<IFeature> QueryInternal(Envelope bbox, Func<Geometry, bool> spatialPredicate)
        {
            Contract.Assert(bbox != null);
            Contract.Assert(spatialPredicate != null);

            foreach (int index in _featuresIndex.Query(bbox))
            {
                var feature = this[index];
                if (spatialPredicate(feature.Geometry))
                    yield return feature;
            }
        }
    }
}
