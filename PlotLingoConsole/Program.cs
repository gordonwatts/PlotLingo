using PlotLingoFunctionality;
using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlotLingoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // What file are we going to monitor and parse?
            var fname = GetFilenameOfScript(args);
            if (fname == null)
            {
                Console.WriteLine("Invoke this application by opening a plot lingo script file (.plotlingo extension).");
                return;
            }

            var fi = new FileInfo(fname);

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
                    ProtectedParse(fi);

                    fi.Refresh();
                    lastWriteTime = fi.LastWriteTime;
                }
            };
            watcher.EnableRaisingEvents = true;

            // First time we should run it...

            ProtectedParse(fi);

            // Sit there and wait until we are killed or...

            while (Console.Read() != 'q') ;
        }

        /// <summary>
        /// Parse, but protect against crashes.
        /// </summary>
        /// <param name="fi"></param>
        private static void ProtectedParse(FileInfo fi)
        {
            try
            {
                Parse(fi);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed during parse: {0}", e.Message);
            }
            catch
            {
                Console.WriteLine("Unknown error occured");
            }
        }

        /// <summary>
        /// Get the filename of the script we are to proces
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string GetFilenameOfScript(string[] args)
        {
            // From the command line?
            if (args.Length > 0)
            {
                return args[0];
            }

            // Via click-once?
            var activationData = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
            if (activationData != null && activationData.Length > 0)
            {
                Uri uri = new Uri(activationData[0]);
                string fileNamePassedIn = uri.LocalPath.ToString();
                return fileNamePassedIn;
            }

            return null;
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

            Console.WriteLine("There were {0} results.", results.Where(r => r is IPlotScriptResult).Count());
            int sequenceNumber = 0;
            foreach (var r in results)
            {
                var pr = r as IPlotScriptResult;
                if (pr != null)
                {
                    // Generate a filename for saving this data
                    var outFNameStub = string.Format("{0}/{1}-{2}-{3}", fi.DirectoryName, Path.GetFileNameWithoutExtension(fi.Name), pr.Name, sequenceNumber).FileNameSantize();
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
