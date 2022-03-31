using NetTopologySuite.Geometries;
using System.IO;

namespace NetTopologySuite.IO.Esri.Shp.Writers
{

    /// <summary>
    /// MultiLine SHP file writer. 
    /// </summary>
    public class ShpPolyLineWriter : ShpWriter<MultiLineString>
    {
        /// <inheritdoc/>
        public ShpPolyLineWriter(Stream shpStream, Stream shxStream, ShapeType type) : base(shpStream, shxStream, type)
        {
            if (!ShapeType.IsPolyLine())
                ThrowUnsupportedShapeTypeException();
        }

        internal override void WriteGeometry(MultiLineString multiLineString, Stream stream)
        {
            // SHP Docs: A part is a connected sequence of two or more points. (page 7)
            var partsBuilder = new ShpMultiPartBuilder(multiLineString.Count, 2);
            for (int i = 0; i < multiLineString.Count; i++)
            {
                var ln = (LineString)multiLineString[i];
                if (ln.NumPoints >= 2)
                {
                    partsBuilder.AddPart(ln.CoordinateSequence);
                }
            }

            partsBuilder.WriteParts(stream, HasZ, HasM);
            partsBuilder.UpdateExtent(Extent);
        }
    }


}
