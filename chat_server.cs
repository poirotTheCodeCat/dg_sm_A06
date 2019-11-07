/*
 * File : chat_server.cs
 * Programmers : Daniel Grew and Sasha Malesevic
 * Date Last Modified : 2019-11-06
 * Description : this file contains the main function which starts the server
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace dg_sm_A06
{
    static class chat_server
    {
        private static List<TcpClient> clientList = new List<TcpClient>();          // keeps track of the clients that are connected
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new MyService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
