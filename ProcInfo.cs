using NLog;
using System;
using System.Diagnostics;

namespace ProcessScanner
{
    /// <summary>
    /// Class with information about each process
    /// </summary>
    public class ProcInfo
    {
        private readonly System.Diagnostics.Process process;

        private readonly int workTimeLimit;

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public ProcInfo(Process process, int workTimeLimit)
        {
            this.process = process;
            this.workTimeLimit = workTimeLimit;
        }

        private int TimeDelta { 
            get
            {
                var delta = DateTime.Now - process.StartTime;
                return (int)delta.TotalMinutes;
            }
        }

        public Status ProcessStatus
        {
            get
            {
                if (process.HasExited)
                {
                    return Status.Terminated;
                }
              
                if (workTimeLimit <= TimeDelta)
                {
                    try
                    {
                        process.Kill();
                        logger.Info("SUCCESS! Process {0} with ID {1} terminated", process.ProcessName, process.Id);
                        return Status.Terminated;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Error while terminating process {0} with ID {1}", process.ProcessName, process.Id);
                    }
                }
                return Status.Running;
            }
        }
    }

    public enum Status
    {
        Running,
        Terminated
    }
}
