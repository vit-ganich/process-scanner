using System.Collections.Generic;
using System.Diagnostics;
using CommandLine;
using NLog;
using System;
using System.Threading.Tasks;

namespace ProcessScanner
{
    class Program
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
//#if DEBUG
//                        args = new[] { "-n", "chrome, notepad", "-l", "0", "-i", "1" };
//#endif

            IEnumerable<string> processesNamesFromCMD = new List<string>();
            int timeLimitMin = 0;
            int intervalMin = 0;

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opt =>
                {
                    processesNamesFromCMD = opt.ProcessList;
                    timeLimitMin = opt.TimeLimitMin;
                    intervalMin = opt.IntervalMin;
                });

            var runningProcesses = GetProcessInstances(processesNamesFromCMD, timeLimitMin);

            var terminatedProcessesCount = 0;

            while (true)
            {
                foreach(var item in runningProcesses)
                {
                    if (item.ProcessStatus == Status.Terminated)
                        terminatedProcessesCount++;
                }
                
                if (runningProcesses.Count <= terminatedProcessesCount)
                {
                    logger.Info("Program finished. Bye!\n");
                    break;
                }
                    
                logger.Debug("Wait for '{0}' minutes", intervalMin);
                Timer(intervalMin);
                System.Threading.Thread.Sleep(intervalMin * 60000);
            }
        }

        /// <summary>
        /// Create a list of ProcessInstance instances for each process name from the command line
        /// </summary>
        /// <param name="procNamesCMD"></param>
        /// <param name="timeLimitMin"></param>
        /// <returns>List of </returns>
        static List<ProcessInstance> GetProcessInstances(IEnumerable<string> procNamesCMD, int timeLimitMin)
        {
            if (procNamesCMD != null && timeLimitMin != 0)
                logger.Info("Program started"); // To avoid unnesessary entries in the console

            var procInstList = new List<ProcessInstance>();

            foreach(var procName in procNamesCMD)
            {
                logger.Debug("Looking for running processes with name '{0}'", procName);

                Process[] runningProceses = Process.GetProcessesByName(procName.Trim());

                if (runningProceses.Length == 0)
                {
                    logger.Warn("Process with name '{0}' not found", procName);
                    continue;
                }
                    
                foreach(var singleProc in runningProceses)
                {
                    var process = new ProcessInstance(singleProc, timeLimitMin);
                    procInstList.Add(process);
                    logger.Info("Found process '{0}' with ID '{1}'", process.Name, process.ID);
                }
            }
            return procInstList;
        }

        private static async void Timer(int intervalMin)
        {
            int seconds = intervalMin * 60;
            while (seconds != 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                Console.Write("\rCountdown: {0} sec", seconds);
                seconds--;
            }
            Console.WriteLine("\r");
        }
    }
}
