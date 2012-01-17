using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Intro
{
    class Program
    {
        static void Main(string[] args)
        {
            var fallen8 = new Fallen8.API.Fallen8();
            var introProvicer = new IntroProvider(fallen8);
            int nodeCount = 100000;
            int edgeCount = 10;

            Stopwatch sw = Stopwatch.StartNew();

            introProvicer.CreateScaleFreeNetwork(nodeCount, edgeCount);
            sw.Stop();
            Console.WriteLine(String.Format("It took {0}ms to create a Fallen-8 graph with {1} nodes and {2} edges per node.", sw.Elapsed.TotalMilliseconds, nodeCount, edgeCount));

            introProvicer.TraverseABit();

            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
