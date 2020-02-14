using NLog;
using System;
using System.Diagnostics;

namespace ProcessScanner
{
    
    public enum Status
    {
        Running,
        Terminated
    }

    public class ProcessInstance
    {
        private readonly Process process;
        private readonly int workTimeLimit;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public ProcessInstance(Process process, int workTimeLimit)
        {
            this.process = process;
            this.workTimeLimit = workTimeLimit;
        }

        public string Name => process.ProcessName;
        public int ID => process.Id;
        private int TimeDelta { 
            get
            {
                DateTime timeNow = DateTime.Now;
                var delta = timeNow - process.StartTime;
                return (int)delta.TotalMinutes;
            }
        }

        public Status ProcessStatus
        {
            get
            {
                if (process.HasExited)
                    return Status.Terminated;

                if (workTimeLimit <= TimeDelta)
                {
                    try
                    {
                        process.Kill();
                        logger.Info("SUCCESS! Process {0} with ID {1} terminated", Name, ID);
                        return Status.Terminated;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Error while terminating process {0} with ID {1}", Name, ID);
                    }
                }
                return Status.Running;
            }
        }
    }
}
