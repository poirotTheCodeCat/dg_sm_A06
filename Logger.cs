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

        public static void TxtLog(string logInfo)
        {
            string logFile = "log.txt";
            logInfo = logInfo + "\n";
            if(!File.Exists(logFile))
            {
                StreamWriter write = File.CreateText(logFile);
                try
                {
                    write.WriteLine(logInfo);
                }
                catch(IOException io)
                {
                    return;
                }
            }
            else
            {
                StreamWriter append = File.AppendText(logFile);
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
