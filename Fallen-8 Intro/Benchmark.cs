using System;
using System.Collections.Generic;
using System.Linq;
using Fallen8.API.Index;
using System.Diagnostics;
using Fallen8.API.Model;

namespace Intro
{
    public class Benchmark
    {
        public static int[] w_ids = new[] { 
            12340,  5729,   9763,   5806,   3398,   6358,   22037,  18391,  11439,  5622,
            18413,  3969,   2571,   1524,   9746,   6696,   15562,  16258,  9809,   11561,
            8621,   3797,   17290,  14283,  22046,  13457,  15290,  4735,   7722,   2670,
            8529,   12336,  5421,   5452,   9167,   7857,   2546,   17361,  18124,  306, 
            4932,   12769,  26917,  28094,  7215,   25316,  10751,  16089,  52248,  17520,
            31030,  1008,   14402,  10427,  1455,   15617,  26999,  28070,  4596,   17624,
            23537,  11311,  18036,  1962,   2963,   14091,  8032,   3431,   2955,   19083,
            22995,  7046,   1919,   6108,   12489,  3528,   4578,   11637,  3159,   198898,
            128888, 8957,   3537,   3671,   73988,  8460,   3783,   19635,  8037,   63119, 
            10738,  11269,  16267,  20918,  3788,   20138,  16991,  10438,  15575,  13652
        };

        /// <summary>
        /// Finds all edges with distance 2 from start vertex
        /// </summary>
        /// <param name="startVertex"></param>
        /// <param name="myEdgePropertyID"></param>
        private static void Query2(VertexModel startVertex, Int32 myEdgePropertyID)
        {            
            Object sig;
            Object freq;

            EdgePropertyModel edgeProperty1, edgeProperty2;
            if (startVertex.TryGetOutEdge(out edgeProperty1, myEdgePropertyID))
            {
                foreach (var aTargetVertex in edgeProperty1.Select(_ => _.TargetVertex))
                {
                    if (aTargetVertex.TryGetOutEdge(out edgeProperty2, myEdgePropertyID))
                    {
                        foreach (var aEdge in edgeProperty2)
                        {
                            aEdge.TryGetProperty(out sig, Config.SIG_PROPERTY_ID);
                            aEdge.TryGetProperty(out freq, Config.FREQ_PROPERTY_ID);
                        }
                    }
                }                
            }
        }

        /// <summary>
        /// Finds all edges between the neighbours of the given start vertex.
        /// </summary>
        /// <param name="startVertex"></param>
        /// <param name="myEdgePropertyID"></param>
        private static void Query3(VertexModel startVertex, Int32 myEdgePropertyID)
        {
            Object sig;
            Object freq;

            EdgePropertyModel edgeProperty1, edgeProperty2;

            if (startVertex.TryGetOutEdge(out edgeProperty1, myEdgePropertyID))
            {
                foreach (var aOutEdge1 in edgeProperty1)
                {
                    foreach (var aOutEdge2 in edgeProperty1)
                    {
                        if (ReferenceEquals(aOutEdge1.TargetVertex, aOutEdge2.TargetVertex))
                        {
                        }
                        else
                        {
                            if (aOutEdge1.TargetVertex.TryGetOutEdge(out edgeProperty2, myEdgePropertyID))
                            {
                                foreach (var aEdge in edgeProperty2)
                                {
                                    if (ReferenceEquals(aEdge.TargetVertex, aOutEdge2.TargetVertex))
                                    {
                                        var sourceVertexID = aEdge.SourceEdgeProperty.SourceVertex.Id;
                                        var targetVertexID = aEdge.TargetVertex.Id;
                                        aEdge.TryGetProperty(out sig, Config.SIG_PROPERTY_ID);
                                        aEdge.TryGetProperty(out freq, Config.FREQ_PROPERTY_ID);
                                    }
                                }
                            }

                        }
                    }
                }
            }

        }

        public static List<double> RunQuery2(Fallen8.API.Fallen8 myFallen8, String wordIndexName)
        {
            var nodeIndex = (SingleValueIndex)myFallen8.IndexFactory.Indices[wordIndexName];
            var totalMilliseconds = new List<double>();

            IEnumerable<AGraphElement> vertices;
            VertexModel vertex;
            var sw = new Stopwatch();

            foreach (var t in w_ids)
            {
                if (nodeIndex.TryGetValue(out vertices, t))
                {
                    vertex = (VertexModel) vertices.First();

                    sw.Start();
                    Query2(vertex, Config.CO_S_EDGE_PROPERTY_ID);
                    totalMilliseconds.Add(Math.Round(sw.Elapsed.TotalMilliseconds));
                    sw.Reset();
                }
            }

            return totalMilliseconds;
        }

        public static List<double> RunQuery3(Fallen8.API.Fallen8 myFallen8, String nodeIndexName)
        {
            var nodeIndex = (SingleValueIndex)myFallen8.IndexFactory.Indices[nodeIndexName];
            var totalMilliseconds = new List<double>();

            AGraphElement graphElement;
            VertexModel vertex;
            var sw = new Stopwatch();

            foreach (var t in w_ids)
            {
                if (nodeIndex.TryGetValue(out graphElement, t))
                {
                    vertex = (VertexModel)graphElement;

                    sw.Start();
                    Query3(vertex, Config.CO_S_EDGE_PROPERTY_ID);
                    totalMilliseconds.Add(Math.Round(sw.Elapsed.TotalMilliseconds));
                    sw.Reset();
                }
            }

            return totalMilliseconds;
        }
    }
}
