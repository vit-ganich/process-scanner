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
        static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        static int Main(string[] args)
        {
            IEnumerable<string> processesNamesFromCMD = new List<string>();
            int timeLimitMin = 0;
            int intervalMin = 0;

            // Parse command-line arguments
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opt =>
                {
                    processesNamesFromCMD = opt.ProcessList;
                    timeLimitMin = opt.TimeLimitMin;
                    intervalMin = opt.IntervalMin;
                });

            // To terminate the program with invalid arguments
            if (timeLimitMin == 0 || intervalMin < 1)
            {
                logger.Info("Invalid arguments: 'limit' and 'interval' must be more than zero. Program stopped.");
                return 1;
            }

            logger.Info("Program started");

            List<Process> runningProcesses = GetProcessInstances(processesNamesFromCMD, timeLimitMin);

            int terminatedProcessesCount = 0;

            while (true)
            {
                foreach(var item in runningProcesses)
                {
                    if (item.ProcessStatus == Status.Terminated)
                        terminatedProcessesCount++;
                }
                
                if (runningProcesses.Count <= terminatedProcessesCount)
                {
                    logger.Info("All processes have been terminated. Program finished. Bye!\n");
                    break;
                }
                    
                logger.Debug("Wait for '{0}' minutes", intervalMin);
                Timer(intervalMin);
                Console.WriteLine();
                System.Threading.Thread.Sleep(intervalMin * 60000);
            }
            return 0;
        }

        /// <summary>
        /// Create a list of ProcessInstance instances for each process name from the command line
        /// </summary>
        /// <param name="procNamesCMD"></param>
        /// <param name="timeLimitMin"></param>
        /// <returns>List of </returns>
        static List<Process> GetProcessInstances(IEnumerable<string> procNamesCMD, int timeLimitMin)
        {
            var procInstList = new List<Process>();

            foreach(var procName in procNamesCMD)
            {
                logger.Debug("Looking for running processes with name '{0}'", procName);

                System.Diagnostics.Process[] runningProceses = System.Diagnostics.Process.GetProcessesByName(procName.Trim());

                if (runningProceses.Length == 0)
                {
                    logger.Warn("Process with name '{0}' not found", procName);
                    continue;
                }
                    
                foreach(var singleProc in runningProceses)
                {
                    var process = new Process(singleProc, timeLimitMin);
                    procInstList.Add(process);
                    logger.Info("Found process '{0}' with ID '{1}' | Started: {2}", process.Name, process.ID, process.StartTime.ToString("HH:mm:ss"));
                }
            }
            return procInstList;
        }

        static async void Timer(int intervalMin)
        {
            int seconds = intervalMin * 60;
            while (seconds != 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                Console.Write("\rCountdown: {0} sec\r", seconds);
                seconds--;
            }
        }
    }
}
