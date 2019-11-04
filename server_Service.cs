using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace dg_sm_A06
{
    public partial class server_Service : ServiceBase
    {
        public server_Service()
        {
            InitializeComponent();
            CanPauseAndContinue = true;
        }

        protected override void OnStart(string[] args)
        {
            string startMessage = "Service has started";
            Logger.Log(startMessage);
        }

        protected override void OnStop()
        {
            string stopMessage = "Service has stopped";
            Logger.Log(stopMessage);
        }
    }
}
