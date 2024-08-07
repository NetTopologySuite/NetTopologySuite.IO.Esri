﻿using NetTopologySuite.Geometries;
using System;
using NUnit.Framework;
using NetTopologySuite.Operation.Buffer;
using NetTopologySuite.Features;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.Geometries
{
    /// <summary>
    ///
    /// </summary>
    public class LineStringSamples
    {
        protected GeometryFactory Factory { get; private set; }

        protected WKTReader Reader { get; private set; }

        private LineString line = null;

        /// <summary>
        ///
        /// </summary>
        public LineStringSamples() : base()
        {
            this.Factory = new GeometryFactory();
            this.Reader = new WKTReader();

            var coordinates = new Coordinate[]
            {
                 new Coordinate(10, 10),
                 new Coordinate(20, 20),
                 new Coordinate(20, 10),
            };
            line = Factory.CreateLineString(coordinates);
        }

        [Test]
        public void Start()
        {
            var pointInLine = Factory.CreatePoint(new Coordinate(20, 10));
            var pointOutLine = Factory.CreatePoint(new Coordinate(20, 31));
            var aLine = Factory.CreateLineString(new Coordinate[] { new Coordinate(23, 32.2), new Coordinate(922, 11) });
            var anotherLine = Factory.CreateLineString(new Coordinate[] { new Coordinate(0, 1), new Coordinate(30, 30) });

            Write(line.Area);
            Write(line.Boundary);
            Write(line.BoundaryDimension);
            Write(line.Centroid);
            Write(line.Coordinate);
            Write(line.Coordinates);
            Write(line.CoordinateSequence);
            Write(line.Dimension);
            Write(line.EndPoint);
            Write(line.Envelope);
            Write(line.EnvelopeInternal);
            Write(line.InteriorPoint);
            Write(line.IsClosed);
            Write(line.IsEmpty);
            Write(line.IsRing);
            Write(line.IsSimple);
            Write(line.IsValid);
            Write(line.Length);
            Write(line.NumPoints);
            Write(line.StartPoint);
            if (line.UserData != null)
                Write(line.UserData);
            else Write("UserData null");

            Write(line.Buffer(10));
            Write(line.Buffer(10, new BufferParameters { EndCapStyle = EndCapStyle.Flat }));
            Write(line.Buffer(10, new BufferParameters { EndCapStyle = EndCapStyle.Square }));
            Write(line.Buffer(10, 20));
            Write(line.Buffer(10, new BufferParameters(20) { EndCapStyle = EndCapStyle.Flat }));
            Write(line.Buffer(10, new BufferParameters(20) { EndCapStyle = EndCapStyle.Square }));
            Write(line.Contains(pointInLine));
            Write(line.Contains(pointOutLine));
            Write(line.Crosses(pointInLine));
            Write(line.Crosses(pointOutLine));
            Write(line.Difference(pointInLine));
            Write(line.Difference(pointOutLine));
            Write(line.Disjoint(pointInLine));
            Write(line.Disjoint(pointOutLine));
            Write(line.Distance(pointInLine));
            Write(line.Distance(pointOutLine));
            Write(line.EqualsTopologically(line.Copy() as LineString));
            Write(line.EqualsExact(line.Copy() as LineString));
            Write(line.ConvexHull());
            Write(line.Intersection(pointInLine));
            Write(line.Intersection(pointOutLine));
            Write(line.Intersection(aLine));
            Write(line.Intersects(pointInLine));
            Write(line.Intersects(pointOutLine));
            Write(line.Intersects(aLine));
            Write(line.IsWithinDistance(pointOutLine, 2));
            Write(line.IsWithinDistance(pointOutLine, 222));
            Write(line.Overlaps(pointInLine));
            Write(line.Overlaps(pointOutLine));
            Write(line.Overlaps(aLine));
            Write(line.Overlaps(anotherLine));
            Write(line.Relate(pointInLine));
            Write(line.Relate(pointOutLine));
            Write(line.Relate(aLine));
            Write(line.Relate(anotherLine));
            Write(line.SymmetricDifference(pointInLine));
            Write(line.SymmetricDifference(pointOutLine));
            Write(line.SymmetricDifference(aLine));
            Write(line.SymmetricDifference(anotherLine));
            Write(line.ToString());
            Write(line.AsText());
            Write(line.Touches(pointInLine));
            Write(line.Touches(pointOutLine));
            Write(line.Touches(aLine));
            Write(line.Touches(anotherLine));
            Write(line.Union(pointInLine));
            Write(line.Union(pointOutLine));
            Write(line.Union(aLine));
            Write(line.Union(anotherLine));
            Write(line.Within(pointInLine));
            Write(line.Within(pointOutLine));
            Write(line.Within(aLine));
            Write(line.Within(anotherLine));

            string linestring = "LINESTRING (1.2 3.4, 5.6 7.8, 9.1 10.12)";
            string anotherlinestringg = "LINESTRING (12345 3654321, 685 7777.945677, 782 111.1)";
            var geom1 = Reader.Read(linestring);
            Write(geom1.AsText());
            var geom2 = Reader.Read(anotherlinestringg);
            Write(geom2.AsText());

            byte[] bytes = line.AsBinary();
            var test1 = new WKBReader().Read(bytes);
            Write(test1.ToString());

            var features = new Feature[]
            {
                CreateFeature(line), 
                CreateFeature(aLine),
                CreateFeature(anotherLine),
                CreateFeature((LineString)geom1),
                CreateFeature((LineString)geom2),
            };
            Shapefile.WriteAllFeatures(features, GetType().Name);
            Shapefile.ReadAllFeatures(GetType().Name);
        }

        private Feature CreateFeature(LineString lineString)
        {
            var mls = new MultiLineString(new LineString[] { lineString });
            return new Feature(mls, null);
        }

        protected void Write(object o)
        {
            Console.WriteLine(o.ToString());
        }

        protected void Write(string s)
        {
            Console.WriteLine(s);
        }
    }
}
