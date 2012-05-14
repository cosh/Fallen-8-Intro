using System;
using System.Collections.Generic;
using System.Net;
using Fallen8.API.Service;

namespace Intro
{
    class Program
    {
        static void Main(string[] args)
        {
            var shutdown = false;

            #region Fallen-8 startup

            var fallen8 = new Fallen8.API.Fallen8();

            #endregion

            #region services
			
			#region Fallen-8 REST API

            IFallen8Service fallen8RESTService;
            fallen8.ServiceFactory.TryStartService(out fallen8RESTService, "Fallen-8_REST_Service", "Built-In API",  new Dictionary<string, object>
                                     {
                                         {"IPAddress", IPAddress.Parse(Server.Default.IPAdress)},
                                         {"Port", Server.Default.Port}
                                     });
			
			#endregion
			
            #region intro api

            var restServiceProperties = new Dictionary<string, object>
                                     {
                                         {"IPAddress", IPAddress.Parse(Server.Default.IPAdress)},
                                         {"Port", Server.Default.Port}
                                     };
            IFallen8Service introService;
            fallen8.ServiceFactory.TryStartService(out introService, "IntroRESTService", "Intro API", restServiceProperties);

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
