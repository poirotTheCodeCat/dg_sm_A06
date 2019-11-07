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
        static serverClass server;
        static Thread runServer;
        public MyService()
        {
            InitializeComponent();
            CanPauseAndContinue = true;

            server = new serverClass();

            ThreadStart tStart = new ThreadStart(server.chatServer);
            runServer = new Thread(tStart);
        }


        /*
         * Function : OnStart()
         * Parameters : string[] args
         * Descriptions : This function is called when the user starts the service
         * Returns : Nothing
         */
        protected override void OnStart(string[] args)
        {
            runServer.Start();
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
            try
            {
                runServer.Abort();
            }
            catch (ThreadAbortException e)
            {
                server.stop();
                string stopMessage = "Chat Server Service has stopped";
                Logger.TxtLog(stopMessage);
                base.OnStop();
            }
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
            server.continueRun();
            runServer.Interrupt();
            string continueMessage = "Chat Server Service has resumed";
            base.OnContinue();
            Logger.TxtLog(continueMessage);
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
            server.pause();
            base.OnPause();
            string pauseMessage = "Chat Server Service has been paused";
            Logger.TxtLog(pauseMessage);
        }
    }

}

