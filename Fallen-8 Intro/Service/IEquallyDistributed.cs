using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Intro.Service
{
    /// <summary>
    ///   The interface for equally distributed graphs
    /// </summary>
    [ServiceContract]
    public interface IEquallyDistributed
    {
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
    }
}
