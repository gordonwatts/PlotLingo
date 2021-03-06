﻿using PlotLingoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ROOTNET.Interface;

namespace PlotLingoFunctionality.Plots
{
    /// <summary>
    /// Define some operations for the language that work on plots. Note we clone everything!
    /// </summary>
    [Export(typeof(IFunctionObject))]
    public class PlotOperations : IFunctionObject
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
        /// Divide one histogram by another
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="hNumerator"></param>
        /// <param name="hDenomenator"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 OperatorMinus(IScopeContext ctx, ROOTNET.Interface.NTH1 left, ROOTNET.Interface.NTH1 right)
        {
            var clone = left.Clone() as ROOTNET.Interface.NTH1;
            clone.Add(right, -1.0);
            Tags.CopyTags(ctx, left, clone);
            Tags.CopyTags(ctx, right, clone);
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
        /// Set the minimum value for a plot
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="plot"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 minimum(IScopeContext ctx, ROOTNET.Interface.NTH1 plot, double value)
        {
            var np = plot.Clone() as ROOTNET.Interface.NTH1;
            Tags.CopyTags(ctx, plot, np);
            np.Minimum = value;
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
            Tags.CopyTags(ctx, plot, np);
            np.Rebin(rebinFactor);
            return np;
        }

        /// <summary>
        /// Attempt to rebin to a target number of bins. This is a little delicate as we we are restricted
        /// to rebinning to an integer.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="plot"></param>
        /// <param name="desiredBins"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 rebinTo(IScopeContext ctx, ROOTNET.Interface.NTH1 plot, int desiredBins)
        {
            var factor = (double)plot.NbinsX / (double)desiredBins;

            if (factor < 2.0)
            {
                return plot;
            }

            return rebin(ctx, plot, (int)factor);
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
        /// Each bin in the plot is the cut efficiency if you made the cut at those value.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="plot">The 2D plot to convert to an efficiency map</param>
        /// <param name="xCutGreaterThan">True if the cut is greater than along the x axis</param>
        /// <param name="yCutGreaterThan">True if the cut is greater than along the y axis</param>
        /// <returns>The efficiency map</returns>
        /// <remarks>
        /// The overflow and underflow bins are taken into account for the total calculations, so they will contain
        /// sensible results.
        /// 
        /// This works on a histogram, and thus a bin. So we have to define what the cut means. Since the cut is only has a real
        /// value at the bin edges, we must define it as such.
        ///   - For a ">" cut, the low edge of the bin is the cut value.
        ///   - for a "<" cut, the high edge of the bin is the cut value.
        /// 
        /// </remarks>
        public static ROOTNET.Interface.NTH2 asEfficiency(IScopeContext ctx, ROOTNET.Interface.NTH2 plot, bool xCutGreaterThan = true, bool yCutGreaterThan = true)
        {
            var result = plot.Clone() as ROOTNET.Interface.NTH2;
            Tags.CopyTags(ctx, plot, result);

            var xBins = plot.Xaxis.Nbins;
            var yBins = plot.Yaxis.Nbins;

            var xBinRange = BinOrdering(xCutGreaterThan, xBins);
            var yBinRange = BinOrdering(yCutGreaterThan, yBins);

            // We must now build a cumulative 2D matrix with the sizes for each.
            foreach (var ixBin in xBinRange)
            {
                foreach (var iyBin in yBinRange)
                {
                    var binSum = SumBinArea(plot,
                        BinOrdering(xCutGreaterThan, xBins, ixBin),
                        BinOrdering(yCutGreaterThan, yBins, iyBin));

                    result.SetBinContent(ixBin, iyBin, binSum);
                }
            }

            // Turn it into an efficiency
            var totalSum = SumBinArea(plot, BinOrdering(true, xBins), BinOrdering(true, yBins));
            foreach (var ixBin in xBinRange)
            {
                foreach (var iyBin in yBinRange)
                {
                    result.SetBinContent(ixBin, iyBin, result.GetBinContent(ixBin, iyBin) / totalSum);
                }
            }

            // Set up for good display
            result.Maximum = 1.0;
            return result;
        }

        /// <summary>
        /// Compare two tuples.
        /// </summary>
        private class TupleCompare : IEqualityComparer<Tuple<double, double>>
        {
            public bool Equals(Tuple<double, double> x, Tuple<double, double> y)
            {
                return x.Item1 == y.Item1
                    && x.Item2 == y.Item2;
            }

            public int GetHashCode(Tuple<double, double> obj)
            {
                return obj.Item1.GetHashCode() + obj.Item2.GetHashCode();
            }
        }

        /// <summary>
        /// Generate 2D turn on graphs from input signal and background plots.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="plot"></param>
        /// <param name="xCutGreaterThan"></param>
        /// <param name="yCutGreaterThan"></param>
        /// <returns>A graph with the signal eff along the x axis, and the background eff along the y axis</returns>
        public static ROOTNET.Interface.NTGraph asROC(IScopeContext ctx, NTH1 signal, NTH1 background, bool xCutGreaterThan = true, bool yCutGreaterThan = true)
        {
            // The two plots must be identical.
            if (signal.NbinsX != background.NbinsX)
            {
                throw new ArgumentException($"AsROC requires the same binning on the input plots (signal has {signal.NbinsX} and background has {background.NbinsX}).");
            }

            // Now, develop pairs of values so we can track the background and signal efficiency.
            var numberPairs = Enumerable.Range(0, signal.NbinsX + 1)
                .Select(ibin => Tuple.Create(signal.GetBinContent(ibin), background.GetBinContent(ibin)));

            // Now, turn them into efficiencies if we need to.
            var signalTotal = numberPairs.Select(p => p.Item1).Sum();
            var backgroundTotal = numberPairs.Select(p => p.Item2).Sum();

            double runningTotalSignal = xCutGreaterThan ? 0 : signalTotal;
            double runningTotalBackground = yCutGreaterThan ? 0 : backgroundTotal;

            Func<double, double> calcRunningSignal, calcRunningBackground;
            if (xCutGreaterThan)
            {
                calcRunningSignal = p => runningTotalSignal += p;
            } else
            {
                calcRunningSignal = p => runningTotalSignal -= p;
            }
            if (yCutGreaterThan)
            {
                calcRunningBackground = p => runningTotalBackground += p;
            }
            else
            {
                calcRunningBackground = p => runningTotalBackground -= p;
            }

            numberPairs = numberPairs
                .Select(p => Tuple.Create(calcRunningSignal(p.Item1), calcRunningBackground(p.Item2)))
                .Select(p => Tuple.Create(p.Item1 / signalTotal, p.Item2 / backgroundTotal))
                .ToArray(); // Side effects, make sure this gets run only once!

            // Remove the non-unique pairs, since this is going to be a scatter plot.
            numberPairs = numberPairs
                .Distinct(new TupleCompare());

            // Next, draw them in a graph.
            var pts = numberPairs.ToArray();
            var graf = new ROOTNET.NTGraph(pts.Length, pts.Select(p => p.Item1).ToArray(), pts.Select(p => p.Item2).ToArray());
            graf.FillColor = 0; // Make sure the background is white

            // Track tags for the signal (assuming the background is "common"), and track everything else.
            Tags.CopyTags(ctx, signal, graf);
            graf.SetTitle($"{signal.Title} ROC");
            graf.Xaxis.Title = $"Efficiency (signal)";
            graf.Yaxis.Title = $"Efficiency (background)";
            graf.Histogram.Maximum = 1.0;
            graf.Histogram.Minimum = 0.0;

            return graf;
        }

        /// <summary>
        /// Generate an integral plot out of the current plot.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="plot"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 asIntegral(IScopeContext ctx, NTH1 plot, bool sumForward = true)
        {
            // Clone it.
            var result = plot.Clone() as ROOTNET.Interface.NTH1; ;
            Tags.CopyTags(ctx, plot, result);

            // get the numbers out
            var numbers = Enumerable.Range(0, result.NbinsX + 1)
                .Select(ibin => result.GetBinContent(ibin));

            // Now, sum them up.
            var total = numbers.Sum();

            // And transform them.
            double runningTotal = sumForward ? 0 : total;
            Func<double, double> calRunningTotal;
            if (sumForward)
            {
                calRunningTotal = p => runningTotal += p;
            } else
            {
                calRunningTotal = p => runningTotal -= p;
            }

            numbers = numbers
                .Select(n => calRunningTotal(n))
                .ToArray(); // The runningTotal has side effects, so we better put a stop.

            // Stuff them back in result.
            foreach (var n in Enumerable.Range(0, result.NbinsX + 1).Zip(numbers, (ibin, v) => Tuple.Create(ibin, v)))
            {
                result.SetBinContent(n.Item1, n.Item2);
            }

            return result;
        }

        /// <summary>
        /// Sum the bins in an area given.
        /// </summary>
        /// <param name="plot"></param>
        /// <param name="enumerable1"></param>
        /// <param name="enumerable2"></param>
        /// <returns></returns>
        private static double SumBinArea(NTH2 plot, IEnumerable<int> xBins, IEnumerable<int> yBins)
        {
            double total = 0.0;
            foreach (var ix in xBins)
            {
                foreach (var iy in yBins)
                {
                    total += plot.GetBinContent(ix, iy);
                }

            }
            return total;
        }

        /// <summary>
        /// Helper function to set the order we move through a set of columns or rows.
        /// </summary>
        /// <param name="moveForward">Go forward if true, otherwise reverse</param>
        /// <param name="totalBins">Total number of bins we should be looking at.</param>
        /// <returns></returns>
        private static IEnumerable<int> BinOrdering(bool moveForward, int totalBins, int? initialBin = null)
        {
            if (moveForward)
            {
                int first = 0;
                if (initialBin.HasValue)
                {
                    first = initialBin.Value;
                }
                return Enumerable.Range(first, totalBins + 2);
            } else
            {
                int first = totalBins + 1;
                if (initialBin.HasValue)
                {
                    first = initialBin.Value;
                }
                return Enumerable.Range(0, first+1).Reverse();
            }
        }

        /// <summary>
        /// Create a plot out of a single ratio point.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="xpos"></param>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static NTH1 ratioPointAsPlot (NTH1 template, double xpos, double numerator, double denominator)
        {
            // Make a clone, and zero it out.
            var hNum = template.Clone() as NTH1;
            hNum.Reset();
            var hDen = hNum.Clone() as NTH1;

            // Now, do the division
            var index = hNum.FindBin(xpos);
            hNum.SetBinContent(index, numerator);
            hNum.SetBinError(index, Math.Sqrt(numerator));
            hDen.SetBinContent(index, denominator);
            hDen.SetBinError(index, Math.Sqrt(denominator));

            hNum.Divide(hDen);

            return hNum;
        }

        /// <summary>
        /// Returna plot that has as its content its value divided by its error, and zero errors.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static NTH1 asSigma(IScopeContext ctx, NTH1 plot)
        {
            var result = plot.Clone() as NTH1;
            Tags.CopyTags(ctx, plot, result);

            result.Reset();

            // Besure to do the overflow bins as well.
            for (int i_bin_x = 0; i_bin_x < result.NbinsX+2; i_bin_x++)
            {
                for (int i_bin_y = 0; i_bin_y < result.NbinsY+2; i_bin_y++)
                {
                    for (int i_bin_z = 0; i_bin_z < result.NbinsZ+2; i_bin_z++)
                    {
                        var v = plot.GetBinContent(i_bin_x, i_bin_y, i_bin_z);
                        var e = plot.GetBinError(i_bin_x, i_bin_y, i_bin_z);

                        var sig = e == 0 ? 0.0 : v / e;
                        result.SetBinContent(i_bin_x, i_bin_y, i_bin_z, sig);
                    }
                }
            }

            return result;

        }
    }
}
