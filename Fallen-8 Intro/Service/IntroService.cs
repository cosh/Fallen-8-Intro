using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using NoSQL.GraphDB;
using NoSQL.GraphDB.Index;

namespace Intro.Service
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class IntroService : IIntroService, IDisposable
    {
        #region Data

        /// <summary>
        ///   The internal Fallen-8 instance
        /// </summary>
        private readonly Fallen8 _fallen8;

		/// <summary>
		/// The intro provider.
		/// </summary>
		private readonly IntroProvider _introProvider;
        
        #endregion

        #region constructor

        /// <summary>
        /// Creates a new import service
        /// </summary>
        /// <param name="fallen8"></param>
        public IntroService(Fallen8 fallen8)
        {
            _fallen8 = fallen8;
			_introProvider = new IntroProvider (fallen8);
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

        #region IEquallyDistributed Members

        public string CreateGraph(string nodeCount, string edgeCount)
        {

			var sw = Stopwatch.StartNew();

			_introProvider.CreateScaleFreeNetwork(Convert.ToInt32(nodeCount), Convert.ToInt32(edgeCount));

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
			return _introProvider.Bench(Convert.ToInt32(iterations));
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
