﻿using PlotLingoLib;
using System;
using System.ComponentModel.Composition;

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
    }
}
