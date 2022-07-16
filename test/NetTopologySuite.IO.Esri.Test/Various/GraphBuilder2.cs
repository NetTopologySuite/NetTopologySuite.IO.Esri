﻿using NetTopologySuite.Geometries;
using QuickGraph;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.ShortestPath;
using System;
using System.Collections.Generic;

namespace NetTopologySuite.IO.ShapeFile.Test.Various
{

    /// <summary>
    /// A class that manages shortest path computation.
    /// </summary>
    public class GraphBuilder2
    {
        #region Delegates

        /// <summary>
        /// A delegate that defines how to calculate the weight
        /// of a <see cref="LineString">line</see>.
        /// </summary>
        /// <param name="line">A <see cref="LineString">line</see>.</param>
        /// <returns>The weight of the line.</returns>
        public delegate double ComputeWeightDelegate(LineString line);

        #endregion

        private static readonly ComputeWeightDelegate DefaultComputer = line => line.Length;

        private readonly bool bidirectional;

        private readonly AdjacencyGraph<Coordinate, IEdge<Coordinate>> graph;
        private readonly IList<LineString> strings;
        private IDictionary<IEdge<Coordinate>, double> consts;
        private GeometryFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphBuilder2"/> class.
        /// </summary>
        /// <param name="bidirectional">
        /// Specify if the graph must be build using both edges directions.
        /// </param>
        public GraphBuilder2(bool bidirectional)
        {
            this.bidirectional = bidirectional;

            factory = null;
            strings = new List<LineString>();
            graph = new AdjacencyGraph<Coordinate, IEdge<Coordinate>>(true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphBuilder2"/> class,
        /// using a directed graph.
        /// </summary>
        public GraphBuilder2() : this(false)
        {
        } // TODO: maybe the default value must be true...

        /// <summary>
        /// Adds each line to the graph structure.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>
        /// <c>true</c> if all <paramref name="lines">lines</paramref>
        /// are added, <c>false</c> otherwise.
        /// </returns>
        /// <exception cref="TopologyException">
        /// If geometries don't have the same <see cref="GeometryFactory">factory</see>.
        /// </exception>
        public bool Add(params LineString[] lines)
        {
            bool result = true;
            foreach (var line in lines)
            {
                var newfactory = line.Factory;
                if (factory == null)
                    factory = newfactory;
                else if (!newfactory.PrecisionModel.Equals(factory.PrecisionModel))
                    throw new TopologyException("all geometries must have the same precision model");

                bool lineFound = strings.Contains(line);
                result &= !lineFound;
                if (!lineFound)
                    strings.Add(line);
                else continue; // Skip vertex check because line is already present

                foreach (var coord in line.Coordinates)
                {
                    if (!graph.ContainsVertex(coord))
                        graph.AddVertex(coord);
                }
            }
            return result;
        }

        /// <summary>
        /// Initialize the algorithm using the default
        /// <see cref="ComputeWeightDelegate">weight computer</see>,
        /// that uses <see cref="Geometry.Length">string length</see>
        /// as weight value.
        /// </summary>
        /// <exception cref="TopologyException">
        /// If you've don't added two or more geometries to the builder.
        /// </exception>
        /// <exception cref="ApplicationException">
        /// If builder is already initialized.
        /// </exception>
        public void Initialize()
        {
            BuildEdges(DefaultComputer);
        }

        /// <summary>
        /// Initialize the algorithm using the specified
        /// <paramref name="computer">weight computer</paramref>
        /// </summary>
        /// <param name="computer">
        /// A function that computes the weight
        /// of any <see cref="LineString">edge</see> of the graph.
        /// </param>
        /// <exception cref="TopologyException">
        /// If you've don't added two or more geometries to the builder.
        /// </exception>
        /// <exception cref="ApplicationException">
        /// If builder is already initialized.
        /// </exception>
        public void Initialize(ComputeWeightDelegate computer)
        {
            BuildEdges(computer);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="computer"></param>
        private void BuildEdges(ComputeWeightDelegate computer)
        {
            if (strings.Count < 2)
                throw new TopologyException("you must specify two or more geometries to build a graph");

            // Counts the number of edges in the set we pass to this method.
            int numberOfEdgesInLines = 0;
            foreach (var str in strings)
            {
                int edges = str.Coordinates.GetUpperBound(0);
                numberOfEdgesInLines += edges;
            }

            // Double values because we use also reversed edges...
            if (bidirectional)
                numberOfEdgesInLines *= 2;

            consts = new Dictionary<IEdge<Coordinate>, double>(numberOfEdgesInLines);

            foreach (var line in strings)
            {
                // A line has to have at least two dimensions
                int bound = line.Coordinates.GetUpperBound(0);
                if (bound > 0)
                {
                    for (int counter = 0; counter < bound; counter++)
                    {
                        // Prepare a segment
                        var src = line.Coordinates[counter];
                        var dst = line.Coordinates[counter + 1];

                        // Here we calculate the weight of the edge
                        var lineString = factory.CreateLineString(
                            new[] { src, dst, });
                        double weight = computer(lineString);

                        // Add the edge
                        IEdge<Coordinate> localEdge = new Edge<Coordinate>(src, dst);
                        graph.AddEdge(localEdge);
                        consts.Add(localEdge, weight);

                        if (!bidirectional)
                            continue;

                        // Add the reversed edge
                        IEdge<Coordinate> localEdgeRev = new Edge<Coordinate>(dst, src);
                        graph.AddEdge(localEdgeRev);
                        consts.Add(localEdgeRev, weight);
                    }
                }
            }
        }

        /// <summary>
        /// Carries out the shortest path anlayis between the two
        /// <see cref="Geometry.Coordinate">nodes</see>
        /// passed as variables and returns an <see cref="LineString" />
        /// giveing the shortest path.
        /// </summary>
        /// <param name="source">The source geom</param>
        /// <param name="destination">The destination geom</param>
        /// <returns>A line string geometric shape of the path</returns>
        public LineString Perform(Geometry source, Geometry destination)
        {
            return Perform(source.Coordinate, destination.Coordinate);
        }

        /// <summary>
        /// Carries out the shortest path between the two nodes
        /// ids passed as variables and returns an <see cref="LineString" />
        /// giveing the shortest path.
        /// </summary>
        /// <param name="source">The source node</param>
        /// <param name="destination">The destination node</param>
        /// <returns>A line string geometric shape of the path</returns>
        public LineString Perform(Coordinate source, Coordinate destination)
        {
            if (!graph.ContainsVertex(source))
                throw new ArgumentException("key not found in the graph", "source");
            if (!graph.ContainsVertex(destination))
                throw new ArgumentException("key not found in the graph", "destination");

            // Build algorithm
            var dijkstra =
                new DijkstraShortestPathAlgorithm<Coordinate, IEdge<Coordinate>>(graph, edge => consts[edge]);

            // Attach a Distance observer to give us the distances between edges
            var distanceObserver =
                new VertexDistanceRecorderObserver<Coordinate, IEdge<Coordinate>>(edge => consts[edge]);
            distanceObserver.Attach(dijkstra);

            // Attach a Vertex Predecessor Recorder Observer to give us the paths
            var predecessorObserver =
                new VertexPredecessorRecorderObserver<Coordinate, IEdge<Coordinate>>();
            predecessorObserver.Attach(dijkstra);

            // Run the algorithm with A set to be the source
            dijkstra.Compute(source);

            // Get the path computed to the destination.
            IEnumerable<IEdge<Coordinate>> path;
            bool result = predecessorObserver.TryGetPath(destination, out path);

            // Then we need to turn that into a geomery.
            return result ? BuildString(new List<IEdge<Coordinate>>(path)) : null;

            // if the count is greater than one then a
            // path could not be found, so we return null
        }

        /// <summary>
        /// Takes the path returned from QuickGraph library and uses the
        /// list of coordinates to reconstruct the path into a geometric
        /// "shape"
        /// </summary>
        /// <param name="path">Shortest path from the QucikGraph Library</param>
        /// <returns></returns>
        private LineString BuildString(IList<IEdge<Coordinate>> path)
        {
            // if the path has no links then return a null reference
            if (path.Count < 1)
                return null;

            // if we get here then we now that there is at least one
            // edge in the path.
            var links = new Coordinate[path.Count + 1];

            // Add each node to the list of coordinates in to the array.
            int i;
            for (i = 0; i < path.Count; i++)
                links[i] = path[i].Source;

            // Add the target node to the last loction in the list
            links[i] = path[i - 1].Target;

            // Turn the list of coordinates into a geometry.
            var thePath = factory.CreateLineString(links);
            return thePath;
        }

        /// <summary>
        /// Outputs the graph as a DIMACS Graph
        /// </summary>
        /// <param name="fileName">The name of the output graph</param>
        /// <returns>Indicates if the method was worked.</returns>
        public bool WriteAsDIMACS(string fileName)
        {
            // The DIMACS format is a reasonabley standard method
            // of preparing graphs for analys in SP algortihm.
            // This method *could* be used to prepare the graph beforehand
            // so the turning from a GIS layer into a graph is not so
            // intensive.
            //
            // NOTE: Follows the 9th DIMACS format: http://www.dis.uniroma1.it/~challenge9/format.shtml
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool ReadFromDIMACS(string fileName)
        {
            return false;
        }
    }
}
