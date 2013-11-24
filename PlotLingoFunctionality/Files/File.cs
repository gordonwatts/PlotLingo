using System;
using System.IO;

namespace PlotLingoFunctionality.Files
{
    /// <summary>
    /// Return an open file
    /// </summary>
    class File
    {
        public static object Execute(object[] arguments)
        {
            if (arguments.Length != 1)
                throw new ArgumentException("arguments are only file path");

            if (!(arguments[0] is string))
                throw new ArgumentException("1st argument must be a string");

            var fi = new FileInfo(arguments[0] as string);
            if (!fi.Exists)
            {
                throw new ArgumentException(string.Format("Unable to locate file {0}.", fi.FullName));
            }

            var f = ROOTNET.NTFile.Open(fi.FullName);
            if (!f.IsOpen())
                throw new ArgumentException(string.Format("ROOT is unable to open file {0}.", fi.FullName));

            return f;
        }
    }
}
