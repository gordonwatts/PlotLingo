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
        /// <summary>
        /// Given a list of plots, return a plot context.
        /// </summary>
        /// <param name="plotList"></param>
        /// <returns></returns>
        public static PlotContext plot (object[] plotList)
        {
            return new PlotContext(plotList.Cast<ROOTNET.NTH1>().ToArray());
        }

        /// <summary>
        /// Create a plot context given a single plot.
        /// </summary>
        /// <param name="plot"></param>
        /// <returns></returns>
        public static PlotContext plot (ROOTNET.NTH1 plot)
        {
            return new PlotContext(new ROOTNET.NTH1[] { plot });
        }

        /// <summary>
        /// Add a context to turn off stat boxes
        /// </summary>
        public static void TurnOffStatBoxes(Context c)
        {
            c.AddPostCallHook("plot", (obj, result) =>
            {
                var pc = result as PlotContext;
                if (pc == null)
                    return result;

                pc.AddPreplotHook(plotContex =>
                {
                    if (plotContex.Plots.Length > 1)
                    {
                        foreach (var p in plotContex.Plots)
                        {
                            p.Stats = false;
                        }
                    }
                });

                return result;
            });
        }
    }
}
