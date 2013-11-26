﻿using PlotLingoLib;
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

        private static void Parse(FileInfo fi)
        {
            Console.WriteLine("Parsing & Executing...");

            var results = new List<object>();
            RunPlot.Eval(fi, new Action<object>[] { o => results.Add(o) });
            Console.WriteLine("There were {0} results.", results.Count);
        }
    }
}
