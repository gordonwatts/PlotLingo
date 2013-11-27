﻿using PlotLingoLib;
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
        public static object file(string fname)
        {
            var fi = new FileInfo(fname);
            if (!fi.Exists)
            {
                throw new ArgumentException(string.Format("Unable to locate file {0}.", fi.FullName));
            }

            var f = ROOTNET.NTFile.Open(fname);
            if (!f.IsOpen())
                throw new ArgumentException(string.Format("ROOT is unable to open file {0}.", fi.FullName));

            return f;
        }
    }
}
