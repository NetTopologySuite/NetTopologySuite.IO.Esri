using System;
using NetTopologySuite.Geometries;
using Assert = NUnit.Framework.Assert;

namespace NetTopologySuite.IO.Esri.Test.Deprecated.ShapeFile.Extended
{
    public static class HelperMethods
    {
        private static readonly double REQUIRED_PRECISION = Math.Pow(10, -9);

        public static void AssertEnvelopesEqual(Envelope env1, Envelope env2)
        {
            AssertEnvelopesEqual(env1, env2, REQUIRED_PRECISION);
        }

        public static void AssertEnvelopesEqual(Envelope env1, Envelope env2, double requiredPrecision, string errorMessage = "")
        {
            AssertDoubleValuesEqual(env1.MaxX, env2.MaxX, requiredPrecision, errorMessage);
            AssertDoubleValuesEqual(env1.MaxY, env2.MaxY, requiredPrecision, errorMessage);
            AssertDoubleValuesEqual(env1.MinX, env2.MinX, requiredPrecision, errorMessage);
            AssertDoubleValuesEqual(env1.MinY, env2.MinY, requiredPrecision, errorMessage);
        }

        public static void AssertPolygonsEqual(Polygon poly1, Polygon poly2)
        {
            Assert.IsNotNull(poly1);
            Assert.IsNotNull(poly2);

            LineString line1 = poly1.Shell;
            LineString line2 = poly2.Shell;

            Assert.AreEqual(line1.Coordinates.Length, line2.Coordinates.Length, "Number of coordinates between polygons doesn't match");

            for (int i = 0; i < line2.Coordinates.Length; i++)
            {
                AssertCoordinatesEqual(line2.Coordinates[i], line1.Coordinates[i]);
            }
        }

        public static void AssertPolygonsEqual(MultiPolygon multiPoly1, Polygon poly2)
        {
            if (multiPoly1.IsEmpty && poly2.IsEmpty)
            {
                return;
            }

            Assert.AreEqual(multiPoly1.NumGeometries, 1);
            var poly1 = multiPoly1.GetGeometryN(0) as Polygon;
            Assert.IsNotNull(poly1);
            AssertPolygonsEqual(poly1, poly2);
        }

        public static void AssertCoordinatesEqual(Coordinate coord1, Coordinate coord2)
        {
            AssertDoubleValuesEqual(coord1.X, coord2.X);
            AssertDoubleValuesEqual(coord1.Y, coord2.Y);
        }

        public static void AssertDoubleValuesEqual(double num1, double num2)
        {
            AssertDoubleValuesEqual(num1, num2, REQUIRED_PRECISION);
        }

        public static void AssertDoubleValuesEqual(double num1, double num2, double requiredPrecision, string errorMessage = "")
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                Assert.AreEqual(num1, num2, requiredPrecision);
            }
            else
            {
                Assert.AreEqual(num1, num2, requiredPrecision, errorMessage);
            }
        }
    }
}
