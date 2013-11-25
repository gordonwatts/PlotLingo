using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoFunctionality.Plots
{
    /// <summary>
    /// Functions and methods having to do with plots
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class PlotMaker : IFunctionObject
    {
    }
}
