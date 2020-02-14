using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace ProcessScanner
{
    /// <summary>
    /// Class for parsing command-line options
    /// </summary>
    public class Options
    {
        [Option('n', "name", Required = true, HelpText = "Specifies the process name or comma-separated processes list.", Separator = ',')]
        public IEnumerable<string> ProcessList { get; set; }

        [Option('l', "limit", Required = true, HelpText = "Specifies the limit of the process working time in minutes.")]
        public int TimeLimitMin { get; set; }

        [Option('i', "interval", Required = true, HelpText = "Specifies the repeat interval in minutes.")]
        public int IntervalMin { get; set; }

        [Usage(ApplicationAlias = "ProcessScanner.exe")]
        public static IEnumerable<Example> Examples => new List<Example>() {
                        new Example(helpTemplate, 
                                    new Options { ProcessList = new List<string>() {"notepad", "chrome", "mspaint" },
                                    TimeLimitMin = 10, IntervalMin = 1})
                    };

        public static string helpTemplate = @"
ProcessScanner[.exe] [--name <string>, <string>] [--limit <int>] [--interval <int>]

Description:
    This tool is used to terminate tasks by process if thir working time exceeds the limit.

Example:";
    }
}
