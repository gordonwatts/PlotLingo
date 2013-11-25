using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoFunctionality.Plots
{
    public class PlotContext
    {
        /// <summary>
        /// The list of plots we are most insterested in.
        /// </summary>
        private ROOTNET.NTH1[] _plots;

        /// <summary>
        /// Create the plot conext with a list of plots
        /// </summary>
        /// <param name="nTH1"></param>
        public PlotContext(ROOTNET.NTH1[] nTH1)
        {
            this._plots = nTH1;
        }
    }
}
