using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotLingoFunctionality.Plots
{
    [Export(typeof(IFunctionObject))]
    class GraphMaker : IFunctionObject
    {
        /// <summary>
        /// Return a graph plot
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public static GraphContext draw(ROOTNET.NTGraph g)
        {
            return new GraphContext(g);
        }

        /// <summary>
        /// Draw a bunch of them.
        /// </summary>
        /// <param name="plotList"></param>
        /// <returns></returns>
        public static GraphContext draw(IEnumerable<object> plotList)
        {
            return new GraphContext(plotList.Cast<ROOTNET.Interface.NTGraph>().ToArray());
        }
    }
}
