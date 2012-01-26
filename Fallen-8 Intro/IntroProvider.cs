using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Fallen8.API.Helper;
using Fallen8.API.Model;
using System.Collections.ObjectModel;

namespace Intro
{
    public static class IntroProvider
    {
        public static void CreateScaleFreeNetwork(int nodeCound, int edgeCount, Fallen8.API.Fallen8 fallen8)
        {
            var creationDate = DateTime.Now;
            var vertexIDs = new List<Int32>();
            var prng = new Random();

            for (var i = 0; i < nodeCound; i++)
            {
                vertexIDs.Add(
                    fallen8.CreateVertex(creationDate, new Dictionary<int, object> { { 23, 4344 } }).Id);
                        
            }

            foreach (var aVertexId in vertexIDs)
            {
                var targetVertices = new HashSet<Int32>();

                do
                {
                    targetVertices.Add(vertexIDs[prng.Next(0, vertexIDs.Count)]);
                } while (targetVertices.Count < edgeCount);

                foreach (var aTargetVertex in targetVertices)
                {
                    fallen8.CreateEdge(aVertexId, 0, new EdgeModelDefinition(aTargetVertex, creationDate));
                }
            }
        }

        public static String Bench(Fallen8.API.Fallen8 fallen8, int myIterations = 1000)
        {
            var vertices = fallen8.GetVertices();
            var tps = new List<double>();
            long edgeCount = 0;
            var sb = new StringBuilder();

            for (var i = 0; i < myIterations; i++)
            {
                var sw = Stopwatch.StartNew();

                edgeCount = CountAllEdgesParallelPartitioner(vertices);

                sw.Stop();

                tps.Add(edgeCount / sw.Elapsed.TotalSeconds);
            }

            sb.AppendLine(String.Format("Traversed {0} edges. Average: {1}TPS Median: {2}TPS StandardDeviation {3}TPS ", edgeCount, Statistics.Average(tps), Statistics.Median(tps), Statistics.StandardDeviation(tps)));

            return sb.ToString();
        }

        private static long CountAllEdgesParallelPartitioner(ReadOnlyCollection<VertexModel> vertices)
        {
            var lockObject = new object();
            var edgeCount = 0L;
            var rangePartitioner = Partitioner.Create(0, vertices.Count);

            Parallel.ForEach(
                rangePartitioner,
                () => 0L,
                (range, loopstate, initialValue) =>
                    {
                        var localCount = initialValue;

                        for (var i = range.Item1; i < range.Item2; i++)
                        {
                            EdgePropertyModel epm;

                            if (vertices[i].TryGetOutEdge(out epm, 0))
                            {
                                foreach (var aOutGoingEdge in epm)
                                {
                                    var vertex = aOutGoingEdge.TargetVertex;
                                    localCount++;

                                }
                            }


                        }

                        return localCount;

                    },
                delegate(long localSum)
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
