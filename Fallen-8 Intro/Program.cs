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

            fallen8.ServiceFactory.StartGraphService();
            fallen8.ServiceFactory.StartAdminService();

			#endregion
			
            #region intro api

            IService introService;
            fallen8.ServiceFactory.TryAddService(out introService, "IntroRESTService", "Intro API", null);
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
