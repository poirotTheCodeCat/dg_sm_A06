/*
 * File : MyServices.cs
 * Programmers : Daniel Grew and Sasha Malesevic
 * Date Last Modified : 2019-11-06
 * Description : This file contains the logic of the service - that is - this file contains the logic for the chat server
 *               which connects to a client, accepts messages, and sends that message to all other clients
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace dg_sm_A06
{
    public partial class MyService : ServiceBase
    {
        private static List<TcpClient> clientList = new List<TcpClient>();          // keeps track of the clients that are connected
        private static List<Thread> threadList = new List<Thread>();
        static volatile bool isRunning;
        public MyService()
        {
            InitializeComponent();
            CanPauseAndContinue = true;

            ThreadStart tStart = new ThreadStart(chatServer);
            Thread server = new Thread(tStart);
            server.Start();
        }

        public void chatServer()
        {
            TcpListener server = null;
            try
            {
                // set up the tcpListener on port15000
                Int32 port = 15000;
                IPAddress localIP = IPAddress.Parse("127.0.0.1");

                IPEndPoint clientIP;            // used to store the user's IP Address

                server = new TcpListener(localIP, port);    // set up server to listen for incoming connections
                server.Start();     // start listening on the server
                while (true)
                {
                    while (isRunning == true)
                    {
                        Console.WriteLine("Waiting to connect chat...");
                        TcpClient client = server.AcceptTcpClient();    // accept an incoming connection
                        Console.WriteLine("Connected to a chat...");

                        ParameterizedThreadStart startThread = new ParameterizedThreadStart(waitForMessage);    // declare a delegate pointing at waitForMessage()
                        Thread waitThread = new Thread(startThread);        // create a thread that will wait for an incoming message

                        clientIP = client.Client.LocalEndPoint as IPEndPoint;       // get the client's IP address

                        clientList.Add(client);       // add the client to the clientList
                        threadList.Add(waitThread);     // add the client thread to the clientList

                        waitThread.Start(client);     // start the thread
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket Exception: {0}", e);
            }
            finally
            {
                stopAllClients();       // close all clients
                server.Stop();
            }
        }

        /*
        * Function : waitForMessage()
        * Parameters : object o
        * Description : This waits for a client to send a message and calls upon another function to send 
        *              that message to all other connected users
        * Returns : Nothing
        */
        public static void waitForMessage(object o)
        {
            if (isRunning == false)
            {
                Thread.Sleep(Timeout.Infinite);
            }
            TcpClient client = (TcpClient)o;
            Byte[] bytes = new byte[256];       // bytes will be used to read data
            String data = null;                 // this string will be used to read data

            data = null;
            NetworkStream sendStream;
            NetworkStream stream = client.GetStream();      // used to recieve message
            int i;

            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0) // iterate through read stream
            {
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);   // convert bytes recieved to a string
                Console.WriteLine("{0}", data);

                foreach (TcpClient send in clientList)
                {
                    if (send != client)
                    {
                        sendStream = send.GetStream();
                        sendStream.Write(bytes, 0, bytes.Length);
                        sendStream.Flush();
                    }
                }
            }
            clientList.Remove(client);  // remove user from the user list
            client.Close(); // shut down connection when user disconnects      
        }


        /*
         * Function : stopAllClients()
         * Parameters : none
         * Description : This closes all of the connections to the clients and empties the dictionaries
         * Returns : nothing
         */
        static void stopAllClients()
        {
            foreach (TcpClient tcpSend in clientList)
            {
                tcpSend.Close();
            }
            clientList.Clear();
        }

        /*
         * Function : OnStart()
         * Parameters : string[] args
         * Descriptions : This function is called when the user starts the service
         * Returns : Nothing
         */
        protected override void OnStart(string[] args)
        {
            isRunning = true;
            string startMessage = "Chat Server Service has started";
            Logger.TxtLog(startMessage);
            base.OnStart(args);
        }

        /*
         * Function : OnStop()
         * Parameters : None
         * Descriptions : This function is called when the user stops the service
         * Returns : Nothing
         */
        protected override void OnStop()
        {
            isRunning = false;
            stopAllClients();
            string stopMessage = "Chat Server Service has stopped";
            Logger.TxtLog(stopMessage);
            base.OnStop();
        }

        /*
         * Function : OnContinue()
         * Parameters : nothing
         * Descriptions :  This function is called when the user Continues the service, if the service is paused when 
         *                 called, the service resumes as normal
         * Returns : Nothing
         */
        protected override void OnContinue()
        {
            if (isRunning == false)
            {
                isRunning = true;
                foreach (Thread t in threadList)
                {
                    t.Interrupt();
                }
                string continueMessage = "Chat Server Service has resumed";
                base.OnContinue();
                Logger.TxtLog(continueMessage);
            }
        }

        /*
         * Function : OnPause()
         * Parameters : None
         * Descriptions : This function is called when the user pauses the service. If the service is currently running, then it is
         *                paused, and no longer functions (The server no longer accepts clients)
         * Returns : Nothing
         */
        protected override void OnPause()
        {
            if (isRunning == true)
            {
                isRunning = false;
                base.OnPause();
                string pauseMessage = "Chat Server Service has been paused";
                Logger.TxtLog(pauseMessage);
            }
        }
    }

}

