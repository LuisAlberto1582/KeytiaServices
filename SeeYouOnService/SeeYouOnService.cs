using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace SeeYouOnService
{
    public partial class SeeYouOnService : ServiceBase
    {
        SeeYouOnServiceBL.Service poService;
        System.Threading.Thread poThreadService;

        public SeeYouOnService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            poService = new SeeYouOnServiceBL.Service();

            poThreadService = new System.Threading.Thread(poService.Start);
            poThreadService.Start();
        }

        protected override void OnStop()
        {
            if (poService != null)
                poService.Stop();

            poService = null;
            poThreadService = null;
        }
    }
}
