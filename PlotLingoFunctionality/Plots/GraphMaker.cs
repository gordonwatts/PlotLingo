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
        /// Create a asymetric graph with a pave - upper and lower, from data (lines) in three histograms
        /// with a central value.
        /// </summary>
        /// <param name="central">The central line</param>
        /// <param name="upper">The upper line</param>
        /// <param name="lower">The lower line</param>
        /// <returns></returns>
        public static ROOTNET.NTGraph pave(IScopeContext ctx, ROOTNET.NTH1 central, ROOTNET.NTH1 upper, ROOTNET.NTH1 lower)
        {
            // Make sure we are ready to go!
            if (central.NbinsX != upper.NbinsX
                || central.NbinsX != lower.NbinsX)
            {
                throw new ArgumentException($"Can't make pave with (central: {central.Name}, upper: {upper.Name}, lower: {lower.Name}) as they don't all have the same binning!");
            }

            // Create the graph, use the central object to setup meta data.
            var g = new ROOTNET.NTGraphAsymmErrors(central.NbinsX);
            g.Name = $"{central.Name}_g";
            g.Title = central.Title;
            Tags.CopyTags(ctx, central, g);

            // loop through each point and set it.
            for (int idx = 0; idx < central.NbinsX; idx++)
            {
                g.SetPoint(idx, central.GetBinCenter(idx + 1), central.GetBinContent(idx + 1));
                g.SetPointEYhigh(idx, upper.GetBinContent(idx + 1) - central.GetBinContent(idx + 1));
                g.SetPointEYlow(idx, central.GetBinContent(idx + 1) - lower.GetBinContent(idx + 1));
            }
            
            // Wrap it in a context and go!
            return g;
        }

        public static ROOTNET.NTGraph asGraph(IScopeContext ctx, ROOTNET.NTH1 h)
        {
            return h.ConvertHistoToGraph(ctx);
        }

        /// <summary>
        /// Draw a bunch of them.
        /// </summary>
        /// <param name="plotList"></param>
        /// <returns></returns>
        public static GraphContext draw(IScopeContext ctx, IEnumerable<object> plotList)
        {
            return new GraphContext(plotList.ConvertToTGraph(ctx).ToArray());
        }
    }

    internal static class GraphMakerUtils
    {
        /// <summary>
        /// Attempt to convert each object to a TGraph
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<ROOTNET.NTGraph> ConvertToTGraph(this IEnumerable<object> source, IScopeContext ctx)
        {
            foreach (var o in source)
            {
                var directConversion = o as ROOTNET.NTGraph;
                if (directConversion != null)
                {
                    yield return directConversion;
                }
                else if (o is ROOTNET.NTH1)
                {
                    var histo = o as ROOTNET.NTH1;
                    ROOTNET.NTGraph g = histo.ConvertHistoToGraph(ctx);
                    yield return g;
                }
                else
                {
                    throw new InvalidCastException($"Unable to convert or cast object of type {o.GetType().Name} to a TGraph");
                }
            }
        }

        /// <summary>
        /// Convert a histogram to a graph, simply.
        /// </summary>
        /// <param name="histo"></param>
        /// <returns></returns>
        public static ROOTNET.NTGraph ConvertHistoToGraph(this ROOTNET.NTH1 histo, IScopeContext ctx)
        {
            var g = new ROOTNET.NTGraph(histo.NbinsX);

            // Copy meta-data.
            Tags.CopyTags(ctx, histo, g);
            g.Title = histo.Title;
            g.Name = $"{histo.Name}_g";
            g.LineWidth = histo.LineWidth;
            g.LineColor = histo.LineColor;
            g.LineStyle = histo.LineStyle;

            // And points.
            for (int i = 0; i < histo.NbinsX; i++)
            {
                g.SetPoint(i, histo.GetBinCenter(i + 1), histo.GetBinContent(i + 1));
            }

            return g;
        }
    }
}
