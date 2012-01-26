using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Intro.Service
{
    /// <summary>
    ///   The wortschatz interface
    /// </summary>
    [ServiceContract]
    public interface IWortschatz
    {
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
    }
}
