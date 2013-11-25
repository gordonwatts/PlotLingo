using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib
{
    /// <summary>
    /// Attach this to any object that holds static methods that
    /// can be used as functions by the language framework. Use
    /// the MEF [Export(typeof(IFunctionObject))] to delcare it.
    /// </summary>
    public interface IFunctionObject
    {
    }
}
