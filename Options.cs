using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace ProcessScanner
{
    /// <summary>
    /// Class for command-line options
    /// </summary>
    public class Options
    {
        [Option('n', "name", Required = true, HelpText = "Input a process name or comma-separated processes list", Separator = ',')]
        public IEnumerable<string> ProcessList { get; set; }

        [Option('l', "limit", Required = true, HelpText = "Input a max number of minutes of the process lifetime")]
        public int TimeLimitMin { get; set; }

        [Option('i', "interval", Required = true, HelpText = "Set a period in minures to repeat (default - 1 minute)")]
        public int IntervalMin { get; set; }


        [Usage(ApplicationAlias = "ProcessScanner.exe")]
        public static IEnumerable<Example> Examples => new List<Example>() {
                        new Example("Get processes list by names and get it's lifetime in minutes", new Options{ProcessList = new List<string>() {"notepad", "chrome" },
                            TimeLimitMin = 10, IntervalMin = 1})
                    };
    }
}
