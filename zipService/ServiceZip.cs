using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zipService
{
    public partial class ServiceZip : ServiceBase
    {
        private ZipServiceManager ZipService;
        private Thread ZipServiceThread;

        public ServiceZip()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.StartService();
        }

        protected override void OnStop()
        {
            this.StopService();
        }
        public void StartService()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            log4net.Util.SystemInfo.NullText = string.Empty;
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            CultureInfo culture = new CultureInfo("fr-FR");
            LogManager.GetLogger("SERVICE").Info("zip service started");

            this.ZipService = new ZipServiceManager();
            this.ZipServiceThread = new Thread(() => { ZipService.StartService(); });
            this.ZipServiceThread.Start();
        }

        public void StopService()
        {
            if (ZipService != null && this.ZipServiceThread != null)
            {
                this.ZipService.StopService();
                this.ZipServiceThread.Join();
            }

        }

    }
}
