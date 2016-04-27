using PlotLingoLib;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

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
        /// <returns>A plot context that can be dumped</returns>
        /// <remarks>Attempt to find a way to turn this into a list of plots by looking at whatever object is associated with it</remarks>
        public static PlotContext plot(IEnumerable<object> plotList)
        {
            return new PlotContext(plotList.Cast<ROOTNET.NTH1>().ToArray());
        }

        /// <summary>
        /// Plot a list of values in a dictionary
        /// </summary>
        /// <param name="plotList"></param>
        /// <returns></returns>
        public static PlotContext plot(IDictionary<object, object> plotList)
        {
            return new PlotContext(plotList.Select(kv => kv.Value).Cast<ROOTNET.NTH1>().ToArray());
        }

        /// <summary>
        /// Create a plot context given a single plot.
        /// </summary>
        /// <param name="plot"></param>
        /// <returns></returns>
        public static PlotContext plot(ROOTNET.NTH1 plot)
        {
            return new PlotContext(new ROOTNET.NTH1[] { plot });
        }

        /// <summary>
        /// Turn off stat boxes if there is more than a single plot being shown.
        /// </summary>
        public static void TurnOffStatBoxesForMultiPlots(IScopeContext c)
        {
            c.ExecutionContext.AddPostCallHook("plot", "statsboxes", (obj, result) =>
            {
                var pc = result as DrawingContext;
                if (pc == null)
                    return result;

                pc.AddPreplotHook(plotContex =>
                {
                    if (plotContex.ObjectsToDraw.Count() > 1)
                    {
                        foreach (var p in plotContex.ObjectsToDraw)
                        {
                            if (p.NTObject is ROOTNET.Interface.NTH1)
                            {
                                (p.NTObject as ROOTNET.Interface.NTH1).Stats = false;
                            }
                        }
                    }
                });

                return result;
            });
        }

        /// <summary>
        /// Turn off stat boxes on plots.
        /// </summary>
        /// <param name="c"></param>
        public static void TurnOffStatBoxes(IScopeContext c)
        {
            c.ExecutionContext.AddPostCallHook("plot", "statsboxes", (obj, result) =>
            {
                var pc = result as DrawingContext;
                if (pc == null)
                    return result;

                pc.AddPreplotHook(plotContex =>
                {
                    foreach (var p in plotContex.ObjectsToDraw)
                    {
                        if (p.NTObject is ROOTNET.Interface.NTH1)
                        {
                            (p.NTObject as ROOTNET.Interface.NTH1).Stats = false;
                        }
                    }
                });

                return result;
            });
        }

        /// <summary>
        /// Make sure that everything fits
        /// </summary>
        /// <param name="c"></param>
        public static void NormalizePlots(IScopeContext c)
        {
            c.ExecutionContext.AddPostCallHook("plot", "normalize", (obj, result) =>
            {
                var pc = result as DrawingContext;
                if (pc == null)
                    return result;

                pc.AddPreplotHook(plotContex =>
                {
                    var histToDraw = plotContex.ObjectsToDraw
                        .Select(o => o.NTObject)
                        .Where(o => o is ROOTNET.Interface.NTH1)
                        .Cast<ROOTNET.Interface.NTH1>();
                    if (histToDraw.Count() > 1)
                    {
                        var max = histToDraw.Select(p => FindPlotMaximum(p)).Max();
                        max = max * 1.10;

                        foreach (var p in histToDraw)
                        {
                            p.Maximum = max;
                        }
                    }
                });

                return result;
            });
        }

        /// <summary>
        /// Determine the maximum value for a plot, taking the size of the errors into account.
        /// This isn't perfect, but does a slightly faster job than scanning everything. May have to fix eventually.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static double FindPlotMaximum(ROOTNET.Interface.NTH1 p)
        {
            var b = p.GetMaximumBin();
            return p.GetBinContent(b) + p.GetBinError(b);
        }

        /// <summary>
        /// Configure the line width for all plots
        /// </summary>
        /// <param name="c"></param>
        /// <param name="sz"></param>
        public static void ConfigureLineWidth(IScopeContext c)
        {
            short sz = 2;
            c.ExecutionContext.AddPostCallHook("plot", "linewidth", (obj, result) =>
            {
                var pc = result as DrawingContext;
                if (pc == null)
                    return result;

                pc.AddPreplotHook(plotContex =>
                {
                    foreach (var p in plotContex.ObjectsToDraw)
                    {
                        p.LineWidth = sz;
                    }
                });

                return result;
            });
        }
    }
}
