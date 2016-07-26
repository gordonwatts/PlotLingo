using PlotLingoLib;
using ROOTNET.Globals;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoFunctionality.ROOT
{
    /// <summary>
    /// Make it possible to load a ROOT .C macro (e.g. the atlas style).
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class RootMacros : IFunctionObject
    {
        public static void loadMacro (string fname)
        {
            var f = new FileInfo(fname);
            if (!f.Exists)
            {
                throw new FileNotFoundException($"ROOT::LoadMacro file '{f.FullName}' was not found.");
            }

            var r = gROOT.Value.LoadMacro(f.FullName);
            if (r != 0)
            {
                throw new InvalidOperationException($"Failed to load macro in file '{f.FullName}'");
            }
        }
    }
}
