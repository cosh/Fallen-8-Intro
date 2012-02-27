using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.Generic;

namespace Intro.Service
{
    /// <summary>
    ///   The interface that combines all the other services
    /// </summary>
    [ServiceContract]
    public interface IIntroService
    {
		#region misc
		
        /// <summary>
        /// Clears Fallen-8 and indices
        /// </summary>
        [OperationContract]
        [WebGet(UriTemplate = "/TabulaRasa")]
        void Clear();

        /// <summary>
        /// Gets a status
        /// </summary>
        /// <returns>some stats</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/Status")]
        String Status();
		
		#endregion
		
		#region wortschatz
		
		/// <summary>
        /// Imports a database
        /// </summary>
        /// <returns>Some stats</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/Wortschatz/Import")]
        String ImportWortschatz();

        /// <summary>
        /// Execute queries on wortschatz
        /// </summary>
        /// <param name="queryIdentifier">The name of the query that should be executed</param>
        /// <returns>List of execution times in ms</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/Wortschatz/Execute/{queryIdentifier}", ResponseFormat = WebMessageFormat.Json)]
        List<Double> ExecuteQuery(String queryIdentifier);

        /// <summary>
        /// Returns the available queries
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "/Wortschatz/AvailableQueries", ResponseFormat = WebMessageFormat.Json)]
        List<String> GetAvailableQueries();
		
		#endregion
		
		#region IEquallyDistributed
		
		/// <summary>
        /// Create an equally distributed graph
        /// </summary>
        /// <param name="nodeCount">Node count</param>
        /// <param name="edgeCount">Edges per node</param>
        /// <returns>Some stats</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/EquallyDistributed/CreateGraph?nodes={nodeCount}&edgesPerNode={edgeCount}")]
        String CreateGraph(String nodeCount, String edgeCount);

        /// <summary>
        /// Get the count of traversed edges per second
        /// </summary>
        /// <param name="iterations">Iterations</param>
        /// <returns>Some stats</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/EquallyDistributed/TPS?iterations={iterations}")]
        String Bench(String iterations);
		
		#endregion
    }
}
