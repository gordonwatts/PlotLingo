using PlotLingoFunctionality;
using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace PlotLingoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var fi = new FileInfo(args[0]);

            if (!fi.Exists)
            {
                Console.WriteLine("Can't find the file {0} yet.", fi.FullName);
                return;
            }

            // Watch that file for modifications.
            // Also keep track of last write time because we have to deal with "bounce" - many programs
            // will trigger multiple events in the file system when they save the text (e.g. notepad, notpad++).

            var watcher = new FileSystemWatcher(fi.DirectoryName, string.Format("*{0}", fi.Extension));
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            var lastWriteTime = fi.LastWriteTime;
            watcher.Changed += (o, e) =>
            {
                if (e.FullPath == fi.FullName && lastWriteTime != File.GetLastWriteTime(fi.FullName))
                {
                    Console.WriteLine("Just got called");
                    Parse(fi);

                    fi.Refresh();
                    lastWriteTime = fi.LastWriteTime;
                }
            };
            watcher.EnableRaisingEvents = true;

            // First time we should run it...

            Parse(fi);

            // Sit there and wait until we are killed or...

            while (Console.Read() != 'q') ;
        }

        /// <summary>
        /// Run the parser over the default file and the plotting file.
        /// </summary>
        /// <param name="fi"></param>
        private static void Parse(FileInfo fi)
        {
            // First, are there some default files we should be loading up?
            var defaultFile = new FileInfo(string.Format("{0}/defaults.plotlingo", Path.GetDirectoryName(typeof(Program).Assembly.Location)));
            List<FileInfo> files = new List<FileInfo>();
            if (defaultFile.Exists)
                files.Add(defaultFile);
            files.Add(fi);

            // Now, run it!
            Console.WriteLine("Parsing & Executing...");

            var results = new List<object>();
            RunPlot.Eval(files, new Action<object>[] { o => results.Add(o) });

            // For each result, see if it can be reported or not.

            Console.WriteLine("There were {0} results.", results.Count);
            int sequenceNumber = 0;
            foreach (var r in results)
            {
                var pr = r as IPlotScriptResult;
                if (pr != null)
                {
                    // Generate a filename for saving this data
                    var outFNameStub = string.Format("{0}/{1}-{2}-{3}", fi.DirectoryName, Path.GetFileNameWithoutExtension(fi.Name), pr.Name, sequenceNumber);
                    sequenceNumber++;
                    var outputs = pr.Save(outFNameStub);
                    foreach (var o in outputs)
                    {
                        Console.WriteLine("  Wrote {0}", o.Name);
                    }
                }
            }
        }
    }
}
