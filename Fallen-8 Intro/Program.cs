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
            
            IIndex index;
            if (fallen8.IndexProvider.TryCreateIndex(out index, "word_idx"))
            {
                Console.WriteLine("created index word_idx");
            }

            Import.ImportFromMySql(fallen8, index);

            Benchmark.RunQuery2(fallen8, index);

            //var introProvicer = new IntroProvider(fallen8);
            //int nodeCount = 100000;
            //int edgeCount = 10;

            //Stopwatch sw = Stopwatch.StartNew();

            //introProvicer.CreateScaleFreeNetwork(nodeCount, edgeCount);
            //sw.Stop();
            //Console.WriteLine(String.Format("It took {0}ms to create a Fallen-8 graph with {1} nodes and {2} edges per node.", sw.Elapsed.TotalMilliseconds, nodeCount, edgeCount));

            //introProvicer.TraverseABit();

            //Console.WriteLine("done");

            Console.ReadLine();         
        }
    }
}
