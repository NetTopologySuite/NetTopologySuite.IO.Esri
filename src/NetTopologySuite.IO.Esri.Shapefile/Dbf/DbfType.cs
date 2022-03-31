using System;

namespace NetTopologySuite.IO.Esri.Dbf
{
    /// <summary>
    /// dBASE field type
    /// </summary>
    public enum DbfType : byte
    {
        /// <summary>
        /// Character
        /// </summary>
        Character = (byte)'C',

        /// <summary>
        /// Date - 8 bytes (stored as a string in the format YYYYMMDD).
        /// </summary>
        Date = (byte)'D',

        /// <summary>
        /// Numeric - positive or negative numbers. Number stored as a string. Can store integer and decimal numbers but Esri software uses it only for integer values. 
        /// </summary>
        Numeric = (byte)'N',

        /// <summary>
        /// Positive or negative numbers. Identical to Numeric; maintained for compatibility. Used by Esri software.
        /// </summary>
        Float = (byte)'F',

        /// <summary>
        /// Logical - one character ('T','F')
        /// </summary>
        Logical = (byte)'L',

        // <summary>
        // Binary - not dBASE III. This will hold the WKB for a geometry object.
        // </summary>
        // Binary = (byte)'B',


        /// <summary>
        /// Shapefile's shape stored in SHP file (not dBASE standard).
        /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
        [Obsolete(nameof(Shape) + " type is not supported by dBASE standard. It is designed for internal use by NetTopologySuite.")]
#pragma warning restore CS0618 // Type or member is obsolete
        Shape = (byte)'S',

    }

}
