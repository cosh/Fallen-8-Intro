using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Fallen8.API;
using System.Threading.Tasks;
using Fallen8.API.Helper;
using Fallen8.API.Model;
using System.Threading;

namespace Intro
{
    class IntroProvider
    {
        private Fallen8.API.Fallen8 _fallen8;
        private Int32 _edgePropertyId;

        public IntroProvider(Fallen8.API.Fallen8 fallen8)
        {
            _fallen8 = fallen8;
            _edgePropertyId = 0;
        }

        internal void CreateScaleFreeNetwork(int nodeCound, int edgeCount)
        {
            DateTime creationDate = DateTime.Now;
            List<Int32> vertexIDs = new List<Int32>();
            Random prng = new Random();

            for (int i = 0; i < nodeCound; i++)
            {
                vertexIDs.Add(
                    _fallen8.CreateVertex(creationDate, new Dictionary<int, object> {{23, 4344}, {33, "asdasd"}}).Id);
                        
            }

            foreach (var aVertexId in vertexIDs)
            {
                HashSet<Int32> targetVertices = new HashSet<Int32>();

                do
                {
                    targetVertices.Add(vertexIDs[prng.Next(0, vertexIDs.Count)]);
                } while (targetVertices.Count < edgeCount);

                foreach (var aTargetVertex in targetVertices)
                {
                    _fallen8.CreateEdge(aVertexId, _edgePropertyId, new EdgeModelDefinition(aTargetVertex, creationDate, null));
                }
            }
        }

        internal void TraverseABit()
        {
            var vertex = _fallen8.GetVertices().First();


            #region out edges

            EdgePropertyModel edgeProperty;
            if (vertex.TryGetOutEdge(out edgeProperty, _edgePropertyId))
            {
                foreach (var aTargetVertex in edgeProperty.Select(_ => _.TargetVertex))
                {
                    // target vertices
                }

                foreach (var aEdge in edgeProperty)
                {
                    // edge
                }
            } 
            
            #endregion

            #region inc edges

            IEnumerable<EdgeModel> incomingEdges;
            if (vertex.TryGetInEdges(out incomingEdges, _edgePropertyId))
            {
                foreach (var aIncomingVertex in incomingEdges.Select(_ => _.SourceEdgeProperty.SourceVertex))
                {
                    // incoming vertices
                }

                foreach (var aIncomingVertex in incomingEdges.Select(_ => _.SourceEdgeProperty).SelectMany(__ => __))
                {
                    // outgoing vertices of the incoming vertices
                }
            }
            
            #endregion
        }

        internal void Bench(int myIterations = 1000)
        {
            var vertices = _fallen8.GetVertices();
            var tps = new List<double>();
            long edgeCount = 0;

            for (int i = 0; i < myIterations; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();

                edgeCount = CountAllEdgesParallelPartitioner(vertices);

                sw.Stop();

                tps.Add(edgeCount / sw.Elapsed.TotalSeconds);
            }

            Console.WriteLine(String.Format("Traversed {0} edges.", edgeCount));

            Console.WriteLine(String.Format("Traversed {0} edges. Average: {1}TPS Median: {2}TPS StandardDeviation {3}TPS ", edgeCount, Statistics.Average(tps), Statistics.Median(tps), Statistics.StandardDeviation(tps)));
        }

        private long CountAllEdgesParallelPartitioner(System.Collections.ObjectModel.ReadOnlyCollection<VertexModel> vertices)
        {
            object lockObject = new object();
            Int64 edgeCount = 0L;
            var rangePartitioner = Partitioner.Create(0, vertices.Count);

            Parallel.ForEach(
                rangePartitioner,
                () => 0L,
                (range, loopstate, initialValue) =>
                    {
                        Int64 localCount = initialValue;

                        for (int i = range.Item1; i < range.Item2; i++)
                        {
                            EdgePropertyModel epm;

                            if (vertices[i].TryGetOutEdge(out epm, _edgePropertyId))
                            {
                                foreach (var aOutGoingEdge in epm)
                                {
                                    VertexModel vertex = aOutGoingEdge.TargetVertex;
                                    localCount++;

                                }
                            }


                        }

                        return localCount;

                    },
                (localSum) =>
                    {
                        lock (lockObject)
                        {
                            edgeCount += localSum;
                        }
                    });

            return edgeCount;
        }
    }
}
