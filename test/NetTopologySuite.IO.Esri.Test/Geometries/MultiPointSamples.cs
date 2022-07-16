﻿using NetTopologySuite.Geometries;
using System;
using NetTopologySuite.Operation.Buffer;

namespace NetTopologySuite.IO.ShapeFile.Test.Geometries
{
    /// <summary>
    ///
    /// </summary>
    public class MultiPointSamples
    {
        private MultiPoint multiPoint = null;

        protected GeometryFactory Factory { get; private set; }

        protected WKTReader Reader { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public MultiPointSamples() : base()
        {
            this.Factory = new GeometryFactory();
            this.Reader = new WKTReader();

            var coordinates = new Coordinate[]
            {
                new Coordinate(100,100),
                new Coordinate(200,200),
                new Coordinate(300,300),
                new Coordinate(400,400),
                new Coordinate(500,500),
            };
            multiPoint = Factory.CreateMultiPointFromCoords(coordinates);
        }

        /// <summary>
        ///
        /// </summary>
        public void Start()
        {
            try
            {
                Write(multiPoint.Area);
                Write(multiPoint.Boundary);
                Write(multiPoint.BoundaryDimension);
                Write(multiPoint.Centroid);
                Write(multiPoint.Coordinate);
                Write(multiPoint.Coordinates);
                Write(multiPoint.Dimension);
                Write(multiPoint.Envelope);
                Write(multiPoint.EnvelopeInternal);
                Write(multiPoint.Geometries.Length);
                Write(multiPoint.InteriorPoint);
                Write(multiPoint.IsEmpty);
                Write(multiPoint.IsSimple);
                Write(multiPoint.IsValid);
                Write(multiPoint.Length);
                Write(multiPoint.NumGeometries);
                Write(multiPoint.NumPoints);

                Write(multiPoint.Buffer(10));
                Write(multiPoint.Buffer(10, new BufferParameters {EndCapStyle = EndCapStyle.Flat }));
                Write(multiPoint.Buffer(10, new BufferParameters { EndCapStyle = EndCapStyle.Square }));
                Write(multiPoint.Buffer(10, 20));
                Write(multiPoint.Buffer(10, new BufferParameters(20) { EndCapStyle = EndCapStyle.Flat }));
                Write(multiPoint.Buffer(10, new BufferParameters(20) { EndCapStyle = EndCapStyle.Square }));
                Write(multiPoint.ConvexHull());

                byte[] bytes = multiPoint.AsBinary();
                var test1 = new WKBReader().Read(bytes);
                Write(test1.ToString());

                bytes = new GDBWriter().Write(multiPoint);
                test1 = new GDBReader().Read(bytes);
                Write(test1.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
