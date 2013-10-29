using PlotLingoLib;
using System;
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

            var watcher = new FileSystemWatcher(fi.DirectoryName, string.Format("*{0}", fi.Extension));
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += (o, e) =>
            {
                if (e.FullPath == fi.FullName)
                {
                    Parse(fi);
                }
            };
            watcher.EnableRaisingEvents = true;

            // First time we should run it...

            Parse(fi);

            // Sit there and wait until we are killed or...

            while (Console.Read() != 'q') ;
        }

        private static void Parse(FileInfo fi)
        {
            Console.WriteLine("Parsing...");
            RunPlot.Eval(fi);
        }
    }
}
