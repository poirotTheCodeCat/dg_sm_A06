/*
 * File : Logger.cs
 * Programmers : Daniel Grew and Sasha Malesevic
 * Date Last Modified : 2019-11-06
 * Description :  This file contains the Logger class, which is used to create a file log whenever the service changes state 
 *                  (stopped, started, paused, or continued)
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
namespace dg_sm_A06
{
    static class Logger
    {

        /*
         * Function : Log()
         * parameters : string message
         * Description : This function enters a message into an event log
         * Returns : Nothing 
         */
        public static void Log(string message)
        {
            EventLog serviceEventLog = new EventLog();
            if (!EventLog.SourceExists("MyEventSource"))
            {
                EventLog.CreateEventSource("MyEventSource", "MyEventLog");
            }
            serviceEventLog.Source = "MyEventSource";
            serviceEventLog.Log = "MyEventLog";
            serviceEventLog.WriteEntry(message);
        }

        /*
         * Function : TxtLog()
         * parameters : string logInfo
         * Description : This function enters a message into a text file in the current directory of the executable for this program
         * Returns : Nothing 
         */
        public static void TxtLog(string logInfo)
        {
            string logFile =  AppDomain.CurrentDomain.BaseDirectory +"log.txt";     // this is the name of the text file to be written to
            logInfo = logInfo + "\n";
            if(!File.Exists(logFile))               // check if the file actually exists
            {
                StreamWriter write = File.CreateText(logFile);          // create a text file 
                try
                {   
                    write.WriteLine(logInfo);                           // write to the newly created file
                }
                catch(IOException io)
                {
                    return;
                }
            }
            else
            {
                StreamWriter append = File.AppendAllText(logFile, logFile);         // if file exists, append the message to the end of the file
                try
                {
                    append.WriteLine(logInfo);
                }
                catch(IOException ioe)
                {
                    return;
                }
            }
        }

    }
}
