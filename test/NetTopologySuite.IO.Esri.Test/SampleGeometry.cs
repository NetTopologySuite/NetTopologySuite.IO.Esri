using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace NetTopologySuite.IO.Esri.Test;

internal class SampleGeometry
{
    private static readonly GeometryFactory _geomFactory = NtsGeometryServices.Instance.CreateGeometryFactory(5432);
    private static readonly WKTReader _reader = new WKTReader(_geomFactory);

    public static Geometry EmptyPoint { get; } = _reader.Read("POINT EMPTY");
    public static Geometry EmptyMultiPoint { get; } = _reader.Read("MULTIPOINT EMPTY");
    public static Geometry EmptyLineString { get; } = _reader.Read("LINESTRING EMPTY");
    public static Geometry EmptyMultiLineString { get; } = _reader.Read("MULTILINESTRING EMPTY");
    public static Geometry EmptyPolygon { get; } = _reader.Read("POLYGON EMPTY");
    public static Geometry EmptyMultiPolygon { get; } = _reader.Read("MULTIPOLYGON EMPTY");

    // Credits to: https://en.wikipedia.org/wiki/Well-known_text_representation_of_geometry
    public static Geometry SamplePoint { get; } = _reader.Read("POINT (30 10)");
    public static Geometry SampleMultiPoint { get; } = _reader.Read("MULTIPOINT ((10 40), (40 30), (20 20),(30 10))");
    public static Geometry SampleLineString { get; } = _reader.Read("LINESTRING (30 10, 10 30, 40 40)");
    public static Geometry SampleMultiLineString { get; } = _reader.Read("MULTILINESTRING ((10 10, 20 20, 10 40),(40 40, 30 30, 40 20, 30 10))");
    public static Geometry SamplePolygon { get; } = _reader.Read("POLYGON ((30 10, 40 40, 20 40, 10 20, 30 10))");
    public static Geometry SamplePolygonWithHole { get; } = _reader.Read("POLYGON ((35 10, 45 45, 15 40, 10 20, 35 10),(20 30, 35 35, 30 20, 20 30))");
    public static Geometry SampleMultiPolygon { get; } = _reader.Read("MULTIPOLYGON (((30 20, 45 40, 10 40, 30 20)),((15 5, 40 10, 10 20, 5 10, 15 5)))");
    public static Geometry SampleMultiPolygonWithHole { get; } = _reader.Read("MULTIPOLYGON (((40 40, 20 45, 45 30, 40 40)),((20 35, 10 30, 10 10, 30 5, 45 20, 20 35),(30 20, 20 15, 20 25, 30 20)))");

}
