using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Fallen8.API.Index;

namespace Intro.Service
{
    public sealed class IntroService : IIntroService, IDisposable
    {
        #region Data

        /// <summary>
        ///   The internal Fallen-8 instance
        /// </summary>
        private readonly Fallen8.API.Fallen8 _fallen8;

        /// <summary>
        /// The indexName
        /// </summary>
        private const String _wordIndexName = "word_idx";

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new import service
        /// </summary>
        /// <param name="fallen8"></param>
        public IntroService(Fallen8.API.Fallen8 fallen8)
        {
            _fallen8 = fallen8;
        }

        #endregion

        #region IIntroService Members

        public void Clear()
        {
            _fallen8.TabulaRasa();
        }

        public String Status()
        {
            var sb = new StringBuilder();

            _fallen8.Trim();

            var currentProcess = Process.GetCurrentProcess();
            var totalBytesOfMemoryUsed = currentProcess.WorkingSet64 / 1024 / 1024;

            sb.AppendLine(String.Format("Memory consumption: {0} MB", totalBytesOfMemoryUsed));
            sb.AppendLine(String.Format("Graph: |V| = {0} |E| = {1}", _fallen8.GetVertices().Count(), _fallen8.GetEdges().Count()));

            return sb.ToString();
        }

        #endregion

        #region public methods

        /// <summary>
        ///   Shutdown for this service
        /// </summary>
        public void Shutdown()
        {
            //nothing to do atm
        }

        #endregion

        #region IWortschatz Members

        public string ImportWortschatz()
        {
            IIndex index;
            _fallen8.IndexFactory.TryCreateIndex(out index, _wordIndexName, "SingleValueIndex");

            return Import.ImportFromMySql(_fallen8, (SingleValueIndex)index);
        }

        public List<double> ExecuteQuery(string queryIdentifier)
        {
            switch (queryIdentifier)
            {
                case "Q2":
                    return Benchmark.RunQuery2(_fallen8, _wordIndexName);

                case "Q3":
                    return Benchmark.RunQuery3(_fallen8, _wordIndexName);

                default:
                    return null;
            }
        }

        public List<string> GetAvailableQueries()
        {
            return new List<string> {"Q2", "Q3"};
        }

        #endregion

        #region IEquallyDistributed Members

        public string CreateGraph(string nodeCount, string edgeCount)
        {
            var sw = Stopwatch.StartNew();


            IntroProvider.CreateScaleFreeNetwork(Convert.ToInt32(nodeCount), Convert.ToInt32(edgeCount), _fallen8);

            sw.Stop();

            #if __MonoCS__
 			//mono specific code
			#else 
 			GC.Collect();
            GC.Collect();
            GC.WaitForFullGCApproach();
			#endif

            return String.Format("It took {0}ms to create a Fallen-8 graph with {1} nodes and {2} edges per node.", sw.Elapsed.TotalMilliseconds, nodeCount, edgeCount);
        }

        public string Bench(string iterations)
        {
            return IntroProvider.Bench(_fallen8, Convert.ToInt32(iterations));
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //do nothing atm
        }

        #endregion
    }
}
