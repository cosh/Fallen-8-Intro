using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fallen8.API;
using Fallen8.API.Index;
using Fallen8.Model;
using System.Diagnostics;

namespace Intro
{
    public class Benchmark
    {
        public static int[] w_ids = new int[] { 
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

        private static void Query2(IVertexModel startVertex, long myEdgePropertyID)
        {            
            Object sig;
            Object freq;

            long sourceVertexID;
            long targetVertexID;

            IEdgePropertyModel edgeProperty1, edgeProperty2;
            if (startVertex.TryGetOutEdge(out edgeProperty1, myEdgePropertyID))
            {
                foreach (var aTargetVertex in edgeProperty1.Select(_ => _.TargetVertex))
                {
                    if (aTargetVertex.TryGetOutEdge(out edgeProperty2, myEdgePropertyID))
                    {
                        foreach (var aEdge in edgeProperty2)
                        {
                            sourceVertexID = aEdge.SourceEdgeProperty.SourceVertex.Id;
                            targetVertexID = aEdge.TargetVertex.Id;
                            aEdge.TryGetProperty(out sig, Config.SIG_PROPERTY_ID);
                            aEdge.TryGetProperty(out freq, Config.FREQ_PROPERTY_ID);

                            //Console.WriteLine(String.Format("[{0},{1},{2},{3}]",
                            //    sourceVertexID,
                            //    targetVertexID,
                            //    sig,
                            //    freq));
                        }
                    }
                }                
            }
        }

        public static void RunQuery2(IFallen8 myFallen8, IIndex nodeIndex)
        {
            IEnumerable<IGraphElementModel> vertices;
            IVertexModel vertex;
            Stopwatch sw = new Stopwatch();

            for (int i = 0; i < w_ids.Length; i++)
            {
                if (nodeIndex.GetValue(out vertices, w_ids[i]))
                {
                    vertex = (IVertexModel) vertices.First();

                    sw.Start();
                    Query2(vertex, Config.CO_S_EDGE_PROPERTY_ID);
                    Console.WriteLine(Math.Round(sw.Elapsed.TotalMilliseconds));
                    sw.Reset();
                }
            }
        }
    }
}
