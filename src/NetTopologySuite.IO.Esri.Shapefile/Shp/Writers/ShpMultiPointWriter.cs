using NetTopologySuite.Geometries;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Writers
{

    /// <summary>
    /// MultiPoint SHP file writer. 
    /// </summary>
    public class ShpMultiPointWriter : ShpWriter<MultiPoint>
    {
        /// <inheritdoc/>
        public ShpMultiPointWriter(Stream shpStream, Stream shxStream, ShapeType type) : base(shpStream, shxStream, type)
        {
            if (!ShapeType.IsMultiPoint())
                ThrowUnsupportedShapeTypeException();
        }

        internal override void WriteGeometry(MultiPoint geometry, Stream shapeBinary)
        {
            var coordinateSequence = geometry.Factory.CoordinateSequenceFactory.Create(geometry.Coordinates);
            var geometryExtent = new ShpExtent();
            geometryExtent.Expand(coordinateSequence);

            shapeBinary.WriteXYBoundingBox(geometryExtent);
            shapeBinary.WritePointCount(coordinateSequence.Count);

            shapeBinary.WriteXYCoordinates(coordinateSequence);

            if (HasZ)
            {
                shapeBinary.WriteZRange(geometryExtent.Z.Min, geometryExtent.Z.Max);
                shapeBinary.WriteZCoordinates(coordinateSequence);
            }

            if (HasM)
            {
                shapeBinary.WriteMRange(geometryExtent.M.Min, geometryExtent.M.Max);
                shapeBinary.WriteMValues(coordinateSequence);
            }

            Extent.Expand(geometryExtent);
        }

    }


}
