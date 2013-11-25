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
        public static object file(string fname)
        {
            var fi = new FileInfo(fname);
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
