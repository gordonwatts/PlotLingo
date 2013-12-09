using PlotLingoLib;
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
        public static ROOTNET.Interface.NTH1 OperatorPlus(ROOTNET.Interface.NTH1 h1, ROOTNET.Interface.NTH1 h2)
        {
            var clone = h1.Clone() as ROOTNET.Interface.NTH1;
            clone.Add(h2);
            Tags.CopyTags(h1, clone);
            Tags.CopyTags(h2, clone);
            return clone;
        }

        /// <summary>
        /// Scale the contents of a histogram up or down.
        /// </summary>
        /// <param name="h"></param>
        /// <param name="scaleFactor"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 OperatorMultiply(ROOTNET.Interface.NTH1 h, double scaleFactor)
        {
            var clone = h.Clone() as ROOTNET.Interface.NTH1;
            clone.Scale(scaleFactor);
            Tags.CopyTags(h, clone);
            return clone;
        }
    }
}
