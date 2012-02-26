using System;
using System.Collections.Generic;
using System.Net;
using Fallen8.API;
using Fallen8.API.Service;

namespace Intro
{
    class Program
    {
        static void Main(string[] args)
        {
            var shutdown = false;

            #region Fallen-8 startup

            var server = new Fallen8Server();

            #endregion

            #region services
			
			#region Fallen-8 REST API

            IFallen8Service fallen8RESTService;
            server.TryStartService(out fallen8RESTService, "Fallen-8_REST_Service", new Dictionary<string, object>
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
            server.TryStartService(out introService, "IntroRESTService", restServiceProperties);

            #endregion
			
            Console.WriteLine("Started services:");
            foreach (var aService in server.Services)
            {
                Console.WriteLine(aService.Description);
            }

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
            server.Shutdown();
            Console.WriteLine("Shutdown complete");

            #endregion
        }
    }
}
