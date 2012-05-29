using System;
using System.Collections.Generic;
using System.Net;
using NoSQL.GraphDB;
using NoSQL.GraphDB.Service;

namespace Intro
{
    class Program
    {
        static void Main(string[] args)
        {
            var shutdown = false;

            #region Fallen-8 startup

            var fallen8 = new Fallen8();

            #endregion

            #region services
			
			#region Fallen-8 REST API

            IService fallen8RESTService;
            fallen8.ServiceFactory.TryAddService(out fallen8RESTService, "Fallen-8_REST_Service", "Built-In API",  new Dictionary<string, object>
                                     {
                                         {"IPAddress", IPAddress.Parse(Server.Default.IPAdress)},
                                         {"Port", Server.Default.Port}
                                     });
            fallen8RESTService.TryStart();
			
			#endregion
			
            #region intro api

            var restServiceProperties = new Dictionary<string, object>
                                     {
                                         {"IPAddress", IPAddress.Parse(Server.Default.IPAdress)},
                                         {"Port", Server.Default.Port}
                                     };
            IService introService;
            fallen8.ServiceFactory.TryAddService(out introService, "IntroRESTService", "Intro API", restServiceProperties);
            introService.TryStart();

            #endregion

            #endregion

            #region shutdown

            Console.WriteLine("Enter 'shutdown' to initiate the shutdown of this instance.");

            while (!shutdown)
            {
                var command = Console.ReadLine();

                if (command == null) continue;

                if (command.ToUpper() == "SHUTDOWN")
                    shutdown = true;
            }

            Console.WriteLine("Shutting down Fallen-8 intro");
            fallen8.Shutdown();
            Console.WriteLine("Shutdown complete");

            #endregion
        }
    }
}
