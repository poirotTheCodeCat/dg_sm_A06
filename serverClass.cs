using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace dg_sm_A06
{
    class serverClass
    {
        private static List<TcpClient> clientList = new List<TcpClient>();          // keeps track of the clients that are connected
        private static List<Thread> threadList = new List<Thread>();

        TcpListener server = null;

        public volatile bool isRunning = true;
        public static volatile bool pause = false;

        public serverClass()
        {
        }

        public void chatServer()
        {
            try
            {
                // set up the tcpListener on port15000
                Int32 port = 15000;
                IPAddress localIP = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localIP, port);    // set up server to listen for incoming connections
                server.Start();     // start listening on the server

                while (isRunning)
                {
                    server.BeginAcceptTcpClient(new AsyncCallback(waitForMessage), server);        // accept an incoming connection
                    Logger.TxtLog("Server Connected");
                }
            }
            catch (SocketException e)
            {
                //Console.WriteLine("Socket Exception: {0}", e);
                string chatError = "Exception Occured: " + e;
                Logger.TxtLog(chatError);
            }
            finally
            {
                server.Stop();  // stop listening for new clients
                Logger.TxtLog("Server service has been closed");
            }
        }

        /*
        * Function : waitForMessage()
        * Parameters : object o
        * Description : This waits for a client to send a message and calls upon another function to send 
        *              that message to all other connected users
        * Returns : Nothing
        */
        public static void waitForMessage(IAsyncResult asyncResult)
        {
            TcpListener server = (TcpListener)asyncResult.AsyncState;
            TcpClient client = server.EndAcceptTcpClient(asyncResult);

            if (!pause)
            {
                Byte[] bytes = new byte[256];       // bytes will be used to read data
                String data = null;                 // this string will be used to read data

                data = null;
                NetworkStream sendStream;
                NetworkStream stream = client.GetStream();      // used to recieve message
                int i;

                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0) // iterate through read stream
                {
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);   // convert bytes recieved to a string
                                                                                //Console.WriteLine("{0}", data);

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
        }

        /*
         * Function : stop()
         * Parameters : None
         * Descriptions : This function is called when the user stops the service
         * Returns : Nothing
         */
        public void stop()
        {
            try
            {
                server.Stop();
                isRunning = false;
                Logger.TxtLog("Service has been stopped");
            }
            catch(Exception e)
            {
                string exception = "Exception Thrown : " + e;
                Logger.TxtLog(exception);
            }
        }

        /*
         * Function : continueRun()
         * Parameters : nothing
         * Descriptions :  This function is called when the user Continues the service, if the service is paused when 
         *                 called, the service resumes as normal
         * Returns : Nothing
         */
        public void continueRun()
        {
            pause = !pause;
            string continueMessage = "Chat Server Service has resumed";
            Logger.TxtLog(continueMessage);
        }

        /*
         * Function : pauseServer()
         * Parameters : None
         * Descriptions : This function is called when the user pauses the service. If the service is currently running, then it is
         *                paused, and no longer functions (The server no longer accepts clients)
         * Returns : Nothing
         */
        public void pauseServer()
        {
            pause = !pause;
            string pauseMessage = "Chat Server Service has been paused";
            Logger.TxtLog(pauseMessage);
        }
    }
}
