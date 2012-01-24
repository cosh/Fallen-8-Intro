using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Fallen8.API.Index;

namespace Intro
{
    class Program
    {
        static void Main(string[] args)
        {
            var fallen8 = new Fallen8.API.Fallen8();

            //IIndex index;
            //if (fallen8.IndexFactory.TryCreateIndex(out index, "word_idx", "SingleValueIndex"))
            //{
            //    Console.WriteLine("created index word_idx");
            //}

            //Import.ImportFromMySql(fallen8, index);
            //Console.WriteLine("done");

            //Console.ReadLine();         
            ////Benchmark.RunQuery2(fallen8, index);
            //Benchmark.RunQuery3(fallen8, (SingleValueIndex)index);

            var introProvicer = new IntroProvider(fallen8);
            int nodeCount = 100000;
            int edgeCount = 60;

            Stopwatch sw = Stopwatch.StartNew();

            introProvicer.CreateScaleFreeNetwork(nodeCount, edgeCount);
            sw.Stop();
            Console.WriteLine(String.Format("It took {0}ms to create a Fallen-8 graph with {1} nodes and {2} edges per node.", sw.Elapsed.TotalMilliseconds, nodeCount, edgeCount));

            GC.Collect();
            GC.Collect();
            GC.WaitForFullGCApproach();

            introProvicer.Bench();

            Console.WriteLine("done");

            Console.ReadLine();         
        }
    }
}
