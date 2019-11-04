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
    public partial class MyService : ServiceBase
    {
        public MyService()
        {
            InitializeComponent();
            CanPauseAndContinue = true;
        }

        protected override void OnStart(string[] args)
        {
            string startMessage = "Chat Server Service has started";
            Logger.Log(startMessage);
            Logger.TxtLog(startMessage);
        }

        protected override void OnStop()
        {
            string stopMessage = "Chat Server Service has stopped";
            Logger.Log(stopMessage);
            Logger.TxtLog(stopMessage);
        }

        protected override void OnContinue()
        {
            string continueMessage = "Chat Server Service has resumed";
            base.OnContinue();
            Logger.Log(continueMessage);
            Logger.TxtLog(continueMessage);
        }

        protected override void OnPause()
        {
            base.OnPause();
            string pauseMessage = "Chat Server Service has been paused";
            Logger.Log(pauseMessage);
            Logger.TxtLog(pauseMessage);
        }
    }
}
