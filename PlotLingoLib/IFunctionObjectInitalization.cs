using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoLib
{
    /// <summary>
    /// An initalization that is called fro each function object. Add this to the list of thing
    /// one inherrits from if you want the intialization to be called.
    /// </summary>
    public interface IFunctionObjectInitalization
    {
        /// <summary>
        /// Called with the root context
        /// </summary>
        /// <param name="ctx"></param>
        void Initalize(IScopeContext ctx);
    }
}
