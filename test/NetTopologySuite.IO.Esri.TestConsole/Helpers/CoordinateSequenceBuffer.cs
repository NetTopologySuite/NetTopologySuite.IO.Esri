using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetTopologySuite.IO.Esri.TestConsole
{
    public class CoordinateSequenceBuffer : CoordinateSequence
    {
        // List will keep te internal array capacity even after List.Clear() call.
        private List<(double, double, double)> Xyz = new List<(double, double, double)>();

        private CoordinateSequenceBuffer(List<(double, double, double)> buffer) : base(buffer.Count, 3, 0)
        {
            Xyz = buffer;
        }

        /// Count property cannot be overriden. Use Build() method to create Buffer with proper Count property
        public CoordinateSequenceBuffer() : this(new List<(double, double, double)>())
        {
        }

        public override CoordinateSequence Copy()
        {
            var copy = new CoordinateSequenceBuffer(Xyz);
            copy.Xyz = Xyz.ToList(); // Make a copy
            return copy;
        }

        public override double GetOrdinate(int index, int ordinateIndex)
        {
            if (index >= Xyz.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Internal buffer changed.");
            }
            if (ordinateIndex == 0)
                return Xyz[index].Item1;

            if (ordinateIndex == 1)
                return Xyz[index].Item2;

            if (ordinateIndex == 2)
                return Xyz[index].Item3;

            return double.NaN;
        }

        public override void SetOrdinate(int index, int ordinateIndex, double value)
        {
            if (index >= Xyz.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Internal buffer changed.");
            }
            if (ordinateIndex == 0)
                Xyz[index] = (value, Xyz[index].Item2, Xyz[index].Item3);

            if (ordinateIndex == 1)
                Xyz[index] = (Xyz[index].Item1, value, Xyz[index].Item3);

            if (ordinateIndex == 2)
                Xyz[index] = (Xyz[index].Item1, Xyz[index].Item2, value);
        }

        public void Clear()
        {
            Xyz.Clear();
        }

        public void AddXyz(double x, double y, double z)
        {
            Xyz.Add((x, y, z));
        }

        /// Count property cannot be overriden so this workaround is needed.
        public CoordinateSequenceBuffer Build()
        {
            return new CoordinateSequenceBuffer(Xyz);
        }
    }
}
