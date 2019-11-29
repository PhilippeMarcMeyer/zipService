using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace zipService
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        static void Main()
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                ServiceZip service = new ServiceZip();
                service.StartService();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new ServiceZip()
                };
                ServiceBase.Run(ServicesToRun);
            }

        }
    }
}
