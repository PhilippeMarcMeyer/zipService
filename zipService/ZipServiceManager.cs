using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using log4net;
using System.Threading;
using System.IO;
using System.IO.Compression;

namespace zipService
{
   public class ZipServiceManager
    {
        private string SourcePath;
        private string TargetPath;
        const int defaultLoopTimerInSeconds = 15;
        public static int zipLoopTimerInSeconds;


        public Timer timerManager;
        public static TimeSpan CheckInterval;
        private ManualResetEvent AbortEvent = new ManualResetEvent(false);


        public ZipServiceManager()
        {
            int test = 0;
            string settingLoop = ConfigurationManager.AppSettings["loopSpeedInSeconds"];
            zipLoopTimerInSeconds = int.TryParse(settingLoop, out test) ? test : defaultLoopTimerInSeconds;
            CheckInterval = TimeSpan.FromSeconds(zipLoopTimerInSeconds);

            SourcePath = ConfigurationManager.AppSettings["sourceFolder"];
            TargetPath = ConfigurationManager.AppSettings["targetFolder"];

            if (string.IsNullOrEmpty(SourcePath))
            {
                LogManager.GetLogger("ERROR").Info("SourcePath is not defined in configuration file");
                StopService();
            }

            if (string.IsNullOrEmpty(TargetPath))
            {
                LogManager.GetLogger("ERROR").Info("TargetPath is not defined in configuration file");
                StopService();
            }

        }

        public void StartService()
        {
            bool firstCheck = true;
            while (firstCheck || this.AbortEvent.WaitOne(CheckInterval) == false) // wait until 'CheckInterval' elapsed or abort flag set
            {
                firstCheck = false;
                this.Execute();
            }
        }

        private void Execute()
        {
            DirectoryInfo sourceFolder = Directory.CreateDirectory(SourcePath);
            DirectoryInfo destFolder = Directory.CreateDirectory(TargetPath);
            List<string> localFilesList = new List<string>();
            try
            {
                localFilesList = Directory.GetFiles(SourcePath, "*").ToList();
                if (localFilesList != null) 
                {
                    if (localFilesList.Any()) 
                    {
                        foreach(string fullpath in localFilesList)
                        {
                            string srcFilename = Path.GetFileName(fullpath);
                            string destFilename = srcFilename;
                            string ext = Path.GetExtension(destFilename);
                            if (ext != "")
                            {
                                destFilename = destFilename.Substring(0, destFilename.Length - ext.Length);
                            }

                            string fullDestPath = Path.Combine(TargetPath, destFilename+".zip");
                            int cpt = 1;
                            while(File.Exists(fullDestPath))
                            {
                                cpt++;
                                fullDestPath = Path.Combine(TargetPath, destFilename + string.Format(@"({0}).zip", cpt));
                            }
                            using (FileStream fs = new FileStream(fullDestPath, FileMode.Create))
                            using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
                            {
                                arch.CreateEntryFromFile(fullpath, srcFilename);
                            }
                            
                            File.Delete(fullpath);
                            LogManager.GetLogger("SERVICE").InfoFormat("File zipped : {0} => {1}", srcFilename, Path.GetFileName(fullDestPath));
                        }

                    }
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                LogManager.GetLogger("ERROR").Info(ex.Message);
                LogManager.GetLogger("ERROR").InfoFormat("Cannot check files in {0}", SourcePath);
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("ERROR").Info(ex.Message);
            }
        }

        public void StopService()
        {
            this.AbortEvent.Set();
            LogManager.GetLogger("SERVICE").Info("ZIP service stopped");
        }

        public void StopConsole()
        {
            LogManager.GetLogger("SERVICE").Info("ZIP service stopped");
            Environment.Exit(0);
        }
    }
}
