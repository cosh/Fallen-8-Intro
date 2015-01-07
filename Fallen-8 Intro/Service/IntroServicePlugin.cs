using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml;
using Framework.Serialization;
using NoSQL.GraphDB;
using NoSQL.GraphDB.Log;
using NoSQL.GraphDB.Service;

namespace Intro.Service
{
    public sealed class IntroServicePlugin : IService
    {
        #region Data

        /// <summary>
        ///   The starting time of the service
        /// </summary>
        private DateTime _startTime;

        /// <summary>
        ///   Is running?
        /// </summary>
        private Boolean _isRunning;

        /// <summary>
        ///   MetaData for this service
        /// </summary>
        private readonly Dictionary<String, String> _metaData = new Dictionary<string, string>();

        /// <summary>
        ///   The host that runs the service
        /// </summary>
        private ServiceHost _host;

        /// <summary>
        /// The intro service
        /// </summary>
        private IntroService _service;

        /// <summary>
        ///   Service description
        /// </summary>
        private String _description = "The Fallen-8 plugin that starts the intro service";

        /// <summary>
        /// The URI-Pattern of the service
        /// </summary>
        private String _uriPattern;

        /// <summary>
        /// The IP-Address of the service
        /// </summary>
        private IPAddress _address;

        /// <summary>
        /// The port of the service
        /// </summary>
        private UInt16 _port;

        /// <summary>
        /// The URI of the service
        /// </summary>
        private Uri _uri;

        /// <summary>
        /// REST service address
        /// </summary>
        private String _restServiceAddress;

        #endregion

        #region constructor

        /// <summary>
        ///   Initializes a new instance for plugin purpose
        /// </summary>
        public IntroServicePlugin()
        {
        }

        #endregion

        #region IFallen8Service Members

        public DateTime StartTime
        {
            get { return _startTime; }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public IDictionary<string, string> Metadata
        {
            get { return _metaData; }
        }

        public void OnServiceRestart()
        {
            //do nothing
        }

        public bool TryStop()
        {
            _service.Shutdown();
            _host.Close();

            return true;
        }

        public bool TryStart()
        {
            try
            {
                if (!_isRunning)
                {
                    _host.Open();

                    _isRunning = true;
                    _startTime = DateTime.Now;
                    Logger.LogInfo(_description + Environment.NewLine + "   -> Service is started at " + _uri + "/" + _restServiceAddress);   
                }
                else
                {
                    Logger.LogInfo(_description + Environment.NewLine + "   -> Service is already started at " + _uri + "/" + _restServiceAddress);                       
                }
            }
            catch (Exception e)
            {
                Logger.LogError(String.Format("Could not start service \"{0}\".{1}{2}", this.PluginName, Environment.NewLine, e.Message));

                return false;
            }

            return true;
        }

        #endregion

        #region IFallen8Plugin Members

        public void Save(SerializationWriter writer)
        {
            writer.Write(_uriPattern);
            writer.Write(_address.ToString());
            writer.Write(_port);
        }

        public void Load(SerializationReader reader, Fallen8 fallen8)
        {
            _uriPattern = reader.ReadString();
            _address = IPAddress.Parse(reader.ReadString());
            _port = reader.ReadUInt16();

            StartService(fallen8);
        }

        public string PluginName
        {
            get { return "IntroRESTService"; }
        }

        public Type PluginType
        {
            get { return typeof(IntroServicePlugin); }
        }

        public Type PluginCategory
        {
            get { return typeof(IService); }
        }

        public string Description
        {
            get { return _description; }
        }

        public string Manufacturer
        {
            get { return "Henning Rauch"; }
        }

        public void Initialize(Fallen8 fallen8, IDictionary<string, object> parameter)
        {
            _uriPattern = "Intro";
            if (parameter != null && parameter.ContainsKey("URIPattern"))
                _uriPattern = (String)Convert.ChangeType(parameter["URIPattern"], typeof(String));

            _address = IPAddress.Any;
            if (parameter != null && parameter.ContainsKey("IPAddress"))
                _address = (IPAddress)Convert.ChangeType(parameter["IPAddress"], typeof(IPAddress));

            _port = 2323;
            if (parameter != null && parameter.ContainsKey("Port"))
                _port = (ushort)Convert.ChangeType(parameter["Port"], typeof(ushort));

            StartService(fallen8);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            TryStop();
            _service.Dispose();
        }

        #endregion

        #region private helper methods

        /// <summary>
        ///   Start the intro service
        /// </summary>
        /// <param name="fallen8"> Fallen-8 instance </param>
        private void StartService(Fallen8 fallen8)
        {
            _uri = new Uri("http://" + _address + ":" + _port + "/" + _uriPattern);

            if (!_uri.IsWellFormedOriginalString())
                throw new Exception("The URI Pattern is not well formed!");

            _service = new IntroService(fallen8);

            _host = new ServiceHost(_service, _uri)
                        {
                            CloseTimeout = new TimeSpan(0, 0, 0, 0, 50)
                        };
            _restServiceAddress = "REST";

            try
            {
                var binding = new WebHttpBinding
                {
                    MaxBufferSize = 268435456,
                    MaxReceivedMessageSize = 268435456,
                    SendTimeout = new TimeSpan(1, 0, 0),
                    ReceiveTimeout = new TimeSpan(1, 0, 0)
                };

                var readerQuotas = new XmlDictionaryReaderQuotas
                {
                    MaxDepth = 2147483647,
                    MaxStringContentLength = 2147483647,
                    MaxBytesPerRead = 2147483647,
                    MaxNameTableCharCount = 2147483647,
                    MaxArrayLength = 2147483647
                };

                binding.ReaderQuotas = readerQuotas;

                var se = _host.AddServiceEndpoint(typeof(IIntroService), binding, _restServiceAddress);
                var webBehav = new WebHttpBehavior
                {
                    HelpEnabled = true
                };
                se.Behaviors.Add(webBehav);

                ((ServiceBehaviorAttribute)_host.Description.Behaviors[typeof(ServiceBehaviorAttribute)]).
                    InstanceContextMode = InstanceContextMode.Single;
            }
            catch (CommunicationException)
            {
                _host.Abort();
                throw;
            }
        }

        #endregion
    }
}
