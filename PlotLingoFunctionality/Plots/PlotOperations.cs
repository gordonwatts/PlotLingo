using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace PlotLingoFunctionality.Plots
{
    /// <summary>
    /// Define some operations for the language that work on plots. Note we clone everything!
    /// </summary>
    [Export(typeof(IFunctionObject))]
    class PlotOperations : IFunctionObject
    {
        /// <summary>
        /// Add two histograms
        /// </summary>
        /// <param name="h1"></param>
        /// <param name="h2"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 OperatorPlus(IScopeContext ctx, ROOTNET.Interface.NTH1 h1, ROOTNET.Interface.NTH1 h2)
        {
            var clone = h1.Clone() as ROOTNET.Interface.NTH1;
            clone.Add(h2);
            Tags.CopyTags(ctx, h1, clone);
            Tags.CopyTags(ctx, h2, clone);
            return clone;
        }

        /// <summary>
        /// Scale the contents of a histogram up or down.
        /// </summary>
        /// <param name="h"></param>
        /// <param name="scaleFactor"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 OperatorMultiply(IScopeContext ctx, ROOTNET.Interface.NTH1 h, double scaleFactor)
        {
            var clone = h.Clone() as ROOTNET.Interface.NTH1;
            clone.Scale(scaleFactor);
            Tags.CopyTags(ctx, h, clone);
            return clone;
        }

        /// <summary>
        /// Divide one histogram by another
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="hNumerator"></param>
        /// <param name="hDenomenator"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 OperatorDivide(IScopeContext ctx, ROOTNET.Interface.NTH1 hNumerator, ROOTNET.Interface.NTH1 hDenomenator)
        {
            var clone = hNumerator.Clone() as ROOTNET.Interface.NTH1;
            if (!clone.Divide(hDenomenator))
                throw new InvalidOperationException("ROOT refused to divide two histograms");
            Tags.CopyTags(ctx, hNumerator, clone);
            Tags.CopyTags(ctx, hDenomenator, clone);
            return clone;
        }

        /// <summary>
        /// Normalize the area of the plot to a specified number.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="plot"></param>
        /// <param name="totalArea"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 normalize(IScopeContext ctx, ROOTNET.Interface.NTH1 plot)
        {
            var np = plot.Clone() as ROOTNET.Interface.NTH1;
            Tags.CopyTags(ctx, plot, np);
            var area = np.Integral();
            np.Scale(1.0 / area);
            return np;
        }

        /// <summary>
        /// Rebin by a factor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="plot"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 rebin(IScopeContext ctx, ROOTNET.Interface.NTH1 plot, int rebinFactor)
        {
            var np = plot.Clone() as ROOTNET.Interface.NTH1;
            np.Rebin(rebinFactor);
            return np;
        }

        /// <summary>
        /// Given a 2D plot, map to a 1D plot assuming radial distances.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="plot"></param>
        /// <returns>1D plot of the 2D plot</returns>
        /// <remarks>
        /// - Radial conversion assumes that both coordinates are the same units. For example, if they are x and y distances.
        /// - Overflow & underflow bins are not handled.
        /// - Errors are not properly handled.
        /// </remarks>
        public static ROOTNET.Interface.NTH1 asRadial(IScopeContext ctx, ROOTNET.Interface.NTH2 plot)
        {
            // First, determine the distances
            var xmin = plot.Xaxis.GetBinLowEdge(1);
            var ymin = plot.Yaxis.GetBinLowEdge(1);
            var xmax = plot.Xaxis.GetBinUpEdge(plot.Xaxis.Nbins);
            var ymax = plot.Yaxis.GetBinUpEdge(plot.Yaxis.Nbins);

            var rmin = Math.Sqrt(xmin * xmin + ymin * ymin);
            var rmax = Math.Sqrt(xmax * xmax + ymax * ymax);

            var nbin = Math.Max(plot.Xaxis.Nbins, plot.Yaxis.Nbins);

            var result = new ROOTNET.NTH1F(plot.Name, plot.Title, nbin, rmin, rmax);

            // Loop over, adding everything in.
            for (int i_x = 1; i_x <= plot.Xaxis.Nbins; i_x++)
            {
                for (int i_y = 1; i_y <= plot.Yaxis.Nbins; i_y++)
                {
                    var x = plot.Xaxis.GetBinCenter(i_x);
                    var y = plot.Yaxis.GetBinCenter(i_y);
                    var r = Math.Sqrt(x * x + y * y);

                    result.Fill(r, plot.GetBinContent(i_x, i_y));
                }
            }

            return result;
        }

        /// <summary>
        /// Given a 2D plot, turn it into an efficiency plot. You can control the cut on each axis ( greater than or less than ).
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="plot">The 2D plot to convert to an efficiency map</param>
        /// <param name="xCutGreaterThan">True if the cut is greater than along the x axis</param>
        /// <param name="yCutGreaterThan">True if the cut is greater than along the y axis</param>
        /// <returns>The efficiency map</returns>
        /// <remarks>
        /// The overflow and underflow bins are taken into account.
        /// 
        /// This works on a histogram, and thus a bin. So we have to define what the cut means. Since the cut is only has a real
        /// value at the bin edges, we must define it as such.
        /// 
        /// The algorithm sums over the columns, and adds the last column to each place as it goes.
        /// 
        /// </remarks>
        public static ROOTNET.Interface.NTH2 asEfficiency(IScopeContext ctx, ROOTNET.Interface.NTH2 plot, bool xCutGreaterThan = true, bool yCutGreaterThan = true)
        {
            var result = plot.Clone() as ROOTNET.Interface.NTH2; ;

            var xBins = plot.Xaxis.Nbins;
            var yBins = plot.Yaxis.Nbins;

            var xBinRange = BinOrdering(xCutGreaterThan, xBins);
            var yBinRange = BinOrdering(yCutGreaterThan, yBins);

            // Build a 2D with the actual size
            int lastxBin = -1;
            double runningSum = 0.0;
            foreach (var ixBin in xBinRange)
            {
                double columnTotal = 0.0;
                foreach (var iyBin in yBinRange)
                {
                    var binValue = plot.GetBinContent(ixBin, iyBin);

                    columnTotal += binValue;
                    runningSum += binValue;
                    var lastColTotal = lastxBin < 0 ? 0.0 : result.GetBinContent(lastxBin, iyBin);

                    result.SetBinContent(ixBin, iyBin, columnTotal + lastColTotal);
                }
                lastxBin = ixBin;
            }

            // Turn it into an efficiency
            foreach (var ixBin in xBinRange)
            {
                foreach (var iyBin in yBinRange)
                {
                    result.SetBinContent(ixBin, iyBin, 1.0 - result.GetBinContent(ixBin, iyBin) / runningSum);
                }
            }

            return result;
        }

        /// <summary>
        /// Helper function to set the order we move through a set of columns or rows.
        /// </summary>
        /// <param name="moveForward">Go forward if true, otherwise reverse</param>
        /// <param name="totalBins">Total number of bins we should be looking at.</param>
        /// <returns></returns>
        private static IEnumerable<int> BinOrdering(bool moveForward, int totalBins)
        {
            var xBinRange = Enumerable.Range(0, totalBins + 2);
            if (!moveForward)
            {
                xBinRange = xBinRange.Reverse();
            }

            return xBinRange;
        }
    }
}
