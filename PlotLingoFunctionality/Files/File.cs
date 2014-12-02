using PlotLingoLib;
using System;
using System.ComponentModel.Composition;
using System.IO;

namespace PlotLingoFunctionality.Files
{
    /// <summary>
    /// Return an open file
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class File : IFunctionObject
    {
        /// <summary>
        /// Open a local file. Only a local file.
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        public static object file(IScopeContext c, string fname)
        {
            // See if we can locate the file. If a relative path look relative to the
            // script directory.

            FileInfo fi = null;
            if (Path.IsPathRooted(fname))
            {
                fi = new FileInfo(fname);
            }
            else
            {
                // Located near the script?
                fi = new FileInfo(Path.Combine(c.ExecutionContext.CurrentScriptFile.DirectoryName, fname));
                if (!fi.Exists)
                {
                    fi = new FileInfo(fname);
                }
            }

            if (!fi.Exists)
            {
                throw new ArgumentException(string.Format("Unable to locate file {0}.", fi.FullName));
            }

            var f = ROOTNET.NTFile.Open(fi.FullName);
            if (f == null || !f.IsOpen())
                throw new ArgumentException(string.Format("ROOT is unable to open file {0}.", fi.FullName));

            return f;
        }

        /// <summary>
        /// Search an input directory for a histogram.
        /// </summary>
        /// <param name="?"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object Get(ROOTNET.Interface.NTDirectory d, string path)
        {
            var h = d.Get(path);
            if (h == null)
                throw new ArgumentException(string.Format("Unable to locate histogram '{0}' in directory '{1}'", path, d.Name));
            return h;
        }
    }
}
