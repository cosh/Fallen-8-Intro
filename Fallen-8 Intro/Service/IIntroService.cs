using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Intro.Service
{
    /// <summary>
    ///   The interface that combines all the other services
    /// </summary>
    [ServiceContract]
    public interface IIntroService : IWortschatz, IEquallyDistributed
    {
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
    }
}
