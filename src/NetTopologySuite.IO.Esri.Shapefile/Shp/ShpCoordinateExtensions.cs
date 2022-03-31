namespace NetTopologySuite.IO.Esri.Shp
{

    /// <summary>
    /// Helper methods for ordinate converting as specified in the ESRI Shapefile Technical Description.
    /// </summary>
    internal static class ShpCoordinateExtensions
    {

        /// <summary>
        /// Converts Z ordinate to value as specified in the ESRI Shapefile Technical Description.
        /// </summary>
        /// <param name="coordinate">Z value.</param>
        /// <returns>
        /// • 0.0 if  specified <paramref name="coordinate"/> value is <see cref="double.NaN"/> <br />
        /// • <see cref="double.MaxValue"/> if specified <paramref name="coordinate"/> value is <see cref="double.PositiveInfinity"/> <br />
        /// • <see cref="double.MinValue"/> if specified <paramref name="coordinate"/> value is <see cref="double.NegativeInfinity"/> <br />
        /// otherwise it returns specified <paramref name="coordinate"/> value.
        /// </returns>
        /// <remarks>
        /// SHP Spec: Floating point numbers must be numeric values. Positive infinity, negative infinity,
        /// and Not-a-Number(NaN) values are not allowed in shapefiles. 
        /// </remarks>
        public static double ToValidShpCoordinate(this double coordinate)
        {
            if (double.IsNaN(coordinate) || coordinate == double.MinValue) // ArcMap 10.6 saves empty point as doubule.MinValue coordinates
                return 0.0;

            if (double.IsPositiveInfinity(coordinate))
                return double.MaxValue;

            if (double.IsNegativeInfinity(coordinate))
                return double.MinValue + double.Epsilon; // ArcMap 10.6 saves empty point as doubule.MinValue coordinates -> add Epsilon.

            return coordinate;
        }


        /// <summary>
        /// Converts Measure to value as specified in the ESRI Shapefile Technical Description.
        /// </summary>
        /// <param name="m">Measure value.</param>
        /// <returns>
        /// • value less than –10E38 if  specified <paramref name="m"/> value is <see cref="double.NaN"/> <br />
        /// • <see cref="double.MaxValue"/> if specified <paramref name="m"/> value is <see cref="double.PositiveInfinity"/> <br />
        /// • -10E38 if specified <paramref name="m"/> value is <see cref="double.NegativeInfinity"/> <br />
        /// otherwise it returns specified <paramref name="m"/> value.
        /// </returns>
        /// <remarks>
        /// SHP Spec: Floating point numbers must be numeric values. Positive infinity, negative infinity,
        /// and Not-a-Number(NaN) values are not allowed in shapefiles. Nevertheless, shapefiles
        /// support the concept of "no data" values, but they are currently used only for measures.
        /// Any floating point number smaller than –10E38 is considered by a shapefile reader to represent a "no data" value.
        /// </remarks>
        public static double ToValidShpMeasure(this double m)
        {
            if (double.IsNaN(m))
                return double.MinValue; //  Shapefile.MeasureMinValue - 1;  (Esri uses double.MinValue)

            if (double.IsPositiveInfinity(m))
                return double.MaxValue;

            if (double.IsNegativeInfinity(m))
                return Shapefile.MeasureMinValue;

            return m;
        }
    }





}
